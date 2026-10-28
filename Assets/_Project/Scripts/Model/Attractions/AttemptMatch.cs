using System;

namespace Fairground.Model.Attractions
{
    /// <summary>
    /// Mutable attempt match. Spending an attempt, scoring a success, and resolving one in flight share one phase.
    /// </summary>
    public sealed class AttemptMatch
    {
        readonly AttemptMatchRules _rules;

        public AttemptMatch(AttemptMatchRules rules)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        public MatchPhase Phase { get; private set; } = MatchPhase.NotStarted;
        public int Score { get; private set; }
        public int AttemptsRemaining { get; private set; }
        public int Successes { get; private set; }
        public int InFlight { get; private set; }
        public int TargetsRemaining => _rules.HasTargets ? _rules.TargetCount - Successes : 0;

        public void Start()
        {
            if (Phase != MatchPhase.NotStarted)
                return;

            Begin();
        }

        public void Restart() => Begin();

        public bool TrySpendAttempt()
        {
            if (Phase != MatchPhase.Playing || AttemptsRemaining <= 0)
                return false;

            AttemptsRemaining--;
            InFlight++;
            Refresh();
            return true;
        }

        public bool TryRegisterSuccess()
        {
            if (Phase != MatchPhase.Playing)
                return false;

            if (_rules.HasTargets && Successes >= _rules.TargetCount)
                return false;

            Successes++;
            Score += _rules.PointsPerSuccess;
            Refresh();
            return true;
        }

        public bool ResolveAttempt()
        {
            if (InFlight <= 0)
                return false;

            InFlight--;
            Refresh();
            return true;
        }

        void Begin()
        {
            Score = 0;
            Successes = 0;
            AttemptsRemaining = _rules.StartingAttempts;
            InFlight = 0;
            Phase = MatchPhase.Playing;
        }

        void Refresh()
        {
            if (_rules.IsWin(Successes, Score))
                Phase = MatchPhase.Won;
            else if (_rules.IsLose(AttemptsRemaining, InFlight, Successes, Score))
                Phase = MatchPhase.Lost;
            else
                Phase = MatchPhase.Playing;
        }
    }
}
