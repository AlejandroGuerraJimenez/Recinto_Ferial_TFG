using Fairground.Core.Attractions;
using Fairground.Model.Attractions;
using Fairground.Model.Attractions.PunchingBall;
using Fairground.ViewModel.Attractions;

namespace Fairground.ViewModel.Attractions.PunchingBall
{
    /// <summary>
    /// Publishes punch-match state. The session owns the counters.
    /// </summary>
    public sealed class PunchingBallViewModel : BoothMatchViewModel
    {
        readonly PunchingBallSession _session;

        public PunchingBallViewModel(PunchingBallRules rules = null)
        {
            _session = new PunchingBallSession(rules ?? new PunchingBallRules());
            PublishState();
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
        public string SceneName => AttractionCatalog.PunchingBall.SceneName;

        public MatchPhase Phase => _session.Phase;
        public int Score => _session.Score;
        public int BestScore => _session.BestScore;
        public int LastPunchPower => _session.LastPunchPower;
        public int PunchesRemaining => _session.PunchesRemaining;
        public int WinScoreThreshold => _session.Rules.WinScoreThreshold;
        public int MaxScore => _session.Rules.MaxScore;
        public bool IsPlaying => Phase == MatchPhase.Playing;
        public bool CanPunch => IsPlaying && PunchesRemaining > 0;
        public string StatusText => Describe(Phase);
        public string[] HudLines { get; private set; }

        public override void StartGame()
        {
            _session.Start();
            Notify();
        }

        public bool NotifyPunch(float impactMagnitude) => Apply(() => _session.TryRegisterPunch(impactMagnitude));

        public override void Restart()
        {
            _session.Restart();
            Notify();
        }

        protected override void PublishState()
        {
            HudLines = new[]
            {
                $"Best: {BestScore}",
                $"Punches: {PunchesRemaining}",
                $"Last: {LastPunchPower}",
                StatusText,
            };
        }

        string Describe(MatchPhase phase)
        {
            switch (phase)
            {
                case MatchPhase.Playing: return $"Hit {WinScoreThreshold}+ to win!";
                case MatchPhase.Won: return "Strongman!";
                case MatchPhase.Lost: return "Too weak!";
                default: return "Ready";
            }
        }
    }
}
