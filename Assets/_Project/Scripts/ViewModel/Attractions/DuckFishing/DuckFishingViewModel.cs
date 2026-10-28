using Fairground.Core.Attractions;
using Fairground.Model.Attractions;
using Fairground.Model.Attractions.DuckFishing;
using Fairground.ViewModel.Attractions;

namespace Fairground.ViewModel.Attractions.DuckFishing
{
    /// <summary>
    /// Publishes duck-match state. The match owns the counters.
    /// </summary>
    public sealed class DuckFishingViewModel : BoothMatchViewModel
    {
        readonly DuckFishingRules _rules;
        readonly AttemptMatch _match;

        public DuckFishingViewModel(DuckFishingRules rules = null)
        {
            _rules = rules ?? new DuckFishingRules();
            _match = new AttemptMatch(_rules.Attempt);
            PublishState();
        }

        public DuckFishingViewModel(int pointsPerDuck, int startingAttempts, int duckCount, int ducksToWin)
            : this(new DuckFishingRules(pointsPerDuck, startingAttempts, duckCount, ducksToWin))
        {
        }

        public AttractionId AttractionId => AttractionId.DuckFishing;
        public string SceneName => AttractionCatalog.DuckFishing.SceneName;

        public MatchPhase Phase => _match.Phase;
        public int Score => _match.Score;
        public int AttemptsRemaining => _match.AttemptsRemaining;
        public int DucksRemaining => _match.TargetsRemaining;
        public int DucksCaught => _match.Successes;
        public int DuckCount => _rules.DuckCount;
        public int DucksToWin => _rules.DucksToWin;
        public bool IsPlaying => Phase == MatchPhase.Playing;
        public bool CanDip => IsPlaying && AttemptsRemaining > 0;
        public string StatusText => Describe(Phase);
        public string[] HudLines { get; private set; }

        public override void StartGame()
        {
            _match.Start();
            Notify();
        }

        public bool NotifyDipStarted() => Apply(_match.TrySpendAttempt);

        public bool NotifyDuckCaught() => Apply(_match.TryRegisterSuccess);

        public bool NotifyDipResolved() => Apply(_match.ResolveAttempt);

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
                $"Hooks: {AttemptsRemaining}",
                $"Caught: {DucksCaught}/{DucksToWin}",
                StatusText,
            };
        }

        string Describe(MatchPhase phase)
        {
            switch (phase)
            {
                case MatchPhase.Playing: return $"Catch {DucksToWin} ducks!";
                case MatchPhase.Won: return "You win!";
                case MatchPhase.Lost: return "Out of hooks!";
                default: return "Ready";
            }
        }
    }
}
