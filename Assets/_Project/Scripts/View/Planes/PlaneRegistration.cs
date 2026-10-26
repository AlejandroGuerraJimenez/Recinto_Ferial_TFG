using System;
using System.Collections.Generic;
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
        readonly Dictionary<string, ARPlane> _watched = new Dictionary<string, ARPlane>();

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

        public void CatchUp(ARPlaneManager manager)
        {
            if (manager == null)
                return;

            foreach (var plane in manager.trackables)
                Register(plane);
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
            Watch(plane);
            if (visual.IsCandidate && _state() == PlaneSelectionState.AwaitingSelection)
                _standIn.Dismiss(_catalog, Clear);
        }

        void Unregister(TrackableId id)
        {
            var key = id.ToString();
            Unwatch(key);
            if (!_catalog.TryTake(key, out var visual))
                return;

            Clear(visual);
        }

        PlaneSurfaceVisual Bind(ARPlane plane)
        {
            var visual = VisualOn(plane);
            if (StillCurrent(visual, plane))
            {
                _catalog.Track(visual);
                return visual;
            }

            var snapshot = DetectedPlaneMapper.FromPlane(plane);
            var accepted = plane.subsumedBy == null && _isSelectable(snapshot);
            var rank = accepted && PlaneCandidateFilter.TryRank(snapshot, out var scored) ? scored : default;
            visual.ApplySnapshot(snapshot, accepted, rank);
            _catalog.Track(visual);
            return visual;
        }

        static bool StillCurrent(PlaneSurfaceVisual visual, ARPlane plane)
        {
            if (!visual.HasSnapshot || !DetectedPlaneMapper.Matches(plane, visual.Snapshot))
                return false;

            return plane.subsumedBy == null || !visual.IsCandidate;
        }

        void Watch(ARPlane plane)
        {
            var key = plane.trackableId.ToString();
            if (_watched.ContainsKey(key))
                return;

            _watched.Add(key, plane);
            plane.boundaryChanged += OnBoundary;
        }

        void Unwatch(string key)
        {
            if (!_watched.TryGetValue(key, out var plane))
                return;

            plane.boundaryChanged -= OnBoundary;
            _watched.Remove(key);
        }

        void OnBoundary(ARPlaneBoundaryChangedEventArgs args)
        {
            if (args.plane != null)
                Bind(args.plane);
        }

        void Clear(PlaneSurfaceVisual visual)
        {
            _aim.ClearIf(visual);
            _pointer.ClearIf(visual);
        }

        static PlaneSurfaceVisual VisualOn(ARPlane plane)
        {
            var visual = plane.GetComponent<PlaneSurfaceVisual>();
            return visual != null ? visual : plane.gameObject.AddComponent<PlaneSurfaceVisual>();
        }
    }
}
