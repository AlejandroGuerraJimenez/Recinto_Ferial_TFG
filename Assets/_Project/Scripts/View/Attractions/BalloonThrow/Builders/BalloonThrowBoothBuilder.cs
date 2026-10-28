using Fairground.View.Attractions.BalloonThrow.Factories;
using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow.Builders
{
    /// <summary>
    /// Balloon targets and the ball spawner. The shared shell builds the booth around them.
    /// </summary>
    public sealed class BalloonThrowBoothBuilder : BoothBuilder
    {
        readonly ThrowableBallFactory _ballFactory = new ThrowableBallFactory();
        readonly BalloonTargetFactory _balloonFactory = new BalloonTargetFactory();
        readonly BallSpawnerFactory _spawnerFactory = new BallSpawnerFactory();
        readonly int _balloonCount;

        public BalloonTargetView[] Balloons { get; private set; }

        public BallSpawnerView Spawner { get; private set; }

        public BalloonThrowBoothBuilder(GameObject xrOriginPrefab, int balloonCount)
            : base(BalloonThrowBoothLayout.Props(), xrOriginPrefab)
        {
            _balloonCount = balloonCount;
        }

        protected override void BuildGameplay()
        {
            Balloons = _balloonFactory.CreateRow(_balloonCount);
            if (Balloons != null && Balloons.Length > 0 && Balloons[0] != null)
                Track(Balloons[0].transform.parent);

            Spawner = _spawnerFactory.Create(_ballFactory.CreateTemplate());
            Track(Spawner != null ? Spawner.transform : null);
        }

        public void EnsureBallReady()
        {
            if (Spawner != null && Spawner.CurrentBall == null)
                Spawner.TrySpawnImmediate();
        }
    }
}
