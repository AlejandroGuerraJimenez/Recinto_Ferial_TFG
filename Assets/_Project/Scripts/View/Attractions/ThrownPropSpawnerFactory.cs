using UnityEngine;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Pedestal and the shared ball spawner. Each booth passes its own station.
    /// </summary>
    public static class ThrownPropSpawnerFactory
    {
        public static BallSpawnerView Create(
            ThrownPropView template,
            Vector3 position,
            float pedestalRadius,
            Vector3 spawnLocalPosition)
        {
            ThrowStation station = ThrowStation.Create(position, pedestalRadius, spawnLocalPosition);
            var spawner = station.Root.gameObject.AddComponent<BallSpawnerView>();
            spawner.Configure(template, station.SpawnPoint);
            return spawner;
        }
    }
}
