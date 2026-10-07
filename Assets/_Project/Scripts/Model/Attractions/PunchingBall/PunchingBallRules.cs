using System;

namespace Fairground.Model.Attractions.PunchingBall
{
    /// <summary>
    /// Immutable scoring and win/lose policy (Strategy data for the session).
    /// </summary>
    public sealed class PunchingBallRules
    {
        public PunchingBallRules(
            int startingPunches = 5,
            int winScoreThreshold = 700,
            int maxScore = 999,
            float maxImpactSpeed = 8f)
        {
            GuardPositive(startingPunches, nameof(startingPunches));
            GuardPositive(winScoreThreshold, nameof(winScoreThreshold));
            GuardPositive(maxScore, nameof(maxScore));
            GuardPositive(maxImpactSpeed, nameof(maxImpactSpeed));

            if (winScoreThreshold > maxScore)
                throw new ArgumentOutOfRangeException(nameof(winScoreThreshold));

            StartingPunches = startingPunches;
            WinScoreThreshold = winScoreThreshold;
            MaxScore = maxScore;
            MaxImpactSpeed = maxImpactSpeed;
        }

        public int StartingPunches { get; }
        public int WinScoreThreshold { get; }
        public int MaxScore { get; }
        public float MaxImpactSpeed { get; }

        public int ScoreForImpact(float impactMagnitude)
        {
            if (impactMagnitude <= 0f)
                return 0;

            float t = impactMagnitude / MaxImpactSpeed;
            if (t > 1f)
                t = 1f;

            return (int)(t * MaxScore + 0.5f);
        }

        public bool IsWin(int bestScore) => bestScore >= WinScoreThreshold;

        public bool IsLose(int punchesRemaining, int bestScore)
        {
            return punchesRemaining <= 0 && bestScore < WinScoreThreshold;
        }

        static void GuardPositive(int value, string name)
        {
            if (value < 1)
                throw new ArgumentOutOfRangeException(name);
        }

        static void GuardPositive(float value, string name)
        {
            if (value <= 0f)
                throw new ArgumentOutOfRangeException(name);
        }
    }
}
