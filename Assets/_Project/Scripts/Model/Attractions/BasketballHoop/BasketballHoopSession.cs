using System;
using Fairground.Model.Attractions;

namespace Fairground.Model.Attractions.BasketballHoop
{
    /// <summary>
    /// Mutable match state. Applies rules through an end-condition Strategy.
    /// </summary>
    public sealed class BasketballHoopSession
    {
        readonly BasketballHoopRules _rules;
        readonly IBasketballHoopEndCondition _endCondition;

        public BasketballHoopSession(BasketballHoopRules rules, IBasketballHoopEndCondition endCondition = null)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            _endCondition = endCondition ?? new DefaultBasketballHoopEndCondition();
        }

        public BasketballHoopRules Rules => _rules;
        public MatchPhase Phase { get; private set; } = MatchPhase.NotStarted;
        public int Score { get; private set; }
        public int BallsRemaining { get; private set; }
        public int BallsInFlight { get; private set; }

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

            BallsRemaining--;
            BallsInFlight++;
            RefreshPhase();
            return true;
        }

        public bool TryRegisterBasket()
        {
            if (!CanRegisterBasket())
                return false;

            Score += _rules.ScoreForBasket();
            RefreshPhase();
            return true;
        }

        public bool ResolveThrow()
        {
            if (BallsInFlight <= 0)
                return false;

            BallsInFlight--;
            RefreshPhase();
            return true;
        }

        void BeginMatch()
        {
            Score = 0;
            BallsRemaining = _rules.StartingBalls;
            BallsInFlight = 0;
            Phase = MatchPhase.Playing;
        }

        bool CanRegisterThrow() => Phase == MatchPhase.Playing && BallsRemaining > 0;

        bool CanRegisterBasket() => Phase == MatchPhase.Playing;

        void RefreshPhase()
        {
            Phase = _endCondition.Evaluate(_rules, Score, BallsRemaining, BallsInFlight);
        }
    }
}
