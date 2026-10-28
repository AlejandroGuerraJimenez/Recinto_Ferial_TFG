using Fairground.View.Attractions.BalloonThrow.Factories;
using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow.Builders
{
    /// <summary>
    /// Assembles balloon targets, the ball spawner, and the shared booth shell.
    /// </summary>
    public sealed class BalloonThrowBoothBuilder
    {
        readonly ThrowableBallFactory _ballFactory = new ThrowableBallFactory();
        readonly BalloonTargetFactory _balloonFactory = new BalloonTargetFactory();
        readonly BallSpawnerFactory _spawnerFactory = new BallSpawnerFactory();
        readonly BoothHudFactory _hudFactory = new BoothHudFactory();
        readonly BoothAssembly _shell;

        BalloonTargetView[] _balloons;
        BallSpawnerView _spawner;
        BoothHudView _hud;

        public BalloonThrowBoothBuilder(GameObject xrOriginPrefab)
        {
            _shell = new BoothAssembly(
                BalloonThrowBoothLayout.Props(),
                xrOriginPrefab,
                BalloonThrowBoothLayout.LogTag,
                BalloonThrowBoothLayout.Sky);
        }

        public GameObject XrOrigin => _shell.XrOrigin;
        public BalloonTargetView[] Balloons => _balloons;
        public BallSpawnerView Spawner => _spawner;
        public BoothHudView Hud => _hud;

        public BalloonThrowBoothBuilder UseBakedEnvironment(Transform bakedRoot)
        {
            _shell.UseBakedEnvironment(bakedRoot);
            return this;
        }

        public BalloonThrowBoothBuilder BuildEnvironment(GameObject boothEnvironmentPrefab)
        {
            _shell.BuildEnvironment(boothEnvironmentPrefab);
            return this;
        }

        public BalloonThrowBoothBuilder BuildXrRig()
        {
            _shell.BuildXrRig();
            return this;
        }

        public BalloonThrowBoothBuilder BuildGameplay(int balloonCount)
        {
            _balloons = _balloonFactory.CreateRow(balloonCount);
            _spawner = _spawnerFactory.Create(_ballFactory.CreateTemplate());
            _hud = _hudFactory.Create(BalloonThrowBoothLayout.HudPosition, BalloonThrowBoothLayout.HudLines);
            return this;
        }

        public void EnsureBallReady()
        {
            if (_spawner != null && _spawner.CurrentBall == null)
                _spawner.TrySpawnImmediate();
        }

        public Transform[] BoothSpaceRoots()
        {
            Transform balloonsRoot = null;
            if (_balloons != null && _balloons.Length > 0 && _balloons[0] != null)
                balloonsRoot = _balloons[0].transform.parent;

            return new[]
            {
                _shell.Environment != null ? _shell.Environment.transform : null,
                balloonsRoot,
                _spawner != null ? _spawner.transform : null,
                _hud != null ? _hud.transform : null,
            };
        }
    }
}
