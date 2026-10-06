using Fairground.ViewModel;
using UnityEngine;

namespace Fairground.View
{
    public sealed class PlaneAimController
    {
        readonly IAimRaySource _source;
        readonly float _distance;

        public PlaneAimController(IAimRaySource source, float distance)
        {
            _source = source;
            _distance = distance;
        }

        public PlaneSurfaceVisual Hovered { get; private set; }

        public bool HasAim { get; private set; }

        public Vector3 Origin { get; private set; }

        public Vector3 End { get; private set; }

        public void Hover(PlaneSurfaceVisual visual) => Hovered = visual;

        public void ClearIf(PlaneSurfaceVisual visual)
        {
            if (Hovered == visual)
                Hovered = null;
        }

        public void Update(PlaneSelectionState state)
        {
            Reset();
            if (state != PlaneSelectionState.AwaitingSelection)
                return;

            if (_source.TryGetRay(out var ray))
                Cast(ray);
        }

        void Reset()
        {
            Hovered = null;
            HasAim = false;
        }

        void Cast(Ray ray)
        {
            HasAim = true;
            Origin = ray.origin;
            End = ray.origin + ray.direction * _distance;
            if (Physics.Raycast(ray, out var hit, _distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                ApplyHit(hit);
        }

        void ApplyHit(RaycastHit hit)
        {
            End = hit.point;
            var visual = hit.collider.GetComponentInParent<PlaneSurfaceVisual>();
            if (visual != null && visual.IsCandidate)
                Hovered = visual;
        }
    }
}
