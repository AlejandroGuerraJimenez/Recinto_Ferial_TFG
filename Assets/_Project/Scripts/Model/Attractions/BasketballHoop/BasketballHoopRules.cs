using System;

namespace Fairground.Model.Attractions.BasketballHoop
{
    /// <summary>
    /// Immutable scoring and win/lose policy (Strategy data for the session).
    /// </summary>
    public sealed class BasketballHoopRules
    {
        public BasketballHoopRules(int pointsPerBasket = 2, int startingBalls = 10, int winScoreThreshold = 5)
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
