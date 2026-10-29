using System;
using Fairground.ViewModel;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Restart and return-to-fairground for a booth. The booth keeps its own play button.
    /// </summary>
    public sealed class AttractionMatchControls : IDisposable
    {
        readonly AttractionInput _input;
        readonly Action _restart;
        FairgroundVisitState _visit;

        public AttractionMatchControls(Action restart, bool headsetTriggers = false)
        {
            _restart = restart ?? throw new ArgumentNullException(nameof(restart));
            _input = new AttractionInput(headsetTriggers: headsetTriggers);
            _input.Enable();
        }

        public AttractionInput Input => _input;

        public void Use(FairgroundVisitState visit) => _visit = visit;

        public void Tick()
        {
            if (_input.RestartPressed)
                _restart();

            if (_input.ReturnToFairgroundPressed)
                AttractionExit.ReturnToFairground(_visit);
        }

        public void Dispose() => _input.Dispose();
    }
}
