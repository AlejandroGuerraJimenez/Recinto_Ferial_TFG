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
        readonly IPunchingBallStatusPresenter _statusPresenter;

        public PunchingBallViewModel(
            PunchingBallRules rules = null,
            IPunchingBallStatusPresenter statusPresenter = null)
        {
            _session = new PunchingBallSession(rules ?? new PunchingBallRules());
            _statusPresenter = statusPresenter ?? new DefaultPunchingBallStatusPresenter();
            PublishState();
        }

        public PunchingBallViewModel(
            int startingPunches,
            int winScoreThreshold,
            int maxScore,
            float maxImpactSpeed,
            IPunchingBallStatusPresenter statusPresenter = null)
            : this(
                new PunchingBallRules(startingPunches, winScoreThreshold, maxScore, maxImpactSpeed),
                statusPresenter)
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
        public string StatusText => _statusPresenter.Present(Phase, WinScoreThreshold);
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
    }
}
