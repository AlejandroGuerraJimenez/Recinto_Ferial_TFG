using System;
using Fairground.Core.Attractions;
using Fairground.Model.Attractions;

namespace Fairground.Model.Attractions.DuckFishing
{
    /// <summary>
    /// Immutable scoring and win/lose policy.
    /// </summary>
    public sealed class DuckFishingRules
    {
        public const int DefaultPointsPerDuck = DuckFishingBalance.PointsPerDuck;
        public const int DefaultStartingAttempts = DuckFishingBalance.StartingAttempts;
        public const int DefaultDuckCount = DuckFishingBalance.DuckCount;
        public const int DefaultDucksToWin = DuckFishingBalance.DucksToWin;

        public DuckFishingRules(
            int pointsPerDuck = DefaultPointsPerDuck,
            int startingAttempts = DefaultStartingAttempts,
            int duckCount = DefaultDuckCount,
            int ducksToWin = DefaultDucksToWin)
        {
            RuleGuard.NonNegative(pointsPerDuck, nameof(pointsPerDuck));
            RuleGuard.Positive(startingAttempts, nameof(startingAttempts));
            RuleGuard.Positive(duckCount, nameof(duckCount));
            RuleGuard.Positive(ducksToWin, nameof(ducksToWin));

            if (ducksToWin > duckCount)
                throw new ArgumentOutOfRangeException(nameof(ducksToWin));

            PointsPerDuck = pointsPerDuck;
            StartingAttempts = startingAttempts;
            DuckCount = duckCount;
            DucksToWin = ducksToWin;
            Attempt = AttemptMatchRules.BySuccessCount(
                pointsPerDuck,
                startingAttempts,
                duckCount,
                ducksToWin);
        }

        public int PointsPerDuck { get; }
        public int StartingAttempts { get; }
        public int DuckCount { get; }
        public int DucksToWin { get; }
        public AttemptMatchRules Attempt { get; }
    }
}
