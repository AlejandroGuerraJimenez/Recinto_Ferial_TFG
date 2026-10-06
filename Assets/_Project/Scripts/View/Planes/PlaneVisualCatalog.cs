using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Fairground.View
{
    public sealed class PlaneVisualCatalog
    {
        readonly List<PlaneSurfaceVisual> _visuals = new List<PlaneSurfaceVisual>();
        readonly Dictionary<string, PlaneSurfaceVisual> _byId = new Dictionary<string, PlaneSurfaceVisual>();

        public void Track(PlaneSurfaceVisual visual)
        {
            var id = visual.Snapshot.Id;
            if (!_byId.ContainsKey(id))
                _visuals.Add(visual);
            _byId[id] = visual;
        }

        public bool TryTake(string id, out PlaneSurfaceVisual visual)
        {
            if (!_byId.TryGetValue(id, out visual))
                return false;

            _byId.Remove(id);
            _visuals.Remove(visual);
            return true;
        }

        public void Prune() => _visuals.RemoveAll(visual => visual == null);

        public void ForEach(Action<PlaneSurfaceVisual> action)
        {
            for (var i = 0; i < _visuals.Count; i++)
                Visit(_visuals[i], action);
        }

        public void Adopt(GameObject root)
        {
            var visuals = root.GetComponentsInChildren<PlaneSurfaceVisual>(true);
            for (var i = 0; i < visuals.Length; i++)
                Track(visuals[i]);
        }

        public void ForgetChildren(GameObject root, Action<PlaneSurfaceVisual> clear)
        {
            var visuals = root.GetComponentsInChildren<PlaneSurfaceVisual>(true);
            for (var i = 0; i < visuals.Length; i++)
                Forget(visuals[i], clear);
        }

        public bool HasRealPlane()
        {
            for (var i = 0; i < _visuals.Count; i++)
                if (IsReal(_visuals[i]))
                    return true;
            return false;
        }

        void Forget(PlaneSurfaceVisual visual, Action<PlaneSurfaceVisual> clear)
        {
            if (visual.HasSnapshot)
                _byId.Remove(visual.Snapshot.Id);
            _visuals.Remove(visual);
            clear(visual);
        }

        static void Visit(PlaneSurfaceVisual visual, Action<PlaneSurfaceVisual> action)
        {
            if (visual != null)
                action(visual);
        }

        static bool IsReal(PlaneSurfaceVisual visual)
        {
            return visual != null && visual.GetComponent<ARPlane>() != null;
        }
    }
}
