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

        public BalloonThrowViewModel(BalloonThrowRules rules = null)
        {
            _session = new BalloonThrowSession(rules ?? new BalloonThrowRules());
            PublishState();
        }

        public BalloonThrowViewModel(int pointsPerBalloon, int startingThrows, int balloonCount)
            : this(new BalloonThrowRules(pointsPerBalloon, startingThrows, balloonCount))
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
        public string StatusText => Describe(Phase);
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

        static string Describe(MatchPhase phase)
        {
            switch (phase)
            {
                case MatchPhase.Playing: return "Pop the balloons!";
                case MatchPhase.Won: return "You win!";
                case MatchPhase.Lost: return "Out of throws!";
                default: return "Ready";
            }
        }
    }
}
