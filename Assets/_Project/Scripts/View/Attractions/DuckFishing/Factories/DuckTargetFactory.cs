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
            new Color(0.95f, 0.75f, 0.15f, 1f),
            new Color(0.95f, 0.55f, 0.15f, 1f),
            new Color(0.85f, 0.85f, 0.2f, 1f),
        };

        const float PathRadius = 1.15f;
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
            parent.transform.localPosition = new Vector3(0f, 0.78f, 0f);
            return parent.transform;
        }

        static DuckTargetView CreateAt(Transform parent, Vector3 center, float angle, int index)
        {
            var duck = CreateBody(parent, index);
            var view = duck.AddComponent<DuckTargetView>();
            view.Configure(center, angle, PathRadius, PathSpeed);
            return view;
        }

        static GameObject CreateBody(Transform parent, int index)
        {
            var root = new GameObject($"Duck_{index + 1}");
            root.transform.SetParent(parent, false);

            var body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            body.name = "Body";
            body.transform.SetParent(root.transform, false);
            body.transform.localScale = new Vector3(0.28f, 0.22f, 0.34f);
            PrimitiveMaterialApplier.Apply(body, Palette[index % Palette.Length]);

            var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "Head";
            head.transform.SetParent(root.transform, false);
            head.transform.localPosition = new Vector3(0f, 0.08f, 0.16f);
            head.transform.localScale = Vector3.one * 0.16f;
            PrimitiveMaterialApplier.Apply(head, Palette[index % Palette.Length]);
            DestroyCollider(head);

            var beak = GameObject.CreatePrimitive(PrimitiveType.Cube);
            beak.name = "Beak";
            beak.transform.SetParent(root.transform, false);
            beak.transform.localPosition = new Vector3(0f, 0.06f, 0.26f);
            beak.transform.localScale = new Vector3(0.06f, 0.04f, 0.1f);
            PrimitiveMaterialApplier.Apply(beak, new Color(0.95f, 0.45f, 0.1f, 1f));
            DestroyCollider(beak);

            var bodyCollider = body.GetComponent<Collider>();
            if (bodyCollider != null)
                bodyCollider.isTrigger = true;

            var rootCollider = root.AddComponent<SphereCollider>();
            rootCollider.isTrigger = true;
            rootCollider.radius = 0.2f;

            return root;
        }

        static void DestroyCollider(GameObject target)
        {
            var collider = target.GetComponent<Collider>();
            if (collider != null)
                Object.Destroy(collider);
        }
    }
}
