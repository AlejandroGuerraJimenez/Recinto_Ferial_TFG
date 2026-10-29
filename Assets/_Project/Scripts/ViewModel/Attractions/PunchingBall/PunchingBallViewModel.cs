using System;
using Fairground.Core.Attractions;
using Fairground.Model.Attractions;
using Fairground.Model.Attractions.PunchingBall;
using Fairground.ViewModel.Attractions;

namespace Fairground.ViewModel.Attractions.PunchingBall
{
    /// <summary>
    /// Publishes punch-match state. The match owns the counters.
    /// </summary>
    public sealed class PunchingBallViewModel : BoothMatchViewModel
    {
        readonly PunchingBallMatch _match;

        public PunchingBallViewModel()
            : this(new PunchingBallRules())
        {
        }

        public PunchingBallViewModel(PunchingBallRules rules)
        {
            _match = new PunchingBallMatch(rules ?? throw new ArgumentNullException(nameof(rules)));
        }

        public PunchingBallViewModel(
            int startingPunches,
            int winScoreThreshold,
            int maxScore,
            float maxImpactSpeed)
            : this(new PunchingBallRules(startingPunches, winScoreThreshold, maxScore, maxImpactSpeed))
        {
        }

        public AttractionId AttractionId => AttractionId.PunchingBall;

        public MatchPhase Phase => _match.Phase;
        public int BestScore => _match.BestScore;
        public int LastPunchPower => _match.LastPunchPower;
        public int PunchesRemaining => _match.PunchesRemaining;
        public int WinScoreThreshold => _match.Rules.WinScoreThreshold;
        public int MaxScore => _match.Rules.MaxScore;
        public bool IsPlaying => Phase == MatchPhase.Playing;
        public bool CanPunch => IsPlaying && PunchesRemaining > 0;

        public override void StartMatch()
        {
            _match.Start();
            Notify();
        }

        public bool TryRegisterPunch(float impactSpeed) => Apply(() => _match.TryRegisterPunch(impactSpeed));

        public override void Restart()
        {
            _match.Restart();
            Notify();
        }
    }
}
