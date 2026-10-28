using System;
using Fairground.Core.Attractions;
using Fairground.Model.Attractions;

namespace Fairground.Model.Attractions.BalloonThrow
{
    /// <summary>
    /// Immutable scoring and win/lose policy.
    /// </summary>
    public sealed class BalloonThrowRules
    {
        public const int DefaultPointsPerBalloon = BalloonThrowBalance.PointsPerBalloon;
        public const int DefaultStartingThrows = BalloonThrowBalance.StartingThrows;
        public const int DefaultBalloonCount = BalloonThrowBalance.BalloonCount;

        public BalloonThrowRules(
            int pointsPerBalloon = DefaultPointsPerBalloon,
            int startingThrows = DefaultStartingThrows,
            int balloonCount = DefaultBalloonCount)
        {
            GuardNonNegative(pointsPerBalloon, nameof(pointsPerBalloon));
            GuardPositive(startingThrows, nameof(startingThrows));
            GuardPositive(balloonCount, nameof(balloonCount));
            PointsPerBalloon = pointsPerBalloon;
            StartingThrows = startingThrows;
            BalloonCount = balloonCount;
            Attempt = AttemptMatchRules.BySuccessCount(
                pointsPerBalloon,
                startingThrows,
                balloonCount,
                balloonCount);
        }

        public int PointsPerBalloon { get; }
        public int StartingThrows { get; }
        public int BalloonCount { get; }
        public AttemptMatchRules Attempt { get; }

        static void GuardNonNegative(int value, string name)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(name);
        }

        static void GuardPositive(int value, string name)
        {
            if (value < 1)
                throw new ArgumentOutOfRangeException(name);
        }
    }
}
