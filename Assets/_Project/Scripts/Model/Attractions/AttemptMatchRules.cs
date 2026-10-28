using System;

namespace Fairground.Model.Attractions
{
    /// <summary>
    /// Shared win and lose policy for a booth that spends attempts and scores successes.
    /// </summary>
    public sealed class AttemptMatchRules
    {
        readonly int? _targetCount;
        readonly int _successesToWin;
        readonly int _scoreToWin;
        readonly bool _winByScore;

        AttemptMatchRules(
            int pointsPerSuccess,
            int startingAttempts,
            int? targetCount,
            int successesToWin,
            int scoreToWin,
            bool winByScore)
        {
            if (pointsPerSuccess < 0)
                throw new ArgumentOutOfRangeException(nameof(pointsPerSuccess));
            if (startingAttempts < 1)
                throw new ArgumentOutOfRangeException(nameof(startingAttempts));
            if (targetCount.HasValue && targetCount.Value < 1)
                throw new ArgumentOutOfRangeException(nameof(targetCount));
            if (!winByScore && successesToWin < 1)
                throw new ArgumentOutOfRangeException(nameof(successesToWin));
            if (winByScore && scoreToWin < 1)
                throw new ArgumentOutOfRangeException(nameof(scoreToWin));
            if (targetCount.HasValue && successesToWin > targetCount.Value)
                throw new ArgumentOutOfRangeException(nameof(successesToWin));

            PointsPerSuccess = pointsPerSuccess;
            StartingAttempts = startingAttempts;
            _targetCount = targetCount;
            _successesToWin = successesToWin;
            _scoreToWin = scoreToWin;
            _winByScore = winByScore;
        }

        public int PointsPerSuccess { get; }
        public int StartingAttempts { get; }
        public bool HasTargets => _targetCount.HasValue;
        public int TargetCount => _targetCount ?? 0;

        public static AttemptMatchRules BySuccessCount(
            int pointsPerSuccess,
            int startingAttempts,
            int targetCount,
            int successesToWin)
        {
            return new AttemptMatchRules(
                pointsPerSuccess,
                startingAttempts,
                targetCount,
                successesToWin,
                scoreToWin: 0,
                winByScore: false);
        }

        public static AttemptMatchRules ByScore(int pointsPerSuccess, int startingAttempts, int scoreToWin)
        {
            return new AttemptMatchRules(
                pointsPerSuccess,
                startingAttempts,
                targetCount: null,
                successesToWin: 0,
                scoreToWin,
                winByScore: true);
        }

        public bool IsWin(int successes, int score)
        {
            if (_winByScore)
                return score >= _scoreToWin;

            return successes >= _successesToWin;
        }

        public bool IsLose(int attemptsRemaining, int inFlight, int successes, int score)
        {
            return attemptsRemaining <= 0 && inFlight <= 0 && !IsWin(successes, score);
        }
    }
}
