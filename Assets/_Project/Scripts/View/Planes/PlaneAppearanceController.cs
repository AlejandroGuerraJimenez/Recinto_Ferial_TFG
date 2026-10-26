using Fairground.ViewModel;
using UnityEngine.XR.ARFoundation;

namespace Fairground.View
{
    public sealed class PlaneAppearanceController
    {
        readonly PlaneVisualCatalog _catalog;
        readonly PlanePointer _pointer;
        readonly PlaneAimController _aim;

        public PlaneAppearanceController(PlaneVisualCatalog catalog, PlanePointer pointer, PlaneAimController aim)
        {
            _catalog = catalog;
            _pointer = pointer;
            _aim = aim;
        }

        public PlaneSurfaceVisual Proposal { get; private set; }

        public void Apply(PlaneSelectionState state)
        {
            Proposal = Choose();
            _catalog.ForEach(visual => visual.ApplyStyle(Style(state, visual)));
        }

        PlaneSurfaceVisual Choose()
        {
            PlaneSurfaceVisual best = null;
            _catalog.ForEach(visual =>
            {
                if (visual != null && visual.IsCandidate && (best == null || visual.Rank.CompareTo(best.Rank) > 0))
                    best = visual;
            });
            return best;
        }

        PlaneVisualStyle Style(PlaneSelectionState state, PlaneSurfaceVisual visual)
        {
            if (state == PlaneSelectionState.PlaneSelected)
                return LockedStyle(visual);
            if (!CanShow(visual))
                return PlaneVisualStyle.Hidden;
            if (_aim.Hovered == visual)
                return PlaneVisualStyle.Highlighted;
            return visual == Proposal ? PlaneVisualStyle.Proposed : PlaneVisualStyle.Dimmed;
        }

        PlaneVisualStyle LockedStyle(PlaneSurfaceVisual visual)
        {
            return visual == _pointer.Selected ? PlaneVisualStyle.Selected : PlaneVisualStyle.Hidden;
        }

        static bool CanShow(PlaneSurfaceVisual visual)
        {
            return visual.IsCandidate && IsTrackingVisible(visual);
        }

        static bool IsTrackingVisible(PlaneSurfaceVisual visual)
        {
            var plane = visual.Trackable;
            return plane == null || IsTracked(plane);
        }

        static bool IsTracked(ARPlane plane)
        {
            return plane.subsumedBy == null;
        }
    }
}
