using UnityEngine;

namespace Fairground.View.Attractions.BasketballHoop.Factories
{
    /// <summary>
    /// Factory: pedestal and the shared ball spawner.
    /// </summary>
    public sealed class BallSpawnerFactory
    {
        public BallSpawnerView Create(BasketballBallView template)
        {
            ThrowStation station = ThrowStation.Create(
                BasketballHoopLayout.SpawnerPosition,
                0.28f,
                new Vector3(0f, 0.16f, 0f));
            var spawner = station.Root.gameObject.AddComponent<BallSpawnerView>();
            spawner.Configure(template, station.SpawnPoint);
            return spawner;
        }
    }
}
