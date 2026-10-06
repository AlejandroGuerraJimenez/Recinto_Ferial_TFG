using System;
using Fairground.Model;

namespace Fairground.ViewModel
{
    public sealed class PlaneSelectionContext
    {
        public PlaneSelectionContext(IPlaneSelectionState initial) => State = initial;

        public IPlaneSelectionState State { get; private set; }

        public DetectedPlane? Selected { get; private set; }

        public bool HasSelection => Selected.HasValue;

        public event Action<PlaneSelectionState> StateChanged;

        public event Action<DetectedPlane> PlaneSelected;

        public void Remember(DetectedPlane plane) => Selected = plane;

        public void Clear() => Selected = null;

        public void NotifySelected(DetectedPlane plane) => PlaneSelected?.Invoke(plane);

        public void Become(IPlaneSelectionState next)
        {
            State = next;
            StateChanged?.Invoke(next.Kind);
        }
    }
}
