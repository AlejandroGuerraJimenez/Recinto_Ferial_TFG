namespace Fairground.View.Attractions.FerrisWheel
{
    /// <summary>
    /// London Eye ride in Cesium's local frame: X east, Y up, Z north.
    /// The globe origin is the hub. One capsule starts at the bottom.
    /// </summary>
    public static class LondonEyeLayout
    {
        public const double Latitude = 51.5033;
        public const double Longitude = -0.1196;
        public const double HubHeightMeters = 75.0;

        public const float RadiusMeters = 60f;

        /// <summary>
        /// Metres along the axle, toward the window, so the capsule sits outside the photorealistic rim.
        /// </summary>
        public const float OutwardClearanceMeters = 6f;

        /// <summary>
        /// Yaw of the axle, in degrees. Zero points the axle north.
        /// Negative yaw turns the capsule window from north toward west.
        /// -140 looks from the hub toward Westminster, across the Thames. Tune in the editor.
        /// </summary>
        public const float AxisYawDegrees = -140f;

        /// <summary>One turn per minute. The model default stays at 16.</summary>
        public const float DegreesPerSecond = 6f;

        public const int CabinCount = 1;

        public const long GooglePhotorealisticAssetId = 2275207;
        public const float EditorMaximumScreenSpaceError = 16f;
        public const float QuestMaximumScreenSpaceError = 32f;
        public const long EditorMaximumCachedBytes = 512L * 1024L * 1024L;
        public const long QuestMaximumCachedBytes = 128L * 1024L * 1024L;
    }
}
