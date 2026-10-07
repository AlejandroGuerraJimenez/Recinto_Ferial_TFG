using System;

namespace Fairground.Model.Attractions.BalloonThrow
{
    /// <summary>
    /// Mutable match state. Applies rules through an end-condition Strategy.
    /// </summary>
    public sealed class BalloonThrowSession
    {
        readonly BalloonThrowRules _rules;
        readonly IBalloonThrowEndCondition _endCondition;

        public BalloonThrowSession(BalloonThrowRules rules, IBalloonThrowEndCondition endCondition = null)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            _endCondition = endCondition ?? new DefaultBalloonThrowEndCondition();
        }

        public BalloonThrowRules Rules => _rules;
        public BalloonThrowPhase Phase { get; private set; } = BalloonThrowPhase.NotStarted;
        public int Score { get; private set; }
        public int ThrowsRemaining { get; private set; }
        public int BalloonsRemaining { get; private set; }
        public int ThrowsInFlight { get; private set; }
        public int BalloonsPopped => _rules.BalloonCount - BalloonsRemaining;

        public void Start()
        {
            Score = 0;
            ThrowsRemaining = _rules.StartingThrows;
            BalloonsRemaining = _rules.BalloonCount;
            ThrowsInFlight = 0;
            Phase = BalloonThrowPhase.Playing;
        }

        public bool TryRegisterThrow()
        {
            if (!CanRegisterThrow())
                return false;

            ThrowsRemaining--;
            ThrowsInFlight++;
            RefreshPhase();
            return true;
        }

        public bool TryRegisterBalloonHit()
        {
            if (!CanRegisterHit())
                return false;

            BalloonsRemaining--;
            Score += _rules.ScoreForHit();
            RefreshPhase();
            return true;
        }

        public bool ResolveThrow()
        {
            if (ThrowsInFlight <= 0)
                return false;

            ThrowsInFlight--;
            RefreshPhase();
            return true;
        }

        bool CanRegisterThrow() => Phase == BalloonThrowPhase.Playing && ThrowsRemaining > 0;

        bool CanRegisterHit() => Phase == BalloonThrowPhase.Playing && BalloonsRemaining > 0;

        void RefreshPhase()
        {
            Phase = _endCondition.Evaluate(
                _rules,
                BalloonsPopped,
                ThrowsRemaining,
                BalloonsRemaining,
                ThrowsInFlight);
        }
    }
}
