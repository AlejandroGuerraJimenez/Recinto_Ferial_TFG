using System;

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
        public PunchingBallPhase Phase { get; private set; } = PunchingBallPhase.NotStarted;
        public int Score { get; private set; }
        public int BestScore { get; private set; }
        public int LastPunchPower { get; private set; }
        public int PunchesRemaining { get; private set; }

        public void Start()
        {
            Score = 0;
            BestScore = 0;
            LastPunchPower = 0;
            PunchesRemaining = _rules.StartingPunches;
            Phase = PunchingBallPhase.Playing;
        }

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

        bool CanRegisterPunch() => Phase == PunchingBallPhase.Playing && PunchesRemaining > 0;

        void RefreshPhase()
        {
            Phase = _endCondition.Evaluate(_rules, BestScore, PunchesRemaining);
        }
    }
}
