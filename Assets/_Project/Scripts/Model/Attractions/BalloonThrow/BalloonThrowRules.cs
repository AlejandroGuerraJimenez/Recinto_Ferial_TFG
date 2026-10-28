using System;

namespace Fairground.Model.Attractions.BalloonThrow
{
    /// <summary>
    /// Immutable scoring and win/lose policy.
    /// </summary>
    public sealed class BalloonThrowRules
    {
        public const int DefaultPointsPerBalloon = 10;
        public const int DefaultStartingThrows = 8;
        public const int DefaultBalloonCount = 6;

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
        }

        public int PointsPerBalloon { get; }
        public int StartingThrows { get; }
        public int BalloonCount { get; }

        public int ScoreForHit() => PointsPerBalloon;

        public bool IsWin(int balloonsPopped) => balloonsPopped >= BalloonCount;

        public bool IsLose(int throwsRemaining, int balloonsRemaining, int throwsInFlight)
        {
            return throwsRemaining <= 0 && balloonsRemaining > 0 && throwsInFlight <= 0;
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
