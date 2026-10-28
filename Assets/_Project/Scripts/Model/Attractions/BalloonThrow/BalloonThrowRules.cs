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
            RuleGuard.NonNegative(pointsPerBalloon, nameof(pointsPerBalloon));
            RuleGuard.Positive(startingThrows, nameof(startingThrows));
            RuleGuard.Positive(balloonCount, nameof(balloonCount));
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
    }
}
