using Fairground.View.Attractions.BasketballHoop.Factories;
using UnityEngine;

namespace Fairground.View.Attractions.BasketballHoop.Builders
{
    /// <summary>
    /// Builder: assembles booth environment + gameplay objects.
    /// </summary>
    public sealed class BasketballHoopBoothBuilder
    {
        readonly BoothPropFactory _boothFactory = new BoothPropFactory();
        readonly HoopFactory _hoopFactory = new HoopFactory();
        readonly BasketballBallFactory _ballFactory = new BasketballBallFactory();
        readonly BallSpawnerFactory _spawnerFactory = new BallSpawnerFactory();
        readonly BasketballHoopHudFactory _hudFactory = new BasketballHoopHudFactory();
        readonly XrAttractionRigFactory _rigFactory;

        GameObject _xrOrigin;
        GameObject _environment;
        BasketDetectorView _hoop;
        BallSpawnerView _spawner;
        BasketballHoopHudView _hud;

        public BasketballHoopBoothBuilder(GameObject xrOriginPrefab)
        {
            _rigFactory = new XrAttractionRigFactory(xrOriginPrefab);
        }

        public GameObject XrOrigin => _xrOrigin;
        public GameObject Environment => _environment;
        public BasketDetectorView Hoop => _hoop;
        public BallSpawnerView Spawner => _spawner;
        public BasketballHoopHudView Hud => _hud;

        /// <summary>
        /// Lighting only — booth mesh already present in the baked scene.
        /// </summary>
        public BasketballHoopBoothBuilder UseBakedEnvironment(Transform bakedRoot)
        {
            _boothFactory.EnsureLighting();
            _environment = bakedRoot != null ? bakedRoot.gameObject : null;
            return this;
        }

        /// <summary>
        /// Instantiates a prebuilt booth prefab, or falls back to CreatePrimitive factory.
        /// </summary>
        public BasketballHoopBoothBuilder BuildEnvironment(GameObject boothEnvironmentPrefab)
        {
            _boothFactory.EnsureLighting();
            _environment = boothEnvironmentPrefab != null
                ? Object.Instantiate(boothEnvironmentPrefab)
                : _boothFactory.CreateEnvironment();

            if (_environment != null)
                _environment.name = BoothPropFactory.EnvironmentRootName;

            return this;
        }

        public BasketballHoopBoothBuilder BuildXrRig()
        {
            _xrOrigin = _rigFactory.EnsureXrOrigin();
            _rigFactory.EnsureInteractionManager();
            _rigFactory.EnsureEventSystem();
            return this;
        }

        public BasketballHoopBoothBuilder BuildGameplay()
        {
            _hoop = _hoopFactory.Create();
            _spawner = _spawnerFactory.Create(_ballFactory.CreateTemplate());
            _hud = _hudFactory.Create();
            return this;
        }

        public void EnsureBallReady()
        {
            if (_spawner != null && _spawner.CurrentBall == null)
                _spawner.TrySpawnImmediate();
        }
    }
}
