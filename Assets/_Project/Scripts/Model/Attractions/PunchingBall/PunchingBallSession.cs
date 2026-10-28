using System;
using Fairground.Model.Attractions;

namespace Fairground.Model.Attractions.PunchingBall
{
    /// <summary>
    /// Mutable match state. Applies rules through an end-condition Strategy.
    /// </summary>
    public sealed class PunchingBallSession
    {
        readonly PunchingBallRules _rules;
        readonly IPunchingBallEndCondition _endCondition;

        public PunchingBallSession(PunchingBallRules rules, IPunchingBallEndCondition endCondition = null)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            _endCondition = endCondition ?? new DefaultPunchingBallEndCondition();
        }

        public PunchingBallRules Rules => _rules;
        public MatchPhase Phase { get; private set; } = MatchPhase.NotStarted;
        public int Score { get; private set; }
        public int BestScore { get; private set; }
        public int LastPunchPower { get; private set; }
        public int PunchesRemaining { get; private set; }

        public void Start()
        {
            if (Phase != MatchPhase.NotStarted)
                return;

            BeginMatch();
        }

        public void Restart() => BeginMatch();

        public bool TryRegisterPunch(float impactMagnitude)
        {
            if (!CanRegisterPunch())
                return false;

            PunchesRemaining--;
            LastPunchPower = _rules.ScoreForImpact(impactMagnitude);
            if (LastPunchPower > BestScore)
                BestScore = LastPunchPower;

            Score = BestScore;
            RefreshPhase();
            return true;
        }

        void BeginMatch()
        {
            Score = 0;
            BestScore = 0;
            LastPunchPower = 0;
            PunchesRemaining = _rules.StartingPunches;
            Phase = MatchPhase.Playing;
        }

        bool CanRegisterPunch() => Phase == MatchPhase.Playing && PunchesRemaining > 0;

        void RefreshPhase()
        {
            Phase = _endCondition.Evaluate(_rules, BestScore, PunchesRemaining);
        }
    }
}
