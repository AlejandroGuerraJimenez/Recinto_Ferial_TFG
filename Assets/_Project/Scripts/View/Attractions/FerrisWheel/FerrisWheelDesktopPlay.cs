using CesiumForUnity;
using UnityEngine;

namespace Fairground.View.Attractions.FerrisWheel
{
    /// <summary>
    /// Editor look for the wheel. The ride already places the camera when Cesium is present.
    /// </summary>
    public static class FerrisWheelDesktopPlay
    {
        public static readonly Color Sky = new Color(0.45f, 0.68f, 0.9f, 1f);

        public static DesktopPlayProfile Profile()
        {
            return new DesktopPlayProfile
            {
                FallbackSky = Sky,
                UseCustomPlacement = true,
                Place = Place,
                AllowMove = camera => !IsRiding(camera),
            };
        }

        static void Place(Transform camera)
        {
            if (camera == null || camera.GetComponentInParent<CesiumGeoreference>() != null)
                return;

            Vector3 eye = FerrisWheelLayout.DesktopEye;
            Vector3 hub = FerrisWheelLayout.WheelCenter;
            camera.SetPositionAndRotation(eye, Quaternion.LookRotation(hub - eye, Vector3.up));
        }

        static bool IsRiding(Transform camera)
        {
            for (Transform cursor = camera; cursor != null; cursor = cursor.parent)
            {
                if (cursor.name == FerrisWheelTicketBooth.RiderAnchorName)
                    return true;
            }

            return false;
        }
    }
}
