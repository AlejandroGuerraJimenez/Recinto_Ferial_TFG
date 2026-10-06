using System;

namespace Fairground.Model.Attractions.BalloonThrow
{
    /// <summary>
    /// Immutable scoring and win/lose policy (Strategy data for the session).
    /// </summary>
    public sealed class BalloonThrowRules
    {
        public BalloonThrowRules(int pointsPerBalloon = 10, int startingThrows = 8, int balloonCount = 6)
        {
            GuardNonNegative(pointsPerBalloon, nameof(pointsPerBalloon));
            GuardPositive(startingThrows, nameof(startingThrows));
            GuardPositive(balloonCount, nameof(balloonCount));
            PointsPerBalloon = pointsPerBalloon;
            StartingThrows = startingThrows;
            BalloonCount = balloonCount;
        }

        public int PointsPerBalloon { get; }
        public int StartingThrows { get; }
        public int BalloonCount { get; }

        public int ScoreForHit() => PointsPerBalloon;

        public bool IsWin(int balloonsPopped) => balloonsPopped >= BalloonCount;

        public bool IsLose(int throwsRemaining, int balloonsRemaining)
        {
            return throwsRemaining <= 0 && balloonsRemaining > 0;
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
