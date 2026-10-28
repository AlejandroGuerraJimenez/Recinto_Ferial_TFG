using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Pedestal and spawn point shared by throwable booths. Position and radius stay with the booth.
    /// </summary>
    public readonly struct ThrowStation
    {
        public ThrowStation(Transform root, Transform spawnPoint)
        {
            Root = root;
            SpawnPoint = spawnPoint;
        }

        public Transform Root { get; }
        public Transform SpawnPoint { get; }

        public static ThrowStation Create(Vector3 position, float pedestalRadius, Vector3 spawnLocalPosition)
        {
            var root = new GameObject("BallSpawner");
            root.transform.position = position;

            var pedestal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pedestal.name = "Pedestal";
            pedestal.transform.SetParent(root.transform, false);
            pedestal.transform.localPosition = new Vector3(0f, -0.35f, 0f);
            pedestal.transform.localScale = new Vector3(pedestalRadius, 0.35f, pedestalRadius);
            PrimitiveMaterialApplier.Apply(pedestal, new Color(0.55f, 0.4f, 0.25f, 1f));

            var spawnPoint = new GameObject("SpawnPoint");
            spawnPoint.transform.SetParent(root.transform, false);
            spawnPoint.transform.localPosition = spawnLocalPosition;
            return new ThrowStation(root.transform, spawnPoint.transform);
        }
    }
}
