using Unity.XR.CoreUtils;
using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine.XR;
#endif

namespace Fairground.View
{
    public sealed class RightControllerAim : IAimRaySource
    {
        Transform _controller;
#if UNITY_EDITOR
        readonly List<InputDevice> _devices = new List<InputDevice>();
#endif

        public RightControllerAim(Transform controller) => _controller = controller;

        public bool TryGetRay(out Ray ray)
        {
            Resolve();
            if (!CanUse())
                return AimRays.Miss(out ray);

            ray = new Ray(_controller.position, _controller.forward);
            return true;
        }

        void Resolve()
        {
            if (_controller != null)
                return;

            _controller = FindRight();
        }

        bool CanUse() => _controller != null && TrackedOrDevice();

        bool TrackedOrDevice()
        {
#if UNITY_EDITOR
            return IsTracked();
#else
            return true;
#endif
        }

        static Transform FindRight()
        {
            var origin = Object.FindAnyObjectByType<XROrigin>();
            if (origin == null)
                return null;

            return FindNamedChild(origin.transform, "Right Controller")
                   ?? FindNamedChild(origin.transform, "RightHand");
        }

        static Transform FindNamedChild(Transform root, string name)
        {
            var transforms = root.GetComponentsInChildren<Transform>(true);
            for (var i = 0; i < transforms.Length; i++)
                if (string.Equals(transforms[i].name, name, System.StringComparison.OrdinalIgnoreCase))
                    return transforms[i];

            return null;
        }

#if UNITY_EDITOR
        bool IsTracked()
        {
            InputDevices.GetDevicesAtXRNode(XRNode.RightHand, _devices);
            if (!HasDevice())
                return false;

            return ReportsTracked();
        }

        bool HasDevice() => _devices.Count > 0 && _devices[0].isValid;

        bool ReportsTracked()
        {
            if (!_devices[0].TryGetFeatureValue(CommonUsages.isTracked, out var tracked))
                return true;

            return tracked;
        }
#endif
    }
}
