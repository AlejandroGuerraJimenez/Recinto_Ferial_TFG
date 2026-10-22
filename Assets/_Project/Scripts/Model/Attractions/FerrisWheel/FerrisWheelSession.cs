using System;

namespace Fairground.Model.Attractions.FerrisWheel
{
    /// <summary>
    /// Mutable ride state. The wheel starts when <see cref="Start"/> is called and then only spins.
    /// </summary>
    public sealed class FerrisWheelSession
    {
        readonly FerrisWheelRules _rules;

        public FerrisWheelSession(FerrisWheelRules rules)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        public FerrisWheelRules Rules => _rules;
        public FerrisWheelPhase Phase { get; private set; } = FerrisWheelPhase.NotStarted;
        public float AngleDegrees { get; private set; }

        public void Start()
        {
            AngleDegrees = 0f;
            Phase = FerrisWheelPhase.Riding;
        }

        public void Advance(float deltaSeconds)
        {
            if (Phase != FerrisWheelPhase.Riding)
                return;

            AngleDegrees = FerrisWheelMotion.Advance(
                AngleDegrees,
                _rules.DegreesPerSecond,
                deltaSeconds);
        }
    }
}
