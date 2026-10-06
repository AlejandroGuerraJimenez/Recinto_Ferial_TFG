using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow.Factories
{
    /// <summary>
    /// Factory: pedestal + ball spawner.
    /// </summary>
    public sealed class BallSpawnerFactory
    {
        public BallSpawnerView Create(ThrowableBallView template)
        {
            var root = new GameObject("BallSpawner");
            root.transform.position = new Vector3(0.35f, 1.05f, 0.55f);
            CreatePedestal(root.transform);
            Transform spawnPoint = CreateSpawnPoint(root.transform);
            var spawner = root.AddComponent<BallSpawnerView>();
            spawner.Configure(template, spawnPoint);
            return spawner;
        }

        static void CreatePedestal(Transform parent)
        {
            var pedestal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pedestal.name = "Pedestal";
            pedestal.transform.SetParent(parent, false);
            pedestal.transform.localPosition = new Vector3(0f, -0.35f, 0f);
            pedestal.transform.localScale = new Vector3(0.25f, 0.35f, 0.25f);
            PrimitiveMaterialApplier.Apply(pedestal, new Color(0.55f, 0.4f, 0.25f, 1f));
        }

        static Transform CreateSpawnPoint(Transform parent)
        {
            var spawnPoint = new GameObject("SpawnPoint");
            spawnPoint.transform.SetParent(parent, false);
            return spawnPoint.transform;
        }
    }
}
