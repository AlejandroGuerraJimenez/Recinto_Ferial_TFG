namespace Fairground.Core.Attractions
{
    /// <summary>
    /// Shipped booth numbers. Rules, scenes, and the first HUD read these constants.
    /// </summary>
    public static class BalloonThrowBalance
    {
        public const int PointsPerBalloon = 10;
        public const int StartingThrows = 8;
        public const int BalloonCount = 6;
    }

    public static class DuckFishingBalance
    {
        public const int PointsPerDuck = 10;
        public const int StartingAttempts = 8;
        public const int DuckCount = 6;
        public const int DucksToWin = 4;
    }

    public static class BasketballHoopBalance
    {
        public const int PointsPerBasket = 2;
        public const int StartingBalls = 10;
        public const int WinScoreThreshold = 5;
    }

    public static class PunchingBallBalance
    {
        public const int StartingPunches = 5;
        public const int WinScoreThreshold = 700;
        public const int MaxScore = 999;
        public const float MaxImpactSpeed = 8f;
    }

    public static class FerrisWheelBalance
    {
        public const float DegreesPerSecond = 6f;
    }
}
