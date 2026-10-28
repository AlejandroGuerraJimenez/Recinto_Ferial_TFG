using Fairground.Core.Attractions;
using Fairground.Model.Attractions;
using Fairground.Model.Attractions.BalloonThrow;
using Fairground.ViewModel.Attractions;

namespace Fairground.ViewModel.Attractions.BalloonThrow
{
    /// <summary>
    /// Publishes balloon-match state. The session owns the counters.
    /// </summary>
    public sealed class BalloonThrowViewModel : BoothMatchViewModel
    {
        readonly BalloonThrowSession _session;
        readonly IBalloonThrowStatusPresenter _statusPresenter;

        public BalloonThrowViewModel(
            BalloonThrowRules rules = null,
            IBalloonThrowStatusPresenter statusPresenter = null)
        {
            _session = new BalloonThrowSession(rules ?? new BalloonThrowRules());
            _statusPresenter = statusPresenter ?? new DefaultBalloonThrowStatusPresenter();
            PublishState();
        }

        public BalloonThrowViewModel(
            int pointsPerBalloon,
            int startingThrows,
            int balloonCount,
            IBalloonThrowStatusPresenter statusPresenter = null)
            : this(new BalloonThrowRules(pointsPerBalloon, startingThrows, balloonCount), statusPresenter)
        {
        }

        public AttractionId AttractionId => AttractionId.BalloonThrow;
        public string SceneName => AttractionCatalog.BalloonThrow.SceneName;

        public MatchPhase Phase => _session.Phase;
        public int Score => _session.Score;
        public int ThrowsRemaining => _session.ThrowsRemaining;
        public int BalloonsRemaining => _session.BalloonsRemaining;
        public int BalloonsPopped => _session.BalloonsPopped;
        public int BalloonCount => _session.Rules.BalloonCount;
        public bool IsPlaying => Phase == MatchPhase.Playing;
        public bool CanSpawnBall => IsPlaying && ThrowsRemaining > 0;
        public string StatusText => _statusPresenter.Present(Phase);
        public string[] HudLines { get; private set; }

        public override void StartGame()
        {
            _session.Start();
            Notify();
        }

        public bool NotifyBallThrown() => Apply(_session.TryRegisterThrow);

        public bool NotifyBalloonHit() => Apply(_session.TryRegisterBalloonHit);

        public bool NotifyThrowResolved() => Apply(_session.ResolveThrow);

        public override void Restart()
        {
            _session.Restart();
            Notify();
        }

        protected override void PublishState()
        {
            HudLines = new[]
            {
                $"Score: {Score}",
                $"Throws: {ThrowsRemaining}",
                $"Balloons: {BalloonsRemaining}",
                StatusText,
            };
        }
    }
}
