using System;
using Fairground.Core.Attractions;
using Fairground.Model.Attractions;
using Fairground.Model.Attractions.BasketballHoop;

namespace Fairground.ViewModel.Attractions.BasketballHoop
{
    /// <summary>
    /// Timed hoop round. Throws do not spend a ball; the clock does.
    /// </summary>
    public sealed class BasketballHoopViewModel : BoothMatchViewModel
    {
        static readonly BasketballHoopRules Default = new BasketballHoopRules();

        readonly BasketballHoopMatch _match;
        int _shownSeconds;

        public BasketballHoopViewModel()
            : this(Default)
        {
        }

        public BasketballHoopViewModel(BasketballHoopRules rules)
        {
            _match = new BasketballHoopMatch(rules ?? throw new ArgumentNullException(nameof(rules)));
        }

        public BasketballHoopViewModel(int pointsPerBasket, int roundSeconds)
            : this(new BasketballHoopRules(pointsPerBasket, roundSeconds))
        {
        }

        public AttractionId AttractionId => AttractionId.BasketballHoop;
        public MatchPhase Phase => _match.Phase;
        public int Score => _match.Score;
        public int SecondsLeft => (int)Math.Ceiling(_match.SecondsRemaining);
        public bool IsPlaying => Phase == MatchPhase.Playing;
        public bool CanThrow => IsPlaying;

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

        public bool TryRegisterThrow() => IsPlaying;

        public bool TryResolveThrow() => false;

        public bool TryRegisterBasket() => Apply(_match.TryRegisterBasket);

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
