using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Editor Game View movement. A Mock HMD reports itself as an active device and
    /// leaves the camera on its prefab pose, so the booth looks frozen.
    /// </summary>
    public static class AttractionDesktopPlay
    {
        public const float MoveSpeed = 2.5f;

        public static bool IsLiveHeadset()
        {
            if (!XRSettings.enabled || !XRSettings.isDeviceActive)
                return false;

            string name = XRSettings.loadedDeviceName ?? string.Empty;
            if (name.Length == 0 || name.IndexOf("Mock", StringComparison.OrdinalIgnoreCase) >= 0)
                return false;

            var displays = new List<XRDisplaySubsystem>();
            SubsystemManager.GetSubsystems(displays);
            for (int i = 0; i < displays.Count; i++)
            {
                XRDisplaySubsystem display = displays[i];
                if (display.running && display.TryGetDisplayRefreshRate(out float hz) && hz > 1f)
                    return true;
            }

            return false;
        }

        public static void MoveWithKeyboard(Transform camera, float speed)
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null || camera == null)
                return;

            float x = (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f);
            float z = (keyboard.wKey.isPressed ? 1f : 0f) - (keyboard.sKey.isPressed ? 1f : 0f);
            if (Mathf.Approximately(x, 0f) && Mathf.Approximately(z, 0f))
                return;

            Vector3 forward = camera.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.0001f)
                forward = Vector3.forward;
            else
                forward.Normalize();

            Vector3 right = camera.right;
            right.y = 0f;
            right.Normalize();

            Vector3 step = forward * z + right * x;
            if (step.sqrMagnitude > 1f)
                step.Normalize();

            camera.position += step * speed * Time.deltaTime;
        }
    }
}
