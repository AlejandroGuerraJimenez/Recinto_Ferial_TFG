using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Fairground.View
{
    public sealed class PlaneDetectionSession
    {
        ARPlaneManager _manager;
        bool _listening;
        UnityAction<ARTrackablesChangedEventArgs<ARPlane>> _listener;

        public PlaneDetectionSession(ARPlaneManager manager) => _manager = manager;

        public ARPlaneManager Manager => _manager;

        public bool IsSubsystemRunning => _manager != null && _manager.subsystem != null && _manager.subsystem.running;

        public void Begin(GameObject prefab)
        {
            Prepare(prefab);
            if (_manager != null)
                Enable();
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

            _manager.requestedDetectionMode = PlaneDetectionMode.Horizontal;
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

        void Apply(GameObject prefab)
        {
            _manager.planePrefab = prefab;
            _manager.requestedDetectionMode = PlaneDetectionMode.Horizontal;
        }

        void Enable()
        {
            _manager.enabled = true;
            Debug.Log("Scene permission granted. Horizontal plane detection started.");
        }
    }
}
