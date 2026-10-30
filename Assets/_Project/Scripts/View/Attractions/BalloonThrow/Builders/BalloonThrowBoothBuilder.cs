using Fairground.Core.Attractions;
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
        readonly BombTargetFactory _bombFactory = new BombTargetFactory();

        public BalloonTargetView[] Balloons { get; private set; }

        public BombTargetView[] Bombs { get; private set; }

        public BallSpawnerView Spawner { get; private set; }

        public BalloonThrowBoothBuilder(GameObject xrOriginPrefab)
            : base(BalloonThrowBoothLayout.Props(), xrOriginPrefab)
        {
        }

        protected override void OnEnvironmentReady(Transform environmentRoot)
        {
            Transform skull = environmentRoot != null ? environmentRoot.Find("Skull") : null;
            if (skull != null)
                skull.gameObject.SetActive(false);
        }

        protected override void BuildGameplay()
        {
            Balloons = _balloonFactory.CreateRow(BalloonThrowBalance.BalloonCount);
            if (Balloons != null && Balloons.Length > 0 && Balloons[0] != null)
                Track(Balloons[0].transform.parent);

            Bombs = _bombFactory.CreateAll();
            if (Bombs != null && Bombs.Length > 0 && Bombs[0] != null)
                Track(Bombs[0].transform.parent);

            Spawner = ThrownPropSpawnerFactory.Create(
                _ballFactory.CreateTemplate(),
                DartStump.Create(
                    BalloonThrowBoothLayout.SpawnerPosition,
                    BalloonThrowBoothLayout.SpawnerLocalOffset));
            Track(Spawner != null ? Spawner.transform : null);
        }

        public void EnsureBallReady()
        {
            if (Spawner != null && Spawner.CurrentBall == null)
                Spawner.TrySpawnImmediate();
        }
    }
}
