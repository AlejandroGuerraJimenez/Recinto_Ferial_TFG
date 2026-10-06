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
    /// Right trigger confirms. Right B button (secondaryButton) clears the choice and scans again.
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
        bool _permissionGranted;
        bool _permissionSettled;

        /// <summary>
        /// Fired once when the user confirms a surface. The snapshot is the input for a later rectangle step.
        /// </summary>
        public event Action<DetectedPlane> OnPlaneSelected;

        public DetectedPlane? SelectedPlane => _viewModel?.SelectedPlane;

        /// <summary>
        /// Live trackable for <see cref="SelectedPlane"/> when the device still has it. Null for the editor stand-in.
        /// </summary>
        public ARPlane SelectedTrackable => _trackable != null ? _trackable.Plane : null;

        public PlaneSelectionState State => _viewModel != null ? _viewModel.State : PlaneSelectionState.AwaitingSelection;

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
        }

        void LateUpdate()
        {
            _catalog.Prune();
            _standIn.Tick(_catalog);
            _aim.Update(State);
            _input.Read();
            _appearance.Apply(State);
            _aimView.Draw(ShowAim(), _aim.Origin, _aim.End, _aim.Hovered != null);
        }

        /// <summary>
        /// Clears the confirmed surface and starts detection again.
        /// </summary>
        public void RestartSelection() => _viewModel?.RestartSelection();

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
            _registration = new PlaneRegistration(_catalog, _aim, _pointer, _standIn, () => State);
            _aimView = AimRayView.Create(transform, BorderTemplate());
            _input = new PlaneSelectionInput(Confirm, RestartSelection, EditorContext);
        }

        void EnsurePermission()
        {
            if (!_permissionSettled)
                ScenePermissionRequester.Request(OnGranted, OnDenied);
            else if (CanScan())
                Resume();
        }

        void OnGranted()
        {
            MarkGranted(true);
            if (isActiveAndEnabled)
                StartDetection();
        }

        void OnDenied()
        {
            MarkGranted(false);
            Debug.LogWarning("Scene permission denied (" + ScenePermissionRequester.UseScenePermission + "). Plane detection stays off.");
        }

        void StartDetection()
        {
            _detection.Begin(m_PlanePrefab);
            _detection.Listen(_registration.OnChanged);
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
            if (!CanConfirm())
                return;

            Submit(HoldSelection());
        }

        void Submit(DetectedPlane snapshot)
        {
            if (_viewModel.SelectPlane(snapshot))
                LogSelected(snapshot);
            else
                ReleaseHold();
        }

        bool CanConfirm()
        {
            var hovered = _aim.Hovered;
            return IsAwaiting() && hovered != null && hovered.IsCandidate;
        }

        DetectedPlane HoldSelection()
        {
            var hovered = _aim.Hovered;
            var plane = hovered.GetComponent<ARPlane>();
            _pointer.Selected = hovered;
            _trackable.Hold(plane);
            return plane != null ? DetectedPlaneMapper.FromPlane(plane) : hovered.Snapshot;
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

        bool CanScan() => _permissionGranted && IsAwaiting();

        bool DetectionRunning() => _detection.IsSubsystemRunning;

        bool ShowAim() => _aim.HasAim && State == PlaneSelectionState.AwaitingSelection;

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
