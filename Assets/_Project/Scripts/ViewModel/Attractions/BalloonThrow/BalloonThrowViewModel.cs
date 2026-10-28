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

        public MatchPhase Phase => _match.Phase;
        public int Score => _match.Score;
        public int ThrowsRemaining => _match.AttemptsRemaining;
        public int BalloonsRemaining => _match.TargetsRemaining;
        public int BalloonsPopped => _match.Successes;
        public int BalloonCount => _rules.BalloonCount;
        public bool IsPlaying => Phase == MatchPhase.Playing;
        public bool CanSpawnBall => IsPlaying && ThrowsRemaining > 0;
        public string StatusText => Describe(Phase);
        public override string[] HudLines { get; protected set; }

        public override void StartGame()
        {
            _match.Start();
            Notify();
        }

        public bool TryRegisterThrow() => Apply(_match.TrySpendAttempt);

        public bool TryRegisterHit() => Apply(_match.TryRegisterSuccess);

        public bool TryResolveThrow() => Apply(_match.ResolveAttempt);

        public override void Restart()
        {
            _match.Restart();
            Notify();
        }

        protected override void PublishState()
        {
            HudLines = new[]
            {
                $"Puntos: {Score}",
                $"Lanzamientos: {ThrowsRemaining}",
                $"Globos: {BalloonsRemaining}",
                StatusText,
            };
        }

        static string Describe(MatchPhase phase)
        {
            switch (phase)
            {
                case MatchPhase.Playing: return "¡Explota los globos!";
                case MatchPhase.Won: return "¡Ganaste!";
                case MatchPhase.Lost: return "¡Sin lanzamientos!";
                default: return "Listo";
            }
        }
    }
}
