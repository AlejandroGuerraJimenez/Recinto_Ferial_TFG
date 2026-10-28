using System;
using Fairground.Core.Attractions;
using Fairground.Model.Attractions.FerrisWheel;

namespace Fairground.ViewModel.Attractions.FerrisWheel
{
    /// <summary>
    /// Presentation state for the basic wheel. The view applies <see cref="AngleDegrees"/> each frame.
    /// </summary>
    public sealed class FerrisWheelViewModel
    {
        readonly FerrisWheelSession _session;

        public FerrisWheelViewModel(FerrisWheelRules rules = null)
        {
            _session = new FerrisWheelSession(rules ?? new FerrisWheelRules());
        }

        public FerrisWheelViewModel(float degreesPerSecond, int cabinCount)
            : this(new FerrisWheelRules(degreesPerSecond, cabinCount))
        {
        }

        public AttractionId AttractionId => AttractionId.FerrisWheel;
        public string SceneName => AttractionCatalog.FerrisWheel.SceneName;
        public event Action StateChanged;

        public FerrisWheelPhase Phase => _session.Phase;
        public float AngleDegrees => _session.AngleDegrees;
        public float DegreesPerSecond => _session.Rules.DegreesPerSecond;
        public int CabinCount => _session.Rules.CabinCount;
        public bool IsRiding => Phase == FerrisWheelPhase.Riding;

        public void StartRide()
        {
            _session.Start();
            StateChanged?.Invoke();
        }

        public void Advance(float deltaSeconds)
        {
            if (!IsRiding)
                return;

            _session.Advance(deltaSeconds);
            StateChanged?.Invoke();
        }
    }
}
