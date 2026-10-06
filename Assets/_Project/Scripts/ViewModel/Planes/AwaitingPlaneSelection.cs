using Fairground.Model;

namespace Fairground.ViewModel
{
    public sealed class AwaitingPlaneSelection : IPlaneSelectionState
    {
        public static readonly AwaitingPlaneSelection Instance = new AwaitingPlaneSelection();

        AwaitingPlaneSelection()
        {
        }

        public PlaneSelectionState Kind => PlaneSelectionState.AwaitingSelection;

        public bool Select(PlaneSelectionContext context, DetectedPlane plane)
        {
            if (!PlaneCandidateFilter.TryGetPriority(plane, out _))
                return false;

            Accept(context, plane);
            return true;
        }

        public void Restart(PlaneSelectionContext context)
        {
            if (!context.HasSelection)
                return;

            context.Clear();
            context.Become(Instance);
        }

        static void Accept(PlaneSelectionContext context, DetectedPlane plane)
        {
            context.Remember(plane);
            context.NotifySelected(plane);
            context.Become(ConfirmedPlaneSelection.Instance);
        }
    }
}
