using System;

namespace Fairground.Model.Attractions.DuckFishing
{
    /// <summary>
    /// Mutable match state. Applies rules through an end-condition Strategy.
    /// </summary>
    public sealed class DuckFishingSession
    {
        readonly DuckFishingRules _rules;
        readonly IDuckFishingEndCondition _endCondition;

        public DuckFishingSession(DuckFishingRules rules, IDuckFishingEndCondition endCondition = null)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            _endCondition = endCondition ?? new DefaultDuckFishingEndCondition();
        }

        public DuckFishingRules Rules => _rules;
        public DuckFishingPhase Phase { get; private set; } = DuckFishingPhase.NotStarted;
        public int Score { get; private set; }
        public int AttemptsRemaining { get; private set; }
        public int DucksRemaining { get; private set; }
        public int DipsInFlight { get; private set; }
        public int DucksCaught => _rules.DuckCount - DucksRemaining;

        public void Start()
        {
            Score = 0;
            AttemptsRemaining = _rules.StartingAttempts;
            DucksRemaining = _rules.DuckCount;
            DipsInFlight = 0;
            Phase = DuckFishingPhase.Playing;
        }

        public bool TryRegisterDip()
        {
            if (!CanRegisterDip())
                return false;

            AttemptsRemaining--;
            DipsInFlight++;
            RefreshPhase();
            return true;
        }

        public bool TryRegisterCatch()
        {
            if (!CanRegisterCatch())
                return false;

            DucksRemaining--;
            Score += _rules.ScoreForCatch();
            RefreshPhase();
            return true;
        }

        public bool ResolveDip()
        {
            if (DipsInFlight <= 0)
                return false;

            DipsInFlight--;
            RefreshPhase();
            return true;
        }

        bool CanRegisterDip() => Phase == DuckFishingPhase.Playing && AttemptsRemaining > 0;

        bool CanRegisterCatch() => Phase == DuckFishingPhase.Playing && DucksRemaining > 0;

        void RefreshPhase()
        {
            Phase = _endCondition.Evaluate(_rules, DucksCaught, AttemptsRemaining, DipsInFlight);
        }
    }
}
