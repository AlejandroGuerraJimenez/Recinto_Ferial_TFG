using System;
using Fairground.Core.Attractions;
using Fairground.Model.Attractions.FerrisWheel;

namespace Fairground.ViewModel.Attractions.FerrisWheel
{
    /// <summary>
    /// Presentation state for the basic wheel. The view applies <see cref="AngleDegrees"/> each frame.
    /// Cabin count is geometry on the rig, not a ride rule.
    /// </summary>
    public sealed class FerrisWheelViewModel
    {
        readonly FerrisWheelRide _ride;

        public FerrisWheelViewModel(float degreesPerSecond = FerrisWheelBalance.DegreesPerSecond)
        {
            _ride = new FerrisWheelRide(new FerrisWheelRules(degreesPerSecond));
        }

        public AttractionId AttractionId => AttractionId.FerrisWheel;
        public event Action StateChanged;

        public FerrisWheelPhase Phase => _ride.Phase;
        public float AngleDegrees => _ride.AngleDegrees;
        public float DegreesPerSecond => _ride.Rules.DegreesPerSecond;
        public bool IsRiding => Phase == FerrisWheelPhase.Riding;

        public void StartRide()
        {
            _ride.Start();
            StateChanged?.Invoke();
        }

        public void Advance(float deltaSeconds)
        {
            if (!IsRiding)
                return;

            _ride.Advance(deltaSeconds);
        }
    }
}
