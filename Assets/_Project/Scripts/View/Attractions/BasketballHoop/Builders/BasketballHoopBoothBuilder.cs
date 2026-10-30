using Fairground.View.Attractions.BasketballHoop.Factories;
using UnityEngine;

namespace Fairground.View.Attractions.BasketballHoop.Builders
{
    /// <summary>
    /// Hoop and ball spawner. The shared shell builds the booth around them.
    /// </summary>
    public sealed class BasketballHoopBoothBuilder : BoothBuilder
    {
        readonly HoopFactory _hoopFactory = new HoopFactory();
        readonly BasketballBallFactory _ballFactory = new BasketballBallFactory();

        public BasketDetectorView Hoop { get; private set; }

        public BallSpawnerView Spawner { get; private set; }

        public BasketballHoopBoothBuilder(GameObject xrOriginPrefab)
            : base(BasketballHoopBoothLayout.Props(), xrOriginPrefab)
        {
        }

        protected override void BuildGameplay()
        {
            Hoop = _hoopFactory.Create();
            Track(Top(Hoop != null ? Hoop.transform : null));

            Spawner = ThrownPropSpawnerFactory.Create(
                _ballFactory.CreateTemplate(),
                LogStump.Create(
                    BasketballHoopLayout.SpawnerPosition,
                    BasketballHoopLayout.SpawnerLocalOffset,
                    BasketballHoopBoothLayout.Wood,
                    BasketballHoopBoothLayout.LogTag,
                    BasketballHoopLayout.SpawnerPedestalRadius,
                    BasketballHoopLayout.SpawnerLocalOffset));
            Track(Spawner != null ? Spawner.transform : null);
        }

        public void EnsureBallReady()
        {
            if (Spawner != null && Spawner.CurrentBall == null)
                Spawner.TrySpawnImmediate();
        }

        static Transform Top(Transform node)
        {
            if (node == null)
                return null;

            while (node.parent != null)
                node = node.parent;

            return node;
        }
    }
}
