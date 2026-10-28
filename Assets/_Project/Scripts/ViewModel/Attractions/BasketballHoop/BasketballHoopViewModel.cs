using Fairground.Core.Attractions;
using Fairground.Model.Attractions;
using Fairground.Model.Attractions.BasketballHoop;
using Fairground.ViewModel.Attractions;

namespace Fairground.ViewModel.Attractions.BasketballHoop
{
    /// <summary>
    /// Publishes hoop-match state. The match owns the counters.
    /// </summary>
    public sealed class BasketballHoopViewModel : BoothMatchViewModel
    {
        readonly AttemptMatch _match;

        public BasketballHoopViewModel(BasketballHoopRules rules = null)
        {
            _match = new AttemptMatch((rules ?? new BasketballHoopRules()).Attempt);
            PublishState();
        }

        public BasketballHoopViewModel(int pointsPerBasket, int startingBalls, int winScoreThreshold)
            : this(new BasketballHoopRules(pointsPerBasket, startingBalls, winScoreThreshold))
        {
        }

        public AttractionId AttractionId => AttractionId.BasketballHoop;
        public string SceneName => AttractionCatalog.BasketballHoop.SceneName;

        public MatchPhase Phase => _match.Phase;
        public int Score => _match.Score;
        public int BallsRemaining => _match.AttemptsRemaining;
        public bool IsPlaying => Phase == MatchPhase.Playing;
        public bool CanThrow => IsPlaying && BallsRemaining > 0;
        public string StatusText => Describe(Phase);
        public string[] HudLines { get; private set; }

        public override void StartGame()
        {
            _match.Start();
            Notify();
        }

        public bool NotifyBallThrown() => Apply(_match.TrySpendAttempt);

        public bool NotifyBasket() => Apply(_match.TryRegisterSuccess);

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
                $"Balls: {BallsRemaining}",
                StatusText,
            };
        }

        static string Describe(MatchPhase phase)
        {
            switch (phase)
            {
                case MatchPhase.Playing: return "Shoot the hoop!";
                case MatchPhase.Won: return "You win!";
                case MatchPhase.Lost: return "Out of balls!";
                default: return "Ready";
            }
        }
    }
}
