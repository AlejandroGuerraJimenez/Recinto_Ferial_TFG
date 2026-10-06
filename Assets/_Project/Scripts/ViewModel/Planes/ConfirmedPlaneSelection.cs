using Fairground.Model;

namespace Fairground.ViewModel
{
    public sealed class ConfirmedPlaneSelection : IPlaneSelectionState
    {
        public static readonly ConfirmedPlaneSelection Instance = new ConfirmedPlaneSelection();

        ConfirmedPlaneSelection()
        {
        }

        public PlaneSelectionState Kind => PlaneSelectionState.PlaneSelected;

        public bool Select(PlaneSelectionContext context, DetectedPlane plane) => false;

        public void Restart(PlaneSelectionContext context)
        {
            context.Clear();
            context.Become(AwaitingPlaneSelection.Instance);
        }
    }
}
