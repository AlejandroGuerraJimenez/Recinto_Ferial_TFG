using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.OpenXR.Features.Meta;

namespace Fairground.View
{
    public sealed class PlaneDetectionSession
    {
        /// <summary>
        /// Every mode the provider can return. Quest drops a floor when the request omits the flag it used.
        /// The selection filter still keeps only upward surfaces.
        /// </summary>
        public const PlaneDetectionMode FlatSurfaces =
            PlaneDetectionMode.Horizontal | PlaneDetectionMode.Vertical | PlaneDetectionMode.NotAxisAligned;

        ARPlaneManager _manager;
        GameObject _prefab;
        bool _listening;
        UnityAction<ARTrackablesChangedEventArgs<ARPlane>> _listener;

        public PlaneDetectionSession(ARPlaneManager manager) => _manager = manager;

        public ARPlaneManager Manager => _manager;

        public bool IsSubsystemRunning => _manager != null && _manager.subsystem != null && _manager.subsystem.running;

        /// <summary>
        /// Binds the manager again. AR Foundation looks up the plane subsystem only inside OnEnable.
        /// Enabling before OpenXR has loaded that subsystem leaves it null for the rest of the session.
        /// </summary>
        public void TryAttach(GameObject prefab, UnityAction<ARTrackablesChangedEventArgs<ARPlane>> listener)
        {
            _prefab = prefab;
            EnsureManager();
            if (_manager == null)
                return;

            Apply(prefab);
            Listen(listener);
            if (_manager.subsystem != null)
            {
                if (!_manager.enabled)
                    Enable();
                return;
            }

            if (_manager.enabled)
                _manager.enabled = false;
            Enable();
        }

        /// <summary>
        /// Opens the Quest room-capture panel. The provider stays empty until that scene model exists.
        /// </summary>
        public bool TryOpenRoomCapture()
        {
            var session = Object.FindAnyObjectByType<ARSession>();
            if (session == null || session.subsystem is not MetaOpenXRSessionSubsystem meta)
                return false;

            return meta.TryRequestSceneCapture();
        }

        public string Report(int tracked, int candidates)
        {
            if (_manager == null)
                return "No hay ARPlaneManager en el XR Origin.";

            var session = ARSession.state.ToString();
            if (_manager.subsystem == null)
                return "Sesión " + session + ".\nSin subsistema de planos: OpenXR aún no lo ha creado.\nB vuelve a intentarlo.";

            var running = _manager.subsystem.running ? "en marcha" : "parado";
            var sample = tracked > 0 && candidates == 0 ? "\n" + FirstPlane() : string.Empty;
            return "Sesión " + session + ". Subsistema " + running + ".\nCasco " + tracked + ", válidos " + candidates + "." + sample + "\nB vuelve a arrancar la detección.";
        }

        public void Prepare(GameObject prefab)
        {
            EnsureManager();
            if (_manager == null)
                return;

            Apply(prefab);
        }

        public void Listen(UnityAction<ARTrackablesChangedEventArgs<ARPlane>> listener)
        {
            _listener = listener;
            if (_manager == null || _listening)
                return;

            _manager.trackablesChanged.AddListener(listener);
            _listening = true;
        }

        public void StopListening()
        {
            if (_manager == null || !_listening || _listener == null)
                return;

            _manager.trackablesChanged.RemoveListener(_listener);
            _listening = false;
        }

        public void Pause()
        {
            if (_manager == null)
                return;

            _manager.requestedDetectionMode = PlaneDetectionMode.None;
        }

        public void ResumeHorizontal(bool permitted)
        {
            if (!permitted || _manager == null)
                return;

            _manager.requestedDetectionMode = FlatSurfaces;
            _manager.enabled = true;
        }

        void EnsureManager()
        {
            if (_manager != null)
                return;

            _manager = FindOnOrigin();
        }

        static ARPlaneManager FindOnOrigin()
        {
            var origin = Object.FindAnyObjectByType<XROrigin>();
            if (origin == null)
                return MissingOrigin();

            return ExistingOrNew(origin);
        }

        static ARPlaneManager MissingOrigin()
        {
            Debug.LogError("Plane selection requires an XR Origin.");
            return null;
        }

        static ARPlaneManager ExistingOrNew(XROrigin origin)
        {
            var manager = origin.GetComponent<ARPlaneManager>();
            return manager != null ? manager : CreateDisabled(origin);
        }

        static ARPlaneManager CreateDisabled(XROrigin origin)
        {
            var manager = origin.gameObject.AddComponent<ARPlaneManager>();
            manager.enabled = false;
            return manager;
        }

        string FirstPlane()
        {
            foreach (var plane in _manager.trackables)
            {
                if (plane == null)
                    continue;

                var points = plane.boundary.IsCreated ? plane.boundary.Length : 0;
                var absorbed = plane.subsumedBy != null ? " absorbido" : string.Empty;
                return plane.classifications + " " + plane.alignment + " puntos " + points + " arriba " + plane.transform.up.y.ToString("0.00") + absorbed;
            }

            return string.Empty;
        }

        void Apply(GameObject prefab)
        {
            _prefab = prefab != null ? prefab : _prefab;
            _manager.planePrefab = _prefab;
            _manager.requestedDetectionMode = FlatSurfaces;
        }

        void Enable()
        {
            _manager.enabled = true;
            Debug.Log("Scene permission granted. Flat surface detection started.");
        }
    }
}
