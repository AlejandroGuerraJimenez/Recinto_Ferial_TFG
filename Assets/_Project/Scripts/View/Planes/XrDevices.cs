#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Fairground.View
{
    static class XrDevices
    {
        static readonly List<InputDevice> Devices = new List<InputDevice>();

        public static bool AnyPresent()
        {
            InputDevices.GetDevices(Devices);
            for (var i = 0; i < Devices.Count; i++)
                if (Devices[i].isValid)
                    return true;
            return false;
        }
    }
}
#endif
