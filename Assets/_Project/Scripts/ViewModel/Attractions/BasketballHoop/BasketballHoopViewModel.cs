using Fairground.Core.Attractions;
using Fairground.Model.Attractions;
using Fairground.Model.Attractions.BasketballHoop;
using Fairground.ViewModel.Attractions;

namespace Fairground.ViewModel.Attractions.BasketballHoop
{
    /// <summary>
    /// Publishes hoop-match state. The session owns the counters.
    /// </summary>
    public sealed class BasketballHoopViewModel : BoothMatchViewModel
    {
        readonly BasketballHoopSession _session;

        public BasketballHoopViewModel(BasketballHoopRules rules = null)
        {
            _session = new BasketballHoopSession(rules ?? new BasketballHoopRules());
            PublishState();
        }

        public BasketballHoopViewModel(int pointsPerBasket, int startingBalls, int winScoreThreshold)
            : this(new BasketballHoopRules(pointsPerBasket, startingBalls, winScoreThreshold))
        {
        }

        public AttractionId AttractionId => AttractionId.BasketballHoop;
        public string SceneName => AttractionCatalog.BasketballHoop.SceneName;

        public MatchPhase Phase => _session.Phase;
        public int Score => _session.Score;
        public int BallsRemaining => _session.BallsRemaining;
        public bool IsPlaying => Phase == MatchPhase.Playing;
        public bool CanThrow => IsPlaying && BallsRemaining > 0;
        public string StatusText => Describe(Phase);
        public string[] HudLines { get; private set; }

        public override void StartGame()
        {
            _session.Start();
            Notify();
        }

        public bool NotifyBallThrown() => Apply(_session.TryRegisterThrow);

        public bool NotifyBasket() => Apply(_session.TryRegisterBasket);

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
