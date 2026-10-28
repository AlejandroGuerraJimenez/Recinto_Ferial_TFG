using System;
using Fairground.Model.Attractions;

namespace Fairground.Model.Attractions.BasketballHoop
{
    /// <summary>
    /// Mutable match state. The rules decide win and loss.
    /// </summary>
    public sealed class BasketballHoopSession
    {
        readonly BasketballHoopRules _rules;

        public BasketballHoopSession(BasketballHoopRules rules)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
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
            if (_rules.IsWin(Score))
                Phase = MatchPhase.Won;
            else if (_rules.IsLose(BallsRemaining, BallsInFlight, Score))
                Phase = MatchPhase.Lost;
            else
                Phase = MatchPhase.Playing;
        }
    }
}
