using Fairground.Core.Attractions;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Fairground.View
{
    /// <summary>
    /// Draws one parcel per attraction on the proposed flat surface.
    /// </summary>
    public sealed class FairgroundParcelPads
    {
        const float Lift = 0.05f;

        static readonly Color[] Colors =
        {
            new Color(0.9f, 0.2f, 0.25f, 1f),
            new Color(0.2f, 0.45f, 0.95f, 1f),
            new Color(0.95f, 0.75f, 0.15f, 1f),
            new Color(0.65f, 0.3f, 0.9f, 1f),
            new Color(0.95f, 0.45f, 0.15f, 1f),
        };

        Transform _root;
        string _planeId;

        public void Show(PlaneSurfaceVisual surface)
        {
            if (surface == null)
            {
                Hide();
                return;
            }

            var id = surface.Snapshot.Id;
            if (_root != null && _planeId == id && _root.parent == surface.transform)
                return;

            Hide();
            _planeId = id;
            _root = new GameObject("Parcels").transform;
            _root.SetParent(surface.transform, false);
            _root.localPosition = new Vector3(0f, Lift, 0f);
            _root.localRotation = Quaternion.identity;
            Build(surface);
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
        }

        void Build(PlaneSurfaceVisual surface)
        {
            var count = AttractionCatalog.Playable.Length;
            var width = SurfaceWidth(surface);
            var span = Mathf.Max(0.35f, width * 0.8f);
            var step = count <= 1 ? 0f : span / (count - 1);
            var size = Mathf.Min(0.42f, Mathf.Max(0.18f, step * 0.82f));
            var start = -step * (count - 1) * 0.5f;

            for (var i = 0; i < count; i++)
            {
                var attraction = AttractionCatalog.Playable[i];
                var pad = ParcelPad.Create(_root, attraction, Colors[i % Colors.Length], size);
                pad.transform.localPosition = new Vector3(start + step * i, 0f, 0f);
            }
        }

        static float SurfaceWidth(PlaneSurfaceVisual surface)
        {
            var plane = surface.GetComponent<ARPlane>();
            if (plane == null)
                return 2f;

            return Mathf.Max(plane.extents.x, plane.extents.y) * 2f;
        }
    }

    sealed class ParcelPad : MonoBehaviour
    {
            Material _material;

            public AttractionId AttractionId { get; private set; }

            public static ParcelPad Create(Transform parent, AttractionId attractionId, Color color, float size)
            {
                var root = new GameObject(AttractionCatalog.DisplayName(attractionId));
                root.transform.SetParent(parent, false);

                var pad = root.AddComponent<ParcelPad>();
                pad.AttractionId = attractionId;
                pad.BuildPlate(color, size);
                pad.BuildLabel();
                root.AddComponent<BoxCollider>().size = new Vector3(size, 0.08f, size);
                return pad;
            }

            void OnDestroy()
            {
                if (_material != null)
                    Destroy(_material);
            }

            void BuildPlate(Color color, float size)
            {
                var plate = GameObject.CreatePrimitive(PrimitiveType.Quad);
                plate.name = "Plate";
                plate.transform.SetParent(transform, false);
                plate.transform.localScale = new Vector3(size, size, 1f);
                plate.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
                Destroy(plate.GetComponent<Collider>());

                var shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Sprites/Default");
                if (shader == null)
                    return;

                _material = new Material(shader);
                if (_material.HasProperty("_BaseColor"))
                    _material.SetColor("_BaseColor", color);
                _material.color = color;
                plate.GetComponent<MeshRenderer>().sharedMaterial = _material;
            }

            void BuildLabel()
            {
                var label = new GameObject("Label");
                label.transform.SetParent(transform, false);
                label.transform.localPosition = new Vector3(0f, 0.02f, 0f);
                label.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
                label.transform.localScale = Vector3.one * 0.15f;

                var text = label.AddComponent<TextMeshPro>();
                text.font = TMP_Settings.defaultFontAsset;
                text.text = AttractionCatalog.DisplayName(AttractionId);
                text.fontSize = 4f;
                text.alignment = TextAlignmentOptions.Center;
                text.color = Color.white;
                text.rectTransform.sizeDelta = new Vector2(6f, 2f);
            }
    }
}
