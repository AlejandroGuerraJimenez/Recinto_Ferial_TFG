using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow.Factories
{
    /// <summary>
    /// Factory: builds balloon targets in a booth grid.
    /// </summary>
    public sealed class BalloonTargetFactory
    {
        static readonly Color[] Palette =
        {
            new Color(0.9f, 0.15f, 0.2f, 1f),
            new Color(0.15f, 0.4f, 0.95f, 1f),
            new Color(0.95f, 0.85f, 0.15f, 1f),
        };

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
            parent.transform.position = new Vector3(0f, 0f, 3.6f);
            return parent.transform;
        }

        static BalloonTargetView CreateAt(Transform parent, int index)
        {
            var balloon = CreateSphere(parent, index);
            ConfigureBody(balloon.AddComponent<Rigidbody>());
            PrimitiveMaterialApplier.Apply(balloon, Palette[index % Palette.Length]);
            return balloon.AddComponent<BalloonTargetView>();
        }

        static GameObject CreateSphere(Transform parent, int index)
        {
            var balloon = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            balloon.name = $"Balloon_{index + 1}";
            balloon.transform.SetParent(parent, false);
            PlaceInGrid(balloon.transform, index);
            return balloon;
        }

        static void PlaceInGrid(Transform target, int index)
        {
            int row = index / 3;
            int col = index % 3;
            target.localPosition = new Vector3((col - 1) * 0.7f, 1.1f + row * 0.65f, 0f);
            target.localScale = Vector3.one * 0.35f;
        }

        static void ConfigureBody(Rigidbody body)
        {
            body.isKinematic = true;
            body.useGravity = false;
        }
    }
}
