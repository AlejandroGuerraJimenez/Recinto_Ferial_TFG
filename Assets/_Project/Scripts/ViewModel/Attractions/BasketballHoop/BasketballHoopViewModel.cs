using System;
using Fairground.Core.Attractions;
using Fairground.Model.Attractions.BasketballHoop;

namespace Fairground.ViewModel.Attractions.BasketballHoop
{
    /// <summary>
    /// Hoop verbs on a shared attempt match. The match owns the counters.
    /// </summary>
    public sealed class BasketballHoopViewModel : AttemptMatchViewModel
    {
        static readonly BasketballHoopRules Default = new BasketballHoopRules();

        public BasketballHoopViewModel()
            : this(Default)
        {
        }

        public BasketballHoopViewModel(BasketballHoopRules rules)
            : base((rules ?? throw new ArgumentNullException(nameof(rules))).Attempt)
        {
        }

        public BasketballHoopViewModel(int pointsPerBasket, int startingBalls, int winScoreThreshold)
            : this(new BasketballHoopRules(pointsPerBasket, startingBalls, winScoreThreshold))
        {
        }

        public AttractionId AttractionId => AttractionId.BasketballHoop;

        public bool CanThrow => CanSpendAttempt;

        public bool TryRegisterThrow() => TrySpendAttempt();

        public bool TryRegisterBasket() => TryRegisterSuccess();

        public bool TryResolveThrow() => TryResolveAttempt();
    }
}
