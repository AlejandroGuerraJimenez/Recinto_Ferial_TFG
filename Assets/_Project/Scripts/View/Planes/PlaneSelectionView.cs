using System;
using Fairground.Model;
using Fairground.ViewModel;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Fairground.View
{
    /// <summary>
    /// Detects horizontal surfaces and lets the user confirm one.
    /// Quest 3 reads those surfaces from the Space Setup scene model (XR_FB_scene), not a live mobile scan.
    /// There is no start button. Detection attaches itself once the OpenXR plane subsystem exists.
    /// Right trigger confirms. Right B button (secondaryButton) clears the choice and attaches detection again.
    /// Left Y closes the Quest build.
    /// In the editor, without a headset: aim with the view, left click or Space confirms, R restarts.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Fairground/Plane Selection")]
    public sealed class PlaneSelectionView : MonoBehaviour
    {
        [SerializeField] ARPlaneManager m_PlaneManager;
        [SerializeField] GameObject m_PlanePrefab;
        [SerializeField] Transform m_RightController;
        [SerializeField] float m_RayDistance = 8f;
        [SerializeField] bool m_EnableEditorStandIn = true;

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
        bool _permissionGranted;
        bool _permissionSettled;
        float _nextAttach;
        float _nextReport;
        float _scanSince = -1f;
        int _attachAttempts;
        bool _askedForRoom;

        /// <summary>
        /// Fired once when the user confirms a surface. Other surfaces are hidden. Parcels stay on this one.
        /// </summary>
        public event Action<DetectedPlane> OnPlaneSelected;

        /// <summary>
        /// Fired when the trigger is pressed again after a surface is already locked.
        /// </summary>
        public event Action OnLockedConfirm;

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
            CreateSession();
        }

        void OnEnable()
        {
            _input.Enable();
            _detection.Listen(_registration.OnChanged);
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
            var status = StatusText();
            _status?.Present(status);
            LogStatus(status);
        }

        /// <summary>
        /// Clears the confirmed surface and starts detection again.
        /// </summary>
        public void RestartSelection()
        {
            if (!_permissionGranted)
            {
                _permissionSettled = false;
                EnsurePermission();
            }

            _nextAttach = 0f;
            _attachAttempts = 0;
            _scanSince = -1f;
            _askedForRoom = false;
            _detection.TryAttach(m_PlanePrefab, _registration.OnChanged);
            _viewModel?.RestartSelection();
            OnRestarted?.Invoke();
        }

        void CreateViewModel()
        {
            _viewModel = new PlaneSelectionViewModel();
            _viewModel.OnPlaneSelected += HandlePlaneSelected;
            _viewModel.StateChanged += HandleStateChanged;
        }

        void CreateSession()
        {
            _detection = new PlaneDetectionSession(m_PlaneManager);
            _aim = new PlaneAimController(AimSource(), m_RayDistance);
            _appearance = new PlaneAppearanceController(_catalog, _pointer, _aim);
            CreateTools();
        }

        void CreateTools()
        {
            _standIn = new EditorStandInHost(m_EnableEditorStandIn, transform, m_PlanePrefab, DetectionRunning);
            _registration = new PlaneRegistration(
                _catalog,
                _aim,
                _pointer,
                _standIn,
                () => State,
                snapshot => _viewModel.IsSelectable(snapshot));
            _aimView = AimRayView.Create(transform, BorderTemplate());
            _status = PlaneDetectionStatusView.Create();
            _input = new PlaneSelectionInput(Confirm, RestartSelection, EditorContext);
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
                if (_catalog.Count < TrackedPlaneCount())
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
            _detection.TryAttach(m_PlanePrefab, _registration.OnChanged);
        }

        void OnDenied()
        {
            MarkGranted(false);
            Debug.LogWarning("Scene permission denied (" + ScenePermissionRequester.UseScenePermission + "). Plane detection stays off.");
        }

        void LogStatus(string status)
        {
            if (CandidateCount() > 0 || Time.unscaledTime < _nextReport)
                return;

            _nextReport = Time.unscaledTime + 2f;
            Debug.Log("[Planos] " + status.Replace("\n", " | "));
        }

        void HandlePlaneSelected(DetectedPlane plane) => OnPlaneSelected?.Invoke(plane);

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
            _detection.Prepare(m_PlanePrefab);
            _detection.ResumeHorizontal(_permissionGranted);
            _detection.Listen(_registration.OnChanged);
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
                OnLockedConfirm?.Invoke();
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

            _viewModel.OnPlaneSelected -= HandlePlaneSelected;
            _viewModel.StateChanged -= HandleStateChanged;
        }

        bool IsAwaiting() => State == PlaneSelectionState.AwaitingSelection;

        bool PermissionDenied() => _permissionSettled && !_permissionGranted;

        void OfferRoomCapture()
        {
            if (_askedForRoom || TrackedPlaneCount() > 0)
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

        string StatusText()
        {
            const string quit = "\nY del mando izquierdo cierra la app.";
            if (PermissionDenied())
                return "Sin permiso de escena.\nPulsa B para volver a pedir el aviso.\nSi no sale: Ajustes, Aplicaciones, permisos, datos espaciales." + quit;

            if (!_permissionSettled && !_detection.IsSubsystemRunning)
                return "Permiso de escena.\nSi sale el aviso, acéptalo. La búsqueda empieza igual." + quit;

            var planes = CandidateCount();
            if (planes == 0)
                return _detection.Report(TrackedPlaneCount(), 0) + quit;

            if (State == PlaneSelectionState.PlaneSelected)
                return "Plano fijado. El resto queda fuera.\nApunta una parcela y pulsa el gatillo.\nB suelta el plano." + quit;

            return planes + " planos. El marcado es la propuesta.\nGatillo: el apuntado, o esa si no apuntas." + quit;
        }

        int TrackedPlaneCount()
        {
            var manager = _detection != null ? _detection.Manager : null;
            if (manager == null)
                return 0;

            var count = 0;
            foreach (var plane in manager.trackables)
            {
                if (plane != null)
                    count++;
            }

            return count;
        }

        int CandidateCount()
        {
            var count = 0;
            _catalog.ForEach(visual =>
            {
                if (visual != null && visual.IsCandidate)
                    count++;
            });
            return count;
        }

        EditorInputContext EditorContext()
        {
            return new EditorInputContext(RestartSelection, Confirm, IsAwaiting, _aim, m_RayDistance);
        }

        IAimRaySource AimSource()
        {
#if UNITY_EDITOR
            return new FirstAvailableAim(new RightControllerAim(m_RightController), new EditorCameraAim());
#else
            return new RightControllerAim(m_RightController);
#endif
        }

        Material BorderTemplate()
        {
            var visual = m_PlanePrefab != null ? m_PlanePrefab.GetComponent<PlaneSurfaceVisual>() : null;
            return visual != null ? visual.BorderTemplate : null;
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
