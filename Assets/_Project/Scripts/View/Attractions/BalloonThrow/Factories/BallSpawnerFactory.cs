using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow.Factories
{
    /// <summary>
    /// Factory: pedestal and the shared ball spawner.
    /// </summary>
    public sealed class BallSpawnerFactory
    {
        public BallSpawnerView Create(ThrowableBallView template)
        {
            ThrowStation station = ThrowStation.Create(new Vector3(0.35f, 1.05f, 0.55f), 0.25f, Vector3.zero);
            var spawner = station.Root.gameObject.AddComponent<BallSpawnerView>();
            spawner.Configure(template, station.SpawnPoint);
            return spawner;
        }
    }
}
