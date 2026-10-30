using Fairground.Core.Attractions;

namespace Fairground.Model.Attractions.DuckFishing
{
    /// <summary>
    /// Round length and how many ducks are in the pond. The score is the ducks pulled out.
    /// </summary>
    public sealed class DuckFishingRules
    {
        public const int DefaultRoundSeconds = DuckFishingBalance.RoundSeconds;
        public const int DefaultDuckCount = DuckFishingBalance.DuckCount;

        public DuckFishingRules(
            int roundSeconds = DefaultRoundSeconds,
            int duckCount = DefaultDuckCount)
        {
            RuleGuard.Positive(roundSeconds, nameof(roundSeconds));
            RuleGuard.Positive(duckCount, nameof(duckCount));
            RoundSeconds = roundSeconds;
            DuckCount = duckCount;
        }

        public int RoundSeconds { get; }
        public int DuckCount { get; }
    }
}
