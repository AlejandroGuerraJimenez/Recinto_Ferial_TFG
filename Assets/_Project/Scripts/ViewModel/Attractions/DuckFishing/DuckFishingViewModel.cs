using System;
using Fairground.Core.Attractions;
using Fairground.Model.Attractions;
using Fairground.Model.Attractions.DuckFishing;

namespace Fairground.ViewModel.Attractions.DuckFishing
{
    /// <summary>
    /// Timed duck round. The score is how many ducks have been pulled out.
    /// </summary>
    public sealed class DuckFishingViewModel : BoothMatchViewModel
    {
        static readonly DuckFishingRules Default = new DuckFishingRules();

        readonly DuckFishingMatch _match;
        int _shownSeconds;

        public DuckFishingViewModel()
            : this(Default)
        {
        }

        public DuckFishingViewModel(DuckFishingRules rules)
        {
            _match = new DuckFishingMatch(rules ?? throw new ArgumentNullException(nameof(rules)));
        }

        public DuckFishingViewModel(int roundSeconds, int duckCount)
            : this(new DuckFishingRules(roundSeconds, duckCount))
        {
        }

        public AttractionId AttractionId => AttractionId.DuckFishing;
        public MatchPhase Phase => _match.Phase;
        public int Score => _match.Score;
        public int DucksCaught => _match.DucksCaught;
        public int DuckCount => _match.DuckCount;
        public int SecondsLeft => (int)Math.Ceiling(_match.SecondsRemaining);
        public bool IsPlaying => Phase == MatchPhase.Playing;
        public bool CanFish => IsPlaying;

        public override void StartMatch()
        {
            _match.Start();
            _shownSeconds = SecondsLeft;
            Notify();
        }

        public override void Restart()
        {
            _match.Restart();
            _shownSeconds = SecondsLeft;
            Notify();
        }

        public bool TryRegisterCatch() => Apply(_match.TryRegisterCatch);

        public void Tick(float deltaSeconds)
        {
            if (!IsPlaying)
                return;

            _match.Tick(deltaSeconds);
            if (!IsPlaying || SecondsLeft != _shownSeconds)
            {
                _shownSeconds = SecondsLeft;
                Notify();
            }
        }
    }
}
