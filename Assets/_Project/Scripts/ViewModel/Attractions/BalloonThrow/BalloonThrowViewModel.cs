using System;
using Fairground.Core.Attractions;
using Fairground.Model.Attractions.BalloonThrow;

namespace Fairground.ViewModel.Attractions.BalloonThrow
{
    /// <summary>
    /// Balloon verbs on a shared attempt match. The match owns the counters.
    /// </summary>
    public sealed class BalloonThrowViewModel : AttemptMatchViewModel
    {
        static readonly BalloonThrowRules Default = new BalloonThrowRules();

        readonly BalloonThrowRules _rules;

        public BalloonThrowViewModel()
            : this(Default)
        {
        }

        public BalloonThrowViewModel(BalloonThrowRules rules)
            : base((rules ?? throw new ArgumentNullException(nameof(rules))).Attempt)
        {
            _rules = rules;
        }

        public BalloonThrowViewModel(int pointsPerBalloon, int startingThrows, int balloonCount)
            : this(new BalloonThrowRules(pointsPerBalloon, startingThrows, balloonCount))
        {
        }

        public AttractionId AttractionId => AttractionId.BalloonThrow;

        public int BalloonsRemaining => TargetsRemaining;
        public int BalloonsPopped => Successes;
        public int BalloonCount => _rules.BalloonCount;
        public bool CanThrow => CanSpendAttempt;

        public bool TryRegisterThrow() => TrySpendAttempt();

        public bool TryRegisterPop() => TryRegisterSuccess();

        public bool TryRegisterBomb() => Apply(() => Match.TryRegisterPenalty(_rules.BombPenalty));

        public bool TryResolveThrow() => TryResolveAttempt();
    }
}
