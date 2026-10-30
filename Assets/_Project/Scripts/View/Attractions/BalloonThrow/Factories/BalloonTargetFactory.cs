using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow.Factories
{
    /// <summary>
    /// Factory: builds the six bullseyes hung on the gallery. Gameplay still pops the whole target.
    /// </summary>
    public sealed class BalloonTargetFactory
    {
        static readonly Color RingRed = new Color(0.9f, 0.12f, 0.14f, 1f);
        static readonly Color RingWhite = new Color(0.96f, 0.95f, 0.92f, 1f);

        public BalloonTargetView[] CreateRow(int count)
        {
            Transform parent = CreateParent();
            int safeCount = Mathf.Max(1, count);
            var balloons = new BalloonTargetView[safeCount];

            for (int i = 0; i < safeCount; i++)
                balloons[i] = CreateAt(parent, i);

            return balloons;
        }

        static Transform CreateParent()
        {
            var parent = new GameObject("Balloons");
            parent.transform.position = Vector3.zero;
            return parent.transform;
        }

        static BalloonTargetView CreateAt(Transform parent, int index)
        {
            var root = new GameObject($"Balloon_{index + 1}");
            root.transform.SetParent(parent, false);
            root.transform.localPosition = Slot(index);
            root.transform.localScale = Vector3.one * 0.42f;

            var body = root.AddComponent<SphereCollider>();
            body.radius = 0.5f;
            ConfigureBody(root.AddComponent<Rigidbody>());
            AddBullseye(root.transform);
            return root.AddComponent<BalloonTargetView>();
        }

        static Vector3 Slot(int index)
        {
            Vector3[] slots = BalloonThrowBoothLayout.TargetSlots;
            if (index >= 0 && index < slots.Length)
                return slots[index];

            int row = index / 3;
            int col = index % 3;
            float depth = row == 0
                ? BalloonThrowBoothLayout.FrontZ
                : row == 1
                    ? BalloonThrowBoothLayout.MiddleZ
                    : BalloonThrowBoothLayout.BackZ;
            return new Vector3((col - 1) * 0.78f, 0.62f + row * 0.66f, depth);
        }

        static void AddBullseye(Transform root)
        {
            Disc(root, "Board", 1.08f, 0.04f, RingRed);
            Disc(root, "RingWhite", 0.78f, -0.02f, RingWhite);
            Disc(root, "RingRed", 0.52f, -0.06f, RingRed);
            Disc(root, "InnerWhite", 0.28f, -0.1f, RingWhite);
            Disc(root, "Pin", 0.12f, -0.14f, RingRed);
        }

        static void Disc(Transform parent, string name, float diameter, float z, Color color)
        {
            var disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            disc.name = name;
            disc.transform.SetParent(parent, false);
            disc.transform.localPosition = new Vector3(0f, 0f, z);
            disc.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            disc.transform.localScale = new Vector3(diameter, 0.03f, diameter);
            PrimitiveMaterialApplier.Apply(disc, color);
            var collider = disc.GetComponent<Collider>();
            if (collider != null)
                Object.Destroy(collider);
        }

        static void ConfigureBody(Rigidbody body)
        {
            body.isKinematic = true;
            body.useGravity = false;
        }
    }
}
