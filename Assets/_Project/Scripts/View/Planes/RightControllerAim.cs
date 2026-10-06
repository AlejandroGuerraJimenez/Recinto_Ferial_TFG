using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
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
            var controllers = Object.FindObjectsByType<ActionBasedController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            for (var i = 0; i < controllers.Length; i++)
                if (IsRight(controllers[i]))
                    return controllers[i].transform;

            return null;
        }

        static bool IsRight(ActionBasedController controller)
        {
            return controller.name.IndexOf("Right", System.StringComparison.OrdinalIgnoreCase) >= 0;
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
