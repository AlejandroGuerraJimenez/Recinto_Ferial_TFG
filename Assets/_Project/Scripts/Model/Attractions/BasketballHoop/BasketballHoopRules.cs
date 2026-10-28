using Fairground.Core.Attractions;
using Fairground.Model.Attractions;

namespace Fairground.Model.Attractions.BasketballHoop
{
    /// <summary>
    /// Immutable scoring and win/lose policy.
    /// </summary>
    public sealed class BasketballHoopRules
    {
        public const int DefaultPointsPerBasket = BasketballHoopBalance.PointsPerBasket;
        public const int DefaultStartingBalls = BasketballHoopBalance.StartingBalls;
        public const int DefaultWinScoreThreshold = BasketballHoopBalance.WinScoreThreshold;

        public BasketballHoopRules(
            int pointsPerBasket = DefaultPointsPerBasket,
            int startingBalls = DefaultStartingBalls,
            int winScoreThreshold = DefaultWinScoreThreshold)
        {
            RuleGuard.Positive(pointsPerBasket, nameof(pointsPerBasket));
            RuleGuard.Positive(startingBalls, nameof(startingBalls));
            RuleGuard.Positive(winScoreThreshold, nameof(winScoreThreshold));
            PointsPerBasket = pointsPerBasket;
            StartingBalls = startingBalls;
            WinScoreThreshold = winScoreThreshold;
            Attempt = AttemptMatchRules.ByScore(pointsPerBasket, startingBalls, winScoreThreshold);
        }

        public int PointsPerBasket { get; }
        public int StartingBalls { get; }
        public int WinScoreThreshold { get; }
        public AttemptMatchRules Attempt { get; }
    }
}
