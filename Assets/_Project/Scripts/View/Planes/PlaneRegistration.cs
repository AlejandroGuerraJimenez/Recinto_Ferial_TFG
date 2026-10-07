using System;
using Fairground.Model;
using Fairground.ViewModel;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Fairground.View
{
    public sealed class PlaneRegistration
    {
        readonly PlaneVisualCatalog _catalog;
        readonly PlaneAimController _aim;
        readonly PlanePointer _pointer;
        readonly EditorStandInHost _standIn;
        readonly Func<PlaneSelectionState> _state;
        readonly Func<DetectedPlane, bool> _isSelectable;

        public PlaneRegistration(
            PlaneVisualCatalog catalog,
            PlaneAimController aim,
            PlanePointer pointer,
            EditorStandInHost standIn,
            Func<PlaneSelectionState> state,
            Func<DetectedPlane, bool> isSelectable)
        {
            _catalog = catalog;
            _aim = aim;
            _pointer = pointer;
            _standIn = standIn;
            _state = state;
            _isSelectable = isSelectable;
        }

        public void OnChanged(ARTrackablesChangedEventArgs<ARPlane> args)
        {
            for (var i = 0; i < args.added.Count; i++)
                Register(args.added[i]);
            for (var i = 0; i < args.updated.Count; i++)
                Register(args.updated[i]);
            for (var i = 0; i < args.removed.Count; i++)
                Unregister(args.removed[i].Key);
        }

        void Register(ARPlane plane)
        {
            if (plane == null)
                return;

            var visual = Bind(plane);
            if (ReplacesStandIn(visual))
                _standIn.Dismiss(_catalog, Clear);
        }

        void Unregister(TrackableId id)
        {
            if (!_catalog.TryTake(id.ToString(), out var visual))
                return;

            Clear(visual);
        }

        PlaneSurfaceVisual Bind(ARPlane plane)
        {
            var snapshot = DetectedPlaneMapper.FromPlane(plane);
            var visual = VisualOn(plane);
            visual.Bind(snapshot, Accepted(plane, snapshot));
            _catalog.Track(visual);
            return visual;
        }

        bool ReplacesStandIn(PlaneSurfaceVisual visual)
        {
            return visual.IsCandidate && _state() == PlaneSelectionState.AwaitingSelection;
        }

        void Clear(PlaneSurfaceVisual visual)
        {
            _aim.ClearIf(visual);
            _pointer.ClearIf(visual);
        }

        bool Accepted(ARPlane plane, DetectedPlane snapshot)
        {
            return plane.subsumedBy == null && _isSelectable(snapshot);
        }

        static PlaneSurfaceVisual VisualOn(ARPlane plane)
        {
            var visual = plane.GetComponent<PlaneSurfaceVisual>();
            return visual != null ? visual : plane.gameObject.AddComponent<PlaneSurfaceVisual>();
        }
    }
}
