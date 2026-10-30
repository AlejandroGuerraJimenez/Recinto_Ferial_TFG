using System;

namespace Fairground.Model.Attractions.DuckFishing
{
    /// <summary>
    /// Timed round. Each duck pulled out is one point. The score locks when the time ends.
    /// </summary>
    public sealed class DuckFishingMatch
    {
        readonly DuckFishingRules _rules;

        public DuckFishingMatch(DuckFishingRules rules)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            SecondsRemaining = _rules.RoundSeconds;
        }

        public MatchPhase Phase { get; private set; } = MatchPhase.NotStarted;
        public int Score => DucksCaught;
        public int DucksCaught { get; private set; }
        public int DuckCount => _rules.DuckCount;
        public float SecondsRemaining { get; private set; }

        public void Start()
        {
            if (Phase != MatchPhase.NotStarted)
                return;

            Begin();
        }

        public void Restart() => Begin();

        public bool TryRegisterCatch()
        {
            if (Phase != MatchPhase.Playing || DucksCaught >= _rules.DuckCount)
                return false;

            DucksCaught++;
            if (DucksCaught >= _rules.DuckCount)
                Finish();

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
            DucksCaught = 0;
            SecondsRemaining = _rules.RoundSeconds;
            Phase = MatchPhase.Playing;
        }

        void Finish()
        {
            if (Phase != MatchPhase.Playing)
                return;

            Phase = DucksCaught > 0 ? MatchPhase.Won : MatchPhase.Lost;
        }
    }
}
