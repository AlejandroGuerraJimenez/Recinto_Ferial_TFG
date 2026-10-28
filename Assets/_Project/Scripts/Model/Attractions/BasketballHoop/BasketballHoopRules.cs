using System;

namespace Fairground.Model.Attractions.BasketballHoop
{
    /// <summary>
    /// Immutable scoring and win/lose policy.
    /// </summary>
    public sealed class BasketballHoopRules
    {
        public const int DefaultPointsPerBasket = 2;
        public const int DefaultStartingBalls = 10;
        public const int DefaultWinScoreThreshold = 5;

        public BasketballHoopRules(
            int pointsPerBasket = DefaultPointsPerBasket,
            int startingBalls = DefaultStartingBalls,
            int winScoreThreshold = DefaultWinScoreThreshold)
        {
            GuardPositive(pointsPerBasket, nameof(pointsPerBasket));
            GuardPositive(startingBalls, nameof(startingBalls));
            GuardPositive(winScoreThreshold, nameof(winScoreThreshold));
            PointsPerBasket = pointsPerBasket;
            StartingBalls = startingBalls;
            WinScoreThreshold = winScoreThreshold;
        }

        public int PointsPerBasket { get; }
        public int StartingBalls { get; }
        public int WinScoreThreshold { get; }

        public int ScoreForBasket() => PointsPerBasket;

        public bool IsWin(int score) => score >= WinScoreThreshold;

        public bool IsLose(int ballsRemaining, int ballsInFlight, int score)
        {
            return ballsRemaining <= 0 && ballsInFlight <= 0 && score < WinScoreThreshold;
        }

        static void GuardPositive(int value, string name)
        {
            if (value < 1)
                throw new ArgumentOutOfRangeException(name);
        }
    }
}
