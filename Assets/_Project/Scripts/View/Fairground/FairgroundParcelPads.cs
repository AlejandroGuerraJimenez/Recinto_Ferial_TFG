using System.Collections.Generic;
using Fairground.Core.Attractions;
using Fairground.Model;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace Fairground.View
{
    /// <summary>
    /// Draws one parcel per attraction on the proposed flat surface.
    /// </summary>
    public sealed class FairgroundParcelPads
    {
        const float Lift = 0.06f;

        static readonly Color[] Colors =
        {
            new Color(0.9f, 0.2f, 0.25f, 1f),
            new Color(0.2f, 0.45f, 0.95f, 1f),
            new Color(0.95f, 0.75f, 0.15f, 1f),
            new Color(0.65f, 0.3f, 0.9f, 1f),
            new Color(0.95f, 0.45f, 0.15f, 1f),
        };

        readonly List<ParcelPad> _pads = new List<ParcelPad>();
        Transform _root;
        string _planeId;
        bool _pinned;

        public void Show(PlaneSurfaceVisual surface, Ray? aim)
        {
            if (_pinned)
                return;

            if (surface == null)
            {
                Hide();
                return;
            }

            var id = surface.Snapshot.Id;
            if (_root == null || _planeId != id || _root.parent != surface.transform)
                CreatePads(surface);

            Layout(surface, aim);
        }

        /// <summary>
        /// Stops moving the parcels and keeps them on <paramref name="surface"/>.
        /// </summary>
        public void PinTo(PlaneSurfaceVisual surface)
        {
            if (surface != null && (_root == null || _planeId != surface.Snapshot.Id || _root.parent != surface.transform))
                Show(surface, null);

            _pinned = _root != null;
        }

        public void Release()
        {
            _pinned = false;
            Hide();
        }

        public bool TryPick(Ray ray, out AttractionId attractionId)
        {
            attractionId = AttractionId.None;
            if (!Physics.Raycast(ray, out var hit, 8f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                return false;

            var pad = hit.collider.GetComponentInParent<ParcelPad>();
            if (pad == null)
                return false;

            attractionId = pad.AttractionId;
            return true;
        }

        public void Hide()
        {
            if (_root != null)
                Object.Destroy(_root.gameObject);

            _root = null;
            _planeId = null;
            _pads.Clear();
            _pinned = false;
        }

        void CreatePads(PlaneSurfaceVisual surface)
        {
            Hide();
            _planeId = surface.Snapshot.Id;
            _root = new GameObject("Parcels").transform;
            _root.SetParent(surface.transform, false);
            _root.localRotation = Quaternion.identity;

            var count = AttractionCatalog.Playable.Length;
            for (var i = 0; i < count; i++)
            {
                var attraction = AttractionCatalog.Playable[i];
                _pads.Add(ParcelPad.Create(_root, attraction, Colors[i % Colors.Length], surface.FillTemplate));
            }

            Debug.Log("[Parcelas] " + count + " sobre " + _planeId);
        }

        void Layout(PlaneSurfaceVisual surface, Ray? aim)
        {
            var alongX = true;
            var length = 2f;
            var anchor = Vector3.zero;
            if (TryBounds(surface.HasSnapshot ? surface.Snapshot.Boundary : null, out var min, out var max))
            {
                var bounds = max - min;
                alongX = bounds.x >= bounds.y;
                length = Mathf.Max(alongX ? bounds.x : bounds.y, 0.35f);
                anchor = Anchor(surface, min, max, aim);
            }

            _root.localPosition = anchor + new Vector3(0f, Lift, 0f);
            _root.localRotation = Quaternion.identity;

            var span = length * 0.8f;
            var count = _pads.Count;
            var step = count <= 1 ? 0f : span / (count - 1);
            var size = count <= 1 ? Mathf.Min(0.42f, span) : Mathf.Min(0.42f, step * 0.82f);
            if (count > 1 && step > 0f)
                size = Mathf.Min(size, step * 0.9f);
            size = Mathf.Max(size, 0.12f);

            var axis = alongX ? Vector3.right : Vector3.forward;
            var start = -step * (count - 1) * 0.5f;
            for (var i = 0; i < count; i++)
            {
                _pads[i].SetSize(size);
                _pads[i].transform.localPosition = axis * (start + step * i);
            }
        }

        static Vector3 Anchor(PlaneSurfaceVisual surface, Vector2 min, Vector2 max, Ray? aim)
        {
            var center = new Vector3((min.x + max.x) * 0.5f, 0f, (min.y + max.y) * 0.5f);
            if (aim == null)
                return center;

            var ray = aim.Value;
            var plane = new Plane(surface.transform.up, surface.transform.position);
            if (!plane.Raycast(ray, out var distance))
                return center;

            var local = surface.transform.InverseTransformPoint(ray.GetPoint(distance));
            local.x = ClampInside(local.x, min.x, max.x);
            local.z = ClampInside(local.z, min.y, max.y);
            local.y = 0f;
            return local;
        }

        static float ClampInside(float value, float min, float max)
        {
            var inset = Mathf.Min(0.2f, Mathf.Max(0f, max - min) * 0.5f);
            return Mathf.Clamp(value, min + inset, max - inset);
        }

        static bool TryBounds(IReadOnlyList<PlanePoint> boundary, out Vector2 min, out Vector2 max)
        {
            min = max = default;
            if (boundary == null || boundary.Count == 0)
                return false;

            min = max = new Vector2(boundary[0].X, boundary[0].Y);
            for (var i = 1; i < boundary.Count; i++)
            {
                var point = new Vector2(boundary[i].X, boundary[i].Y);
                min = Vector2.Min(min, point);
                max = Vector2.Max(max, point);
            }

            return max.x - min.x > 0.05f && max.y - min.y > 0.05f;
        }
    }

    sealed class ParcelPad : MonoBehaviour
    {
        Material _material;
        Transform _plate;

        public AttractionId AttractionId { get; private set; }

        public static ParcelPad Create(Transform parent, AttractionId attractionId, Color color, Material template)
        {
            var root = new GameObject(AttractionCatalog.DisplayName(attractionId));
            root.transform.SetParent(parent, false);

            var pad = root.AddComponent<ParcelPad>();
            pad.AttractionId = attractionId;
            pad.BuildPlate(color, template);
            pad.BuildLabel();
            return pad;
        }

        public void SetSize(float size)
        {
            if (_plate == null)
                return;

            _plate.localScale = new Vector3(size, 0.04f, size);
        }

        void OnDestroy()
        {
            if (_material != null)
                Destroy(_material);
        }

        void BuildPlate(Color color, Material template)
        {
            var plate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plate.name = "Plate";
            plate.transform.SetParent(transform, false);
            plate.transform.localPosition = new Vector3(0f, 0.02f, 0f);
            _plate = plate.transform;

            var renderer = plate.GetComponent<MeshRenderer>();
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            _material = Solid(template, color);
            if (_material != null)
                renderer.sharedMaterial = _material;
        }

        void BuildLabel()
        {
            var label = new GameObject("Label");
            label.transform.SetParent(transform, false);
            label.transform.localPosition = new Vector3(0f, 0.06f, 0f);
            label.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            label.transform.localScale = Vector3.one * 0.12f;

            var text = label.AddComponent<TextMeshPro>();
            text.font = TMP_Settings.defaultFontAsset;
            text.text = AttractionCatalog.DisplayName(AttractionId);
            text.fontSize = 4.5f;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            text.rectTransform.sizeDelta = new Vector2(8f, 2f);
        }

        static Material Solid(Material template, Color color)
        {
            if (template == null)
                return null;

            var material = new Material(template);
            color.a = 1f;
            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", color);
            material.color = color;
            return material;
        }
    }
}
