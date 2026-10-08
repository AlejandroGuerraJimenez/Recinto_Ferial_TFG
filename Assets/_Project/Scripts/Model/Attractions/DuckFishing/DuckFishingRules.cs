using System;

namespace Fairground.Model.Attractions.DuckFishing
{
    /// <summary>
    /// Immutable scoring and win/lose policy (Strategy data for the session).
    /// </summary>
    public sealed class DuckFishingRules
    {
        public DuckFishingRules(
            int pointsPerDuck = 10,
            int startingAttempts = 8,
            int duckCount = 6,
            int ducksToWin = 4)
        {
            GuardNonNegative(pointsPerDuck, nameof(pointsPerDuck));
            GuardPositive(startingAttempts, nameof(startingAttempts));
            GuardPositive(duckCount, nameof(duckCount));
            GuardPositive(ducksToWin, nameof(ducksToWin));

            if (ducksToWin > duckCount)
                throw new ArgumentOutOfRangeException(nameof(ducksToWin));

            PointsPerDuck = pointsPerDuck;
            StartingAttempts = startingAttempts;
            DuckCount = duckCount;
            DucksToWin = ducksToWin;
        }

        public int PointsPerDuck { get; }
        public int StartingAttempts { get; }
        public int DuckCount { get; }
        public int DucksToWin { get; }

        public int ScoreForCatch() => PointsPerDuck;

        public bool IsWin(int ducksCaught) => ducksCaught >= DucksToWin;

        public bool IsLose(int attemptsRemaining, int ducksCaught, int dipsInFlight)
        {
            return attemptsRemaining <= 0 && ducksCaught < DucksToWin && dipsInFlight <= 0;
        }

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
