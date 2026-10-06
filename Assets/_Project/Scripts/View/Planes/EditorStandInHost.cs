using System;
using UnityEngine;

namespace Fairground.View
{
    public sealed class EditorStandInHost
    {
        readonly bool _enabled;
        readonly Transform _parent;
        readonly GameObject _prefab;
        readonly Func<bool> _detectionRunning;
        PlaneSurfaceVisual _anchor;
        bool _decided;

        public EditorStandInHost(bool enabled, Transform parent, GameObject prefab, Func<bool> detectionRunning)
        {
            _enabled = enabled;
            _parent = parent;
            _prefab = prefab;
            _detectionRunning = detectionRunning;
        }

        public void Tick(PlaneVisualCatalog catalog)
        {
            if (!_enabled || _decided)
                return;

            SpawnIfNeeded(catalog);
        }

        void SpawnIfNeeded(PlaneVisualCatalog catalog)
        {
#if UNITY_EDITOR
            TrySpawn(catalog);
#else
            _decided = true;
#endif
        }

        public void AllowAgain(PlaneVisualCatalog catalog)
        {
            if (_anchor == null && !catalog.HasRealPlane())
                _decided = false;
        }

        public void Dismiss(PlaneVisualCatalog catalog, Action<PlaneSurfaceVisual> clear)
        {
            if (_anchor == null)
                return;

            var root = RootOf(_anchor);
            catalog.ForgetChildren(root, clear);
            UnityEngine.Object.Destroy(root);
            _anchor = null;
        }

        GameObject RootOf(PlaneSurfaceVisual anchor)
        {
            var parent = anchor.transform.parent;
            if (parent == null || parent == _parent)
                return anchor.gameObject;

            return parent.gameObject;
        }

#if UNITY_EDITOR
        void TrySpawn(PlaneVisualCatalog catalog)
        {
            if (Time.unscaledTime < 0.75f)
                return;

            _decided = true;
            if (ShouldSkip(catalog))
                return;

            Spawn(catalog);
        }

        bool ShouldSkip(PlaneVisualCatalog catalog)
        {
            if (Application.isBatchMode || catalog.HasRealPlane())
                return true;

            return _detectionRunning();
        }

        void Spawn(PlaneVisualCatalog catalog)
        {
            var visual = EditorPlaneStandIn.Spawn(Camera.main, Fill(), Border(), !XrDevices.AnyPresent());
            if (visual == null)
                return;

            Adopt(visual, catalog);
            Debug.Log("Editor stand-in planes. Left click or Space confirms, R restarts. On Quest: right trigger confirms, B restarts.");
        }

        void Adopt(PlaneSurfaceVisual visual, PlaneVisualCatalog catalog)
        {
            var root = visual.transform.parent != null ? visual.transform.parent : visual.transform;
            root.SetParent(_parent, true);
            catalog.Adopt(root.gameObject);
            _anchor = visual;
        }

        Material Fill() => Template(visual => visual.FillTemplate);

        Material Border() => Template(visual => visual.BorderTemplate);

        Material Template(Func<PlaneSurfaceVisual, Material> pick)
        {
            var visual = _prefab != null ? _prefab.GetComponent<PlaneSurfaceVisual>() : null;
            return visual != null ? pick(visual) : null;
        }
#endif
    }
}
