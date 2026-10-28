using Fairground.Core.Planes;

namespace Fairground.ViewModel
{
    public interface IPlaneSelectionState
    {
        PlaneSelectionState Kind { get; }

        bool Select(PlaneSelectionContext context, DetectedPlane plane);

        void Restart(PlaneSelectionContext context);
    }
}
