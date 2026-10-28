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

        public BalloonThrowViewModel(BalloonThrowRules rules = null)
            : base((rules ?? Default).Attempt)
        {
            _rules = rules ?? Default;
        }

        public BalloonThrowViewModel(int pointsPerBalloon, int startingThrows, int balloonCount)
            : this(new BalloonThrowRules(pointsPerBalloon, startingThrows, balloonCount))
        {
        }

        public AttractionId AttractionId => AttractionId.BalloonThrow;

        public int ThrowsRemaining => AttemptsRemaining;
        public int BalloonsRemaining => TargetsRemaining;
        public int BalloonsPopped => Successes;
        public int BalloonCount => _rules.BalloonCount;
        public bool CanSpawnBall => CanSpendAttempt;

        public bool TryRegisterThrow() => TrySpendAttempt();

        public bool TryRegisterHit() => TryRegisterSuccess();

        public bool TryResolveThrow() => TryResolveAttempt();
    }
}
