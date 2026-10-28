using Fairground.View.Attractions.BasketballHoop.Factories;
using UnityEngine;

namespace Fairground.View.Attractions.BasketballHoop.Builders
{
    /// <summary>
    /// Assembles the hoop, the ball spawner, and the shared booth shell.
    /// </summary>
    public sealed class BasketballHoopBoothBuilder
    {
        readonly HoopFactory _hoopFactory = new HoopFactory();
        readonly BasketballBallFactory _ballFactory = new BasketballBallFactory();
        readonly BallSpawnerFactory _spawnerFactory = new BallSpawnerFactory();
        readonly BoothHudFactory _hudFactory = new BoothHudFactory();
        readonly BoothAssembly _shell;

        BasketDetectorView _hoop;
        BallSpawnerView _spawner;
        BoothHudView _hud;

        public BasketballHoopBoothBuilder(GameObject xrOriginPrefab)
        {
            _shell = new BoothAssembly(
                BasketballHoopBoothLayout.Props(),
                xrOriginPrefab,
                BasketballHoopBoothLayout.LogTag,
                BasketballHoopBoothLayout.Sky);
        }

        public GameObject XrOrigin => _shell.XrOrigin;
        public BasketDetectorView Hoop => _hoop;
        public BallSpawnerView Spawner => _spawner;
        public BoothHudView Hud => _hud;

        public BasketballHoopBoothBuilder UseBakedEnvironment(Transform bakedRoot)
        {
            _shell.UseBakedEnvironment(bakedRoot);
            return this;
        }

        public BasketballHoopBoothBuilder BuildEnvironment(GameObject boothEnvironmentPrefab)
        {
            _shell.BuildEnvironment(boothEnvironmentPrefab);
            return this;
        }

        public BasketballHoopBoothBuilder BuildXrRig()
        {
            _shell.BuildXrRig();
            return this;
        }

        public BasketballHoopBoothBuilder BuildGameplay()
        {
            _hoop = _hoopFactory.Create();
            _spawner = _spawnerFactory.Create(_ballFactory.CreateTemplate());
            _hud = _hudFactory.Create(BasketballHoopLayout.HudPosition, BasketballHoopBoothLayout.HudLines);
            return this;
        }

        public void EnsureBallReady()
        {
            if (_spawner != null && _spawner.CurrentBall == null)
                _spawner.TrySpawnImmediate();
        }

        public Transform[] BoothSpaceRoots()
        {
            Transform hoopRoot = null;
            if (_hoop != null)
            {
                hoopRoot = _hoop.transform;
                while (hoopRoot.parent != null)
                    hoopRoot = hoopRoot.parent;
            }

            return new[]
            {
                _shell.Environment != null ? _shell.Environment.transform : null,
                hoopRoot,
                _spawner != null ? _spawner.transform : null,
                _hud != null ? _hud.transform : null,
            };
        }
    }
}
