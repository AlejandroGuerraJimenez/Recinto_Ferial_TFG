using Fairground.View.Attractions.DuckFishing;
using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions.DuckFishing.Factories
{
    /// <summary>
    /// Factory: builds circling duck targets in the pond.
    /// </summary>
    public sealed class DuckTargetFactory
    {
        static readonly Color[] Palette =
        {
            new Color(0.98f, 0.82f, 0.12f, 1f),
            new Color(0.9f, 0.2f, 0.16f, 1f),
            new Color(0.18f, 0.42f, 0.84f, 1f),
        };

        static readonly Color Belly = new Color(0.96f, 0.94f, 0.88f, 1f);
        static readonly Color Beak = new Color(0.95f, 0.46f, 0.08f, 1f);
        static readonly Color Eye = new Color(0.08f, 0.06f, 0.05f, 1f);
        static readonly Color EyeShine = new Color(0.95f, 0.95f, 0.92f, 1f);
        static readonly Color RingBrass = new Color(0.93f, 0.72f, 0.28f, 1f);

        const float PathRadius = 1.05f;
        const float PathSpeed = 0.65f;

        public DuckTargetView[] CreateRing(Transform pond, int count)
        {
            Transform parent = CreateParent(pond);
            Vector3 center = parent.position;
            int safeCount = Mathf.Max(1, count);
            var ducks = new DuckTargetView[safeCount];

            for (int i = 0; i < safeCount; i++)
            {
                float angle = (Mathf.PI * 2f / safeCount) * i;
                ducks[i] = CreateAt(parent, center, angle, i);
            }

            return ducks;
        }

        static Transform CreateParent(Transform pond)
        {
            var parent = new GameObject("Ducks");
            parent.transform.SetParent(pond, false);
            parent.transform.localPosition = new Vector3(0f, DuckFishingBoothLayout.DuckFloatY, 0f);
            return parent.transform;
        }

        static DuckTargetView CreateAt(Transform parent, Vector3 center, float angle, int index)
        {
            var duck = CreateBody(parent, index);
            Transform ring = CreateRing(duck.transform);
            var view = duck.AddComponent<DuckTargetView>();
            view.AssignRing(ring);
            view.Configure(center, angle, PathRadius, PathSpeed);
            return view;
        }

        static GameObject CreateBody(Transform parent, int index)
        {
            Color body = Palette[index % Palette.Length];
            Color wing = Shade(body, 0.78f);

            var root = new GameObject($"Duck_{index + 1}");
            root.transform.SetParent(parent, false);

            var hull = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hull.name = "Body";
            hull.transform.SetParent(root.transform, false);
            hull.transform.localScale = new Vector3(0.3f, 0.22f, 0.36f);
            PrimitiveMaterialApplier.Apply(hull, body);
            DestroyCollider(hull);

            Part(root.transform, "Belly", PrimitiveType.Sphere, new Vector3(0f, -0.045f, 0.03f), new Vector3(0.2f, 0.12f, 0.22f), Belly);
            Part(root.transform, "Head", PrimitiveType.Sphere, new Vector3(0f, 0.12f, 0.15f), Vector3.one * 0.17f, body);
            Part(root.transform, "Beak", PrimitiveType.Cube, new Vector3(0f, 0.1f, 0.25f), new Vector3(0.07f, 0.038f, 0.11f), Beak);
            Part(root.transform, "WingLeft", PrimitiveType.Sphere, new Vector3(-0.15f, 0.01f, -0.01f), new Vector3(0.06f, 0.1f, 0.2f), wing, new Vector3(0f, 0f, 16f));
            Part(root.transform, "WingRight", PrimitiveType.Sphere, new Vector3(0.15f, 0.01f, -0.01f), new Vector3(0.06f, 0.1f, 0.2f), wing, new Vector3(0f, 0f, -16f));
            Part(root.transform, "Tail", PrimitiveType.Cube, new Vector3(0f, 0.07f, -0.18f), new Vector3(0.07f, 0.07f, 0.09f), wing, new Vector3(-28f, 0f, 0f));
            Part(root.transform, "EyeLeft", PrimitiveType.Sphere, new Vector3(-0.05f, 0.15f, 0.21f), Vector3.one * 0.038f, Eye);
            Part(root.transform, "EyeRight", PrimitiveType.Sphere, new Vector3(0.05f, 0.15f, 0.21f), Vector3.one * 0.038f, Eye);
            Part(root.transform, "ShineLeft", PrimitiveType.Sphere, new Vector3(-0.04f, 0.16f, 0.228f), Vector3.one * 0.014f, EyeShine);
            Part(root.transform, "ShineRight", PrimitiveType.Sphere, new Vector3(0.058f, 0.16f, 0.228f), Vector3.one * 0.014f, EyeShine);
            return root;
        }

        static Transform CreateRing(Transform duck)
        {
            var ring = new GameObject("Ring");
            ring.transform.SetParent(duck, false);
            ring.transform.localPosition = new Vector3(0f, 0.2f, -0.04f);

            const int segments = 20;
            const float radius = 0.065f;
            const float tube = 0.014f;
            float chord = 2f * radius * Mathf.Sin(Mathf.PI / segments);
            for (int i = 0; i < segments; i++)
            {
                float turns = (Mathf.PI * 2f * i) / segments;
                var piece = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                piece.name = "Band";
                piece.transform.SetParent(ring.transform, false);
                piece.transform.localPosition = new Vector3(Mathf.Cos(turns) * radius, Mathf.Sin(turns) * radius, 0f);
                piece.transform.localRotation = Quaternion.FromToRotation(
                    Vector3.up,
                    new Vector3(-Mathf.Sin(turns), Mathf.Cos(turns), 0f));
                piece.transform.localScale = new Vector3(tube, chord * 0.62f, tube);
                PrimitiveMaterialApplier.Apply(piece, RingBrass);
                DestroyCollider(piece);
            }

            var hole = ring.AddComponent<SphereCollider>();
            hole.isTrigger = true;
            hole.radius = radius * 0.75f;
            return ring.transform;
        }

        static void Part(
            Transform parent,
            string name,
            PrimitiveType type,
            Vector3 position,
            Vector3 scale,
            Color color,
            Vector3 euler = default)
        {
            var part = GameObject.CreatePrimitive(type);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = position;
            part.transform.localEulerAngles = euler;
            part.transform.localScale = scale;
            PrimitiveMaterialApplier.Apply(part, color);
            DestroyCollider(part);
        }

        static Color Shade(Color color, float amount)
        {
            return new Color(color.r * amount, color.g * amount, color.b * amount, 1f);
        }

        static void DestroyCollider(GameObject target)
        {
            var collider = target.GetComponent<Collider>();
            if (collider != null)
                Object.Destroy(collider);
        }
    }
}
