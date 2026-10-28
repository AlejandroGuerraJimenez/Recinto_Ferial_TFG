using Fairground.Core.Attractions;
using Fairground.Model.Attractions;
using Fairground.Model.Attractions.DuckFishing;
using Fairground.ViewModel.Attractions;

namespace Fairground.ViewModel.Attractions.DuckFishing
{
    /// <summary>
    /// Publishes duck-match state. The session owns the counters.
    /// </summary>
    public sealed class DuckFishingViewModel : BoothMatchViewModel
    {
        readonly DuckFishingSession _session;

        public DuckFishingViewModel(DuckFishingRules rules = null)
        {
            _session = new DuckFishingSession(rules ?? new DuckFishingRules());
            PublishState();
        }

        public DuckFishingViewModel(int pointsPerDuck, int startingAttempts, int duckCount, int ducksToWin)
            : this(new DuckFishingRules(pointsPerDuck, startingAttempts, duckCount, ducksToWin))
        {
        }

        public AttractionId AttractionId => AttractionId.DuckFishing;
        public string SceneName => AttractionCatalog.DuckFishing.SceneName;

        public MatchPhase Phase => _session.Phase;
        public int Score => _session.Score;
        public int AttemptsRemaining => _session.AttemptsRemaining;
        public int DucksRemaining => _session.DucksRemaining;
        public int DucksCaught => _session.DucksCaught;
        public int DuckCount => _session.Rules.DuckCount;
        public int DucksToWin => _session.Rules.DucksToWin;
        public bool IsPlaying => Phase == MatchPhase.Playing;
        public bool CanDip => IsPlaying && AttemptsRemaining > 0;
        public string StatusText => Describe(Phase);
        public string[] HudLines { get; private set; }

        public override void StartGame()
        {
            _session.Start();
            Notify();
        }

        public bool NotifyDipStarted() => Apply(_session.TryRegisterDip);

        public bool NotifyDuckCaught() => Apply(_session.TryRegisterCatch);

        public bool NotifyDipResolved() => Apply(_session.ResolveDip);

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
