using System;
using Fairground.Core.Attractions;

namespace Fairground.Model.Attractions.PunchingBall
{
    /// <summary>
    /// Immutable scoring and win/lose policy.
    /// </summary>
    public sealed class PunchingBallRules
    {
        public const int DefaultStartingPunches = PunchingBallBalance.StartingPunches;
        public const int DefaultWinScoreThreshold = PunchingBallBalance.WinScoreThreshold;
        public const int DefaultMaxScore = PunchingBallBalance.MaxScore;
        public const float DefaultMaxImpactSpeed = PunchingBallBalance.MaxImpactSpeed;

        public PunchingBallRules(
            int startingPunches = DefaultStartingPunches,
            int winScoreThreshold = DefaultWinScoreThreshold,
            int maxScore = DefaultMaxScore,
            float maxImpactSpeed = DefaultMaxImpactSpeed)
        {
            RuleGuard.Positive(startingPunches, nameof(startingPunches));
            RuleGuard.Positive(winScoreThreshold, nameof(winScoreThreshold));
            RuleGuard.Positive(maxScore, nameof(maxScore));
            RuleGuard.Positive(maxImpactSpeed, nameof(maxImpactSpeed));

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

        public int ScoreForSpeed(float impactSpeed)
        {
            if (impactSpeed <= 0f)
                return 0;

            float t = impactSpeed / MaxImpactSpeed;
            if (t > 1f)
                t = 1f;

            return (int)(t * MaxScore + 0.5f);
        }

        public bool IsWin(int bestScore) => bestScore >= WinScoreThreshold;

        public bool IsLose(int punchesRemaining, int bestScore)
        {
            return punchesRemaining <= 0 && bestScore < WinScoreThreshold;
        }
    }
}
