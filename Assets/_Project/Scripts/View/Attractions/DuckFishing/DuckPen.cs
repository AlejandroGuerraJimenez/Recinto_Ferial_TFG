using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions.DuckFishing
{
    /// <summary>
    /// Small plot beside the pond. Pulled ducks stay here until the round restarts.
    /// </summary>
    public sealed class DuckPen : MonoBehaviour
    {
        static readonly Color Earth = new Color(0.42f, 0.5f, 0.28f, 1f);
        static readonly Color Trim = new Color(0.52f, 0.36f, 0.22f, 1f);

        Transform[] _slots;
        int _next;

        public static DuckPen Create(Transform booth, int duckCount)
        {
            int count = Mathf.Max(1, duckCount);
            var root = new GameObject("DuckPen");
            root.transform.SetParent(booth, false);
            root.transform.localPosition = new Vector3(2.45f, 0.02f, 2.45f);

            var deck = GameObject.CreatePrimitive(PrimitiveType.Cube);
            deck.name = "Plot";
            deck.transform.SetParent(root.transform, false);
            deck.transform.localScale = new Vector3(1.2f, 0.05f, 0.8f);
            PrimitiveMaterialApplier.Apply(deck, Earth);
            DestroyCollider(deck);

            AddTrim(root.transform, new Vector3(0f, 0.04f, 0.38f), new Vector3(1.2f, 0.06f, 0.05f));
            AddTrim(root.transform, new Vector3(0f, 0.04f, -0.38f), new Vector3(1.2f, 0.06f, 0.05f));
            AddTrim(root.transform, new Vector3(0.58f, 0.04f, 0f), new Vector3(0.05f, 0.06f, 0.8f));
            AddTrim(root.transform, new Vector3(-0.58f, 0.04f, 0f), new Vector3(0.05f, 0.06f, 0.8f));

            var pen = root.AddComponent<DuckPen>();
            pen._slots = BuildSlots(root.transform, count);
            return pen;
        }

        public void Place(DuckTargetView duck)
        {
            if (duck == null || _slots == null || _next >= _slots.Length)
                return;

            Transform slot = _slots[_next];
            _next++;
            duck.Park(slot.position, slot.rotation);
        }

        public void Clear() => _next = 0;

        static Transform[] BuildSlots(Transform pen, int count)
        {
            const int columns = 3;
            var slots = new Transform[count];
            for (int i = 0; i < count; i++)
            {
                int column = i % columns;
                int row = i / columns;
                var slot = new GameObject("Slot_" + (i + 1));
                slot.transform.SetParent(pen, false);
                slot.transform.localPosition = new Vector3((column - 1) * 0.34f, 0.16f, 0.16f - row * 0.32f);
                slot.transform.localRotation = Quaternion.LookRotation(Vector3.back, Vector3.up);
                slots[i] = slot.transform;
            }

            return slots;
        }

        static void AddTrim(Transform parent, Vector3 position, Vector3 scale)
        {
            var trim = GameObject.CreatePrimitive(PrimitiveType.Cube);
            trim.name = "Trim";
            trim.transform.SetParent(parent, false);
            trim.transform.localPosition = position;
            trim.transform.localScale = scale;
            PrimitiveMaterialApplier.Apply(trim, Trim);
            DestroyCollider(trim);
        }

        static void DestroyCollider(GameObject target)
        {
            var collider = target.GetComponent<Collider>();
            if (collider != null)
                Object.Destroy(collider);
        }
    }
}
