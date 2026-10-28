using Fairground.Core.Attractions;
using Fairground.Model.Attractions;
using Fairground.Model.Attractions.BalloonThrow;
using Fairground.ViewModel.Attractions;

namespace Fairground.ViewModel.Attractions.BalloonThrow
{
    /// <summary>
    /// Publishes balloon-match state. The match owns the counters.
    /// </summary>
    public sealed class BalloonThrowViewModel : BoothMatchViewModel
    {
        readonly BalloonThrowRules _rules;
        readonly AttemptMatch _match;

        public BalloonThrowViewModel(BalloonThrowRules rules = null)
        {
            _rules = rules ?? new BalloonThrowRules();
            _match = new AttemptMatch(_rules.Attempt);
            PublishState();
        }

        public BalloonThrowViewModel(int pointsPerBalloon, int startingThrows, int balloonCount)
            : this(new BalloonThrowRules(pointsPerBalloon, startingThrows, balloonCount))
        {
        }

        public AttractionId AttractionId => AttractionId.BalloonThrow;
        public string SceneName => AttractionCatalog.BalloonThrow.SceneName;

        public MatchPhase Phase => _match.Phase;
        public int Score => _match.Score;
        public int ThrowsRemaining => _match.AttemptsRemaining;
        public int BalloonsRemaining => _match.TargetsRemaining;
        public int BalloonsPopped => _match.Successes;
        public int BalloonCount => _rules.BalloonCount;
        public bool IsPlaying => Phase == MatchPhase.Playing;
        public bool CanSpawnBall => IsPlaying && ThrowsRemaining > 0;
        public string StatusText => Describe(Phase);
        public string[] HudLines { get; private set; }

        public override void StartGame()
        {
            _match.Start();
            Notify();
        }

        public bool NotifyBallThrown() => Apply(_match.TrySpendAttempt);

        public bool NotifyBalloonHit() => Apply(_match.TryRegisterSuccess);

        public bool NotifyThrowResolved() => Apply(_match.ResolveAttempt);

        public override void Restart()
        {
            _match.Restart();
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
