namespace Fairground.View.Attractions.FerrisWheel
{
    /// <summary>
    /// London Eye ride in Cesium's local frame: X east, Y up, Z north.
    /// The globe origin is the hub. The project wheel is scaled onto that hub, and the player stands on the platform under it.
    /// </summary>
    public static class LondonEyeLayout
    {
        /// <summary>Hub, centre of the OpenStreetMap footprint of the wheel.</summary>
        public const double Latitude = 51.5033416;
        public const double Longitude = -0.1196765;

        /// <summary>
        /// Ellipsoid height of the hub. Ground here is about 50 m (terrain plus geoid),
        /// and the platform sits 73 m below the hub, so 123 m puts that floor on the bank.
        /// </summary>
        public const double HubHeightMeters = 123.0;

        public const float RadiusMeters = 60f;

        /// <summary>
        /// Metres from the hub down to the walkable platform.
        /// The scaled cabins used to hang about 10 m below this and through the ground.
        /// </summary>
        public const float FootBelowHubMeters = 73f;

        public const float FloorThicknessMeters = 0.4f;
        public const float PlatformHalfAlongMeters = 70f;
        public const float PlatformHalfAxleMeters = 52f;
        public const float EyeHeightMeters = 1.7f;
        public const float CabinClearanceMeters = 1.5f;

        /// <summary>Player spawn on the platform, on the land side of the axle. The booth stands just in front.</summary>
        public static float PlayerLocalZ => -(PlatformHalfAxleMeters - 8f);

        public const float TicketReachMeters = 12f;

        /// <summary>Half-length of the tileset hole, along the wheel, in metres.</summary>
        public const float CutoutHalfLengthMeters = 80f;

        /// <summary>Half-width of the tileset hole, along the axle, in metres.</summary>
        public const float CutoutHalfWidthMeters = 28f;

        /// <summary>
        /// Yaw of the axle, in degrees. Zero points the axle north.
        /// The real wheel stands in a north-south plane, so the axle runs east-west.
        /// -82.7 points the window west, toward Westminster Bridge.
        /// </summary>
        public const float AxisYawDegrees = -82.7f;

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
