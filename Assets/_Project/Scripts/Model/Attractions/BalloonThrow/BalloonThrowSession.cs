using System;
using Fairground.Model.Attractions;

namespace Fairground.Model.Attractions.BalloonThrow
{
    /// <summary>
    /// Mutable match state. The rules decide win and loss.
    /// </summary>
    public sealed class BalloonThrowSession
    {
        readonly BalloonThrowRules _rules;

        public BalloonThrowSession(BalloonThrowRules rules)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        public BalloonThrowRules Rules => _rules;
        public MatchPhase Phase { get; private set; } = MatchPhase.NotStarted;
        public int Score { get; private set; }
        public int ThrowsRemaining { get; private set; }
        public int BalloonsRemaining { get; private set; }
        public int ThrowsInFlight { get; private set; }
        public int BalloonsPopped => _rules.BalloonCount - BalloonsRemaining;

        public void Start()
        {
            if (Phase != MatchPhase.NotStarted)
                return;

            BeginMatch();
        }

        public void Restart() => BeginMatch();

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

        void BeginMatch()
        {
            Score = 0;
            ThrowsRemaining = _rules.StartingThrows;
            BalloonsRemaining = _rules.BalloonCount;
            ThrowsInFlight = 0;
            Phase = MatchPhase.Playing;
        }

        bool CanRegisterThrow() => Phase == MatchPhase.Playing && ThrowsRemaining > 0;

        bool CanRegisterHit() => Phase == MatchPhase.Playing && BalloonsRemaining > 0;

        void RefreshPhase()
        {
            if (_rules.IsWin(BalloonsPopped))
                Phase = MatchPhase.Won;
            else if (_rules.IsLose(ThrowsRemaining, BalloonsRemaining, ThrowsInFlight))
                Phase = MatchPhase.Lost;
            else
                Phase = MatchPhase.Playing;
        }
    }
}
