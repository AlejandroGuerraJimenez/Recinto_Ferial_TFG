using System;
using Fairground.Core.Planes;
using Fairground.ViewModel;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.ARFoundation;

namespace Fairground.View
{
    /// <summary>
    /// Detects horizontal surfaces and lets the user confirm one.
    /// Quest 3 reads those surfaces from the Space Setup scene model (XR_FB_scene), not a live mobile scan.
    /// There is no start button. Detection attaches itself once the OpenXR plane subsystem exists.
    /// Right trigger confirms. Right B on this menu clears the choice and attaches detection again.
    /// Right B inside an attraction returns here, with the floor still locked.
    /// Left Y closes the Quest build.
    /// In the editor, without a headset: aim with the view, left click or Space confirms, R restarts.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Fairground/Plane Selection")]
    public sealed class PlaneSelectionView : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("m_PlaneManager")] ARPlaneManager planeManager;
        [SerializeField, FormerlySerializedAs("m_PlanePrefab")] GameObject planePrefab;
        [SerializeField, FormerlySerializedAs("m_RightController")] Transform rightController;
        [SerializeField, FormerlySerializedAs("m_RayDistance")] float rayDistance = 8f;
        [SerializeField, FormerlySerializedAs("m_EnableEditorStandIn")] bool enableEditorStandIn = true;

        PlaneSelectionViewModel _viewModel;
        PlaneDetectionSession _detection;
        PlaneVisualCatalog _catalog;
        PlanePointer _pointer;
        SelectedTrackableGuard _trackable;
        PlaneAimController _aim;
        AimRayView _aimView;
        PlaneSelectionInput _input;
        PlaneAppearanceController _appearance;
        EditorStandInHost _standIn;
        PlaneRegistration _registration;
        PlaneDetectionStatusView _status;
        readonly PlaneStatusText _statusText = new PlaneStatusText();
        bool _permissionGranted;
        bool _permissionSettled;
        float _nextAttach;
        float _nextReport;
        float _scanSince = -1f;
        int _attachAttempts;
        bool _askedForRoom;
        float _ignoreRestartUntil;
        Action _confirm;
        Action _restart;
        Func<bool> _isAwaiting;

        /// <summary>
        /// Fired once when the user confirms a surface. Other surfaces are hidden. Parcels stay on this one.
        /// </summary>
        public event Action<DetectedPlane> PlaneSelected;

        /// <summary>
        /// Fired when the trigger is pressed again after a surface is already locked.
        /// </summary>
        public event Action LockedConfirm;

        /// <summary>
        /// Fired when the user clears the locked surface and detection starts again.
        /// </summary>
        public event Action OnRestarted;

        public DetectedPlane? SelectedPlane => _viewModel?.SelectedPlane;

        /// <summary>
        /// Live trackable for <see cref="SelectedPlane"/> when the device still has it. Null for the editor stand-in.
        /// </summary>
        public ARPlane SelectedTrackable => _trackable != null ? _trackable.Plane : null;

        /// <summary>
        /// Highest-ranked surface while the user is still choosing. Parcels are drawn on this one.
        /// </summary>
        public PlaneSurfaceVisual Proposal => _appearance != null ? _appearance.Proposal : null;

        /// <summary>
        /// Surface kept after confirmation. The other detected surfaces are hidden.
        /// </summary>
        public PlaneSurfaceVisual LockedSurface => _pointer != null ? _pointer.Selected : null;

        public PlaneSelectionState State => _viewModel != null ? _viewModel.State : PlaneSelectionState.AwaitingSelection;

        public bool TryGetAim(out Ray ray)
        {
            ray = default;
            if (_aim == null || !_aim.HasAim)
                return false;

            var direction = _aim.End - _aim.Origin;
            if (direction.sqrMagnitude < 0.000001f)
                return false;

            ray = new Ray(_aim.Origin, direction.normalized);
            return true;
        }

        void Awake()
        {
            _catalog = new PlaneVisualCatalog();
            _pointer = new PlanePointer();
            _trackable = new SelectedTrackableGuard();
            CreateViewModel();
            OpenDetection();
        }

        void OnEnable()
        {
            _input.Enable();
            _detection.Listen(_registration.HandleTrackablesChanged);
            EnsurePermission();
        }

        void OnDisable()
        {
            _input.Disable();
            _detection.StopListening();
        }

        void OnDestroy()
        {
            Unhook();
            _detection?.StopListening();
            _input?.Dispose();
            _aimView?.Release();
            if (_status != null)
                Destroy(_status.gameObject);
        }

        void LateUpdate()
        {
            PumpDetection();
            _catalog.Prune();
            _standIn.Tick(_catalog);
            _aim.Update(State);
            _appearance.Apply(State);
            _input.Read();
            _aimView.Draw(ShowAim(), _aim.Origin, _aim.End, _aim.Hovered != null);
            var status = DescribeStatus();
            _status?.Present(status);
            LogStatus(status);
        }

        /// <summary>
        /// Shows the parcel menu on a floor chosen before the current scene loaded.
        /// </summary>
        public bool TryRestore(DetectedPlane plane)
        {
            if (_viewModel == null || State != PlaneSelectionState.AwaitingSelection)
                return false;

            if (!_viewModel.IsSelectable(plane))
                return false;

            var visual = SavedPlaneSurface.Spawn(plane, transform, FillTemplate(), BorderTemplate());
            _catalog.Track(visual);
            _pointer.Selected = visual;
            if (_viewModel.SelectPlane(plane))
            {
                _ignoreRestartUntil = Time.unscaledTime + 0.4f;
                return true;
            }

            _pointer.Selected = null;
            _catalog.TryTake(plane.Id, out _);
            Destroy(visual.gameObject);
            return false;
        }

        /// <summary>
        /// Clears the confirmed surface and starts detection again.
        /// </summary>
        public void RestartSelection()
        {
            if (Time.unscaledTime < _ignoreRestartUntil)
                return;

            if (!_permissionGranted)
            {
                _permissionSettled = false;
                EnsurePermission();
            }

            _nextAttach = 0f;
            _attachAttempts = 0;
            _scanSince = -1f;
            _askedForRoom = false;
            _detection.Attach(planePrefab, _registration.HandleTrackablesChanged);
            _viewModel?.RestartSelection();
            OnRestarted?.Invoke();
        }

        void CreateViewModel()
        {
            _viewModel = new PlaneSelectionViewModel();
            _viewModel.PlaneSelected += HandlePlaneSelected;
            _viewModel.StateChanged += HandleStateChanged;
        }

        void OpenDetection()
        {
            _detection = new PlaneDetectionSession(planeManager);
            _aim = new PlaneAimController(AimSource(), rayDistance);
            _appearance = new PlaneAppearanceController(_catalog, _pointer, _aim);
            CreateTools();
        }

        void CreateTools()
        {
            _confirm = Confirm;
            _restart = RestartSelection;
            _isAwaiting = IsAwaiting;
            _standIn = new EditorStandInHost(enableEditorStandIn, transform, planePrefab, DetectionRunning);
            _registration = new PlaneRegistration(
                _catalog,
                _aim,
                _pointer,
                _standIn,
                () => State,
                snapshot => _viewModel.TryRank(snapshot, out var rank) ? (PlaneRank?)rank : null);
            _aimView = AimRayView.Create(transform, BorderTemplate());
            _status = PlaneDetectionStatusView.Create();
            _input = new PlaneSelectionInput(_confirm, _restart, EditorContext);
        }

        void EnsurePermission()
        {
            if (!_permissionSettled)
                ScenePermissionRequester.Request(OnGranted, OnDenied);
        }

        void OnGranted()
        {
            MarkGranted(true);
            _nextAttach = 0f;
        }

        void PumpDetection()
        {
            if (!IsAwaiting() || PermissionDenied())
                return;

            if (ARSession.state < ARSessionState.SessionTracking)
                return;

            if (_detection.IsSubsystemRunning)
            {
                if (_catalog.Count < _detection.TrackedCount)
                    _registration.CatchUp(_detection.Manager);
                OfferRoomCapture();
                return;
            }

#if UNITY_EDITOR
            if (_attachAttempts > 0)
                return;
#endif
            if (Time.unscaledTime < _nextAttach)
                return;

            _nextAttach = Time.unscaledTime + 1.5f;
            _attachAttempts++;
            _detection.Attach(planePrefab, _registration.HandleTrackablesChanged);
        }

        void OnDenied()
        {
            MarkGranted(false);
            Debug.LogWarning("Scene permission denied (" + ScenePermissionRequester.UseScenePermission + "). Plane detection stays off.");
        }

        void LogStatus(string status)
        {
            if (_catalog.CandidateCount() > 0 || Time.unscaledTime < _nextReport)
                return;

            _nextReport = Time.unscaledTime + 2f;
            Debug.Log("[Planos] " + status.Replace("\n", " | "));
        }

        void HandlePlaneSelected(DetectedPlane plane) => PlaneSelected?.Invoke(plane);

        void HandleStateChanged(PlaneSelectionState state)
        {
            if (state == PlaneSelectionState.PlaneSelected)
                _detection.Pause();
            else
                Resume();
        }

        void Resume()
        {
            ReleaseHold();
            _detection.Prepare(planePrefab);
            _detection.ResumeDetection(_permissionGranted);
            _detection.Listen(_registration.HandleTrackablesChanged);
            _standIn.AllowAgain(_catalog);
        }

        void Confirm()
        {
            if (IsAwaiting())
            {
                if (CurrentTarget() == null)
                    return;

                Submit(HoldSelection());
                return;
            }

            if (State == PlaneSelectionState.PlaneSelected)
                LockedConfirm?.Invoke();
        }

        void Submit(DetectedPlane snapshot)
        {
            if (_viewModel.SelectPlane(snapshot))
                LogSelected(snapshot);
            else
                ReleaseHold();
        }

        DetectedPlane HoldSelection()
        {
            var target = CurrentTarget();
            var plane = target.GetComponent<ARPlane>();
            _pointer.Selected = target;
            _trackable.Hold(plane);
            return plane != null ? DetectedPlaneMapper.FromPlane(plane) : target.Snapshot;
        }

        PlaneSurfaceVisual CurrentTarget()
        {
            var hovered = _aim.Hovered;
            if (hovered != null && hovered.IsCandidate)
                return hovered;

            return _appearance.Proposal;
        }

        void ReleaseHold()
        {
            _pointer.Selected = null;
            _trackable.Release(_detection.Manager);
        }

        void Unhook()
        {
            if (_viewModel == null)
                return;

            _viewModel.PlaneSelected -= HandlePlaneSelected;
            _viewModel.StateChanged -= HandleStateChanged;
        }

        bool IsAwaiting() => State == PlaneSelectionState.AwaitingSelection;

        bool PermissionDenied() => _permissionSettled && !_permissionGranted;

        void OfferRoomCapture()
        {
            if (_askedForRoom || _detection.TrackedCount > 0)
                return;

            if (_scanSince < 0f)
                _scanSince = Time.unscaledTime;
            if (Time.unscaledTime - _scanSince < 8f)
                return;

            _askedForRoom = true;
            var opened = _detection.TryOpenRoomCapture();
            Debug.Log(opened
                ? "[Planos] Ningún plano. Abriendo la captura de sala del Quest."
                : "[Planos] Ningún plano y la captura de sala no está disponible.");
        }

        bool DetectionRunning() => _detection.IsSubsystemRunning;

        bool ShowAim() => _aim.HasAim && (IsAwaiting() || State == PlaneSelectionState.PlaneSelected);

        string DescribeStatus()
        {
            return _statusText.Current(
                PermissionDenied(),
                _permissionSettled,
                _detection.IsSubsystemRunning,
                _catalog.CandidateCount(),
                State == PlaneSelectionState.PlaneSelected,
                _detection.TrackedCount,
                _detection.FirstPlaneStamp,
                _detection.Manager,
                tracked => _detection.Report(tracked, 0));
        }

        EditorInputContext EditorContext()
        {
            return new EditorInputContext(_restart, _confirm, _isAwaiting, _aim, rayDistance);
        }

        IAimRaySource AimSource()
        {
#if UNITY_EDITOR
            return new FirstAvailableAim(new RightControllerAim(rightController), new EditorCameraAim());
#else
            return new RightControllerAim(rightController);
#endif
        }

        Material FillTemplate() => Template(visual => visual.FillTemplate);

        Material BorderTemplate() => Template(visual => visual.BorderTemplate);

        Material Template(System.Func<PlaneSurfaceVisual, Material> pick)
        {
            var visual = planePrefab != null ? planePrefab.GetComponent<PlaneSurfaceVisual>() : null;
            return visual != null ? pick(visual) : null;
        }

        void MarkGranted(bool granted)
        {
            _permissionGranted = granted;
            _permissionSettled = true;
        }

        static void LogSelected(DetectedPlane snapshot)
        {
            Debug.Log("Plane selected: " + snapshot.Id + " (" + snapshot.Classification + ").");
        }
    }
}
