using Fairground.ViewModel;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

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

        public void Apply(PlaneSelectionState state)
        {
            _catalog.ForEach(visual => visual.ApplyStyle(Style(state, visual)));
        }

        PlaneVisualStyle Style(PlaneSelectionState state, PlaneSurfaceVisual visual)
        {
            if (state == PlaneSelectionState.PlaneSelected)
                return LockedStyle(visual);
            if (!CanShow(visual))
                return PlaneVisualStyle.Hidden;
            return _aim.Hovered == visual ? PlaneVisualStyle.Highlighted : IdleOrDim();
        }

        PlaneVisualStyle LockedStyle(PlaneSurfaceVisual visual)
        {
            return visual == _pointer.Selected ? PlaneVisualStyle.Selected : PlaneVisualStyle.Hidden;
        }

        PlaneVisualStyle IdleOrDim()
        {
            return _aim.Hovered != null ? PlaneVisualStyle.Dimmed : PlaneVisualStyle.Idle;
        }

        static bool CanShow(PlaneSurfaceVisual visual)
        {
            return visual.IsCandidate && IsTrackingVisible(visual);
        }

        static bool IsTrackingVisible(PlaneSurfaceVisual visual)
        {
            var plane = visual.GetComponent<ARPlane>();
            return plane == null || IsTracked(plane);
        }

        static bool IsTracked(ARPlane plane)
        {
            if (plane.subsumedBy != null)
                return false;

            return plane.trackingState >= TrackingState.Limited && ARSession.state > ARSessionState.Ready;
        }
    }
}
