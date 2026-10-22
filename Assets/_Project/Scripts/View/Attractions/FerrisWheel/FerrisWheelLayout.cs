using UnityEngine;

namespace Fairground.View.Attractions.FerrisWheel
{
    /// <summary>
    /// Human-scale wheel in front of the XR origin, same standing spot as the booths.
    /// </summary>
    public static class FerrisWheelLayout
    {
        public const float Radius = 2.2f;
        public const float HubHeight = 3.25f;
        public const float WheelDistance = 4.8f;
        public const int RimSegments = 20;
        public const float CabinHang = 0.48f;

        public static Vector3 WheelCenter => new Vector3(0f, HubHeight, WheelDistance);

        /// <summary>Editor camera, far enough back to frame the whole wheel.</summary>
        public static readonly Vector3 DesktopEye = new Vector3(0f, 1.65f, -2.2f);
    }
}
