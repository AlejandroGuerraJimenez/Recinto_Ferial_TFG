using Fairground.Core.Attractions;
using Fairground.Model.Attractions.DuckFishing;

namespace Fairground.ViewModel.Attractions.DuckFishing
{
    /// <summary>
    /// Duck verbs on a shared attempt match. The match owns the counters.
    /// </summary>
    public sealed class DuckFishingViewModel : AttemptMatchViewModel
    {
        static readonly DuckFishingRules Default = new DuckFishingRules();

        readonly DuckFishingRules _rules;

        public DuckFishingViewModel(DuckFishingRules rules = null)
            : base((rules ?? Default).Attempt)
        {
            _rules = rules ?? Default;
        }

        public DuckFishingViewModel(int pointsPerDuck, int startingAttempts, int duckCount, int ducksToWin)
            : this(new DuckFishingRules(pointsPerDuck, startingAttempts, duckCount, ducksToWin))
        {
        }

        public AttractionId AttractionId => AttractionId.DuckFishing;

        public int DucksRemaining => TargetsRemaining;
        public int DucksCaught => Successes;
        public int DuckCount => _rules.DuckCount;
        public int DucksToWin => _rules.DucksToWin;
        public bool CanDip => CanSpendAttempt;

        public bool TryRegisterDip() => TrySpendAttempt();

        public bool TryRegisterCatch() => TryRegisterSuccess();

        public bool TryResolveDip() => TryResolveAttempt();
    }
}
