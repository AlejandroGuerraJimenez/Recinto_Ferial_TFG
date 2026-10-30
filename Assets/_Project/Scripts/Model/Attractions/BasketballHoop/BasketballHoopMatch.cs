using System;

namespace Fairground.Model.Attractions.BasketballHoop
{
    /// <summary>
    /// Timed shooting round. Baskets add points until the countdown reaches zero.
    /// </summary>
    public sealed class BasketballHoopMatch
    {
        readonly BasketballHoopRules _rules;

        public BasketballHoopMatch(BasketballHoopRules rules)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            SecondsRemaining = _rules.RoundSeconds;
        }

        public MatchPhase Phase { get; private set; } = MatchPhase.NotStarted;
        public int Score { get; private set; }
        public float SecondsRemaining { get; private set; }
        public int PointsPerBasket => _rules.PointsPerBasket;

        public void Start()
        {
            if (Phase != MatchPhase.NotStarted)
                return;

            Begin();
        }

        public void Restart() => Begin();

        public bool TryRegisterBasket()
        {
            if (Phase != MatchPhase.Playing)
                return false;

            Score += _rules.PointsPerBasket;
            return true;
        }

        public void Tick(float deltaSeconds)
        {
            if (Phase != MatchPhase.Playing || deltaSeconds <= 0f)
                return;

            SecondsRemaining -= deltaSeconds;
            if (SecondsRemaining > 0f)
                return;

            SecondsRemaining = 0f;
            Finish();
        }

        void Begin()
        {
            Score = 0;
            SecondsRemaining = _rules.RoundSeconds;
            Phase = MatchPhase.Playing;
        }

        void Finish()
        {
            if (Phase != MatchPhase.Playing)
                return;

            Phase = Score > 0 ? MatchPhase.Won : MatchPhase.Lost;
        }
    }
}
