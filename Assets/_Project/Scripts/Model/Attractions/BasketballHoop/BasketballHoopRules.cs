using Fairground.Core.Attractions;

namespace Fairground.Model.Attractions.BasketballHoop
{
    /// <summary>
    /// Round length and points per basket. Balls keep coming until the clock ends.
    /// </summary>
    public sealed class BasketballHoopRules
    {
        public const int DefaultPointsPerBasket = BasketballHoopBalance.PointsPerBasket;
        public const int DefaultRoundSeconds = BasketballHoopBalance.RoundSeconds;

        public BasketballHoopRules(
            int pointsPerBasket = DefaultPointsPerBasket,
            int roundSeconds = DefaultRoundSeconds)
        {
            RuleGuard.Positive(pointsPerBasket, nameof(pointsPerBasket));
            RuleGuard.Positive(roundSeconds, nameof(roundSeconds));
            PointsPerBasket = pointsPerBasket;
            RoundSeconds = roundSeconds;
        }

        public int PointsPerBasket { get; }
        public int RoundSeconds { get; }
    }
}
