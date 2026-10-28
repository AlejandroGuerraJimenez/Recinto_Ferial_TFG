using System;
using Fairground.Model.Attractions;

namespace Fairground.Model.Attractions.PunchingBall
{
    /// <summary>
    /// Mutable match state. The rules decide win and loss.
    /// </summary>
    public sealed class PunchingBallMatch
    {
        readonly PunchingBallRules _rules;

        public PunchingBallMatch(PunchingBallRules rules)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        public PunchingBallRules Rules => _rules;
        public MatchPhase Phase { get; private set; } = MatchPhase.NotStarted;
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

        public bool TryRegisterPunch(float impactSpeed)
        {
            if (!CanRegisterPunch())
                return false;

            PunchesRemaining--;
            LastPunchPower = _rules.ScoreForSpeed(impactSpeed);
            if (LastPunchPower > BestScore)
                BestScore = LastPunchPower;

            RefreshPhase();
            return true;
        }

        void BeginMatch()
        {
            BestScore = 0;
            LastPunchPower = 0;
            PunchesRemaining = _rules.StartingPunches;
            Phase = MatchPhase.Playing;
        }

        bool CanRegisterPunch() => Phase == MatchPhase.Playing && PunchesRemaining > 0;

        void RefreshPhase()
        {
            if (_rules.IsWin(BestScore))
                Phase = MatchPhase.Won;
            else if (_rules.IsLose(PunchesRemaining, BestScore))
                Phase = MatchPhase.Lost;
            else
                Phase = MatchPhase.Playing;
        }
    }
}
