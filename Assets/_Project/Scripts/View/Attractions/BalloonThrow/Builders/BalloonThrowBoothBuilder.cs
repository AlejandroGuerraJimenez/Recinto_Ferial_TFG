using Fairground.View.Attractions.BalloonThrow.Factories;
using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow.Builders
{
    /// <summary>
    /// Builder: assembles booth environment + gameplay objects.
    /// </summary>
    public sealed class BalloonThrowBoothBuilder
    {
        readonly BoothPropFactory _boothFactory = new BoothPropFactory();
        readonly ThrowableBallFactory _ballFactory = new ThrowableBallFactory();
        readonly BalloonTargetFactory _balloonFactory = new BalloonTargetFactory();
        readonly BallSpawnerFactory _spawnerFactory = new BallSpawnerFactory();
        readonly BalloonThrowHudFactory _hudFactory = new BalloonThrowHudFactory();
        readonly XrAttractionRigFactory _rigFactory;

        GameObject _xrOrigin;
        GameObject _environment;
        BalloonTargetView[] _balloons;
        BallSpawnerView _spawner;
        BalloonThrowHudView _hud;

        public BalloonThrowBoothBuilder(GameObject xrOriginPrefab)
        {
            _rigFactory = new XrAttractionRigFactory(xrOriginPrefab);
        }

        public GameObject XrOrigin => _xrOrigin;
        public GameObject Environment => _environment;
        public BalloonTargetView[] Balloons => _balloons;
        public BallSpawnerView Spawner => _spawner;
        public BalloonThrowHudView Hud => _hud;

        /// <summary>
        /// Lighting only — booth mesh already present in the baked scene.
        /// </summary>
        public BalloonThrowBoothBuilder UseBakedEnvironment(Transform bakedRoot)
        {
            _boothFactory.EnsureLighting();
            _environment = bakedRoot != null ? bakedRoot.gameObject : null;
            return this;
        }

        /// <summary>
        /// Instantiates a prebuilt booth prefab, or falls back to CreatePrimitive factory.
        /// </summary>
        public BalloonThrowBoothBuilder BuildEnvironment(GameObject boothEnvironmentPrefab)
        {
            _boothFactory.EnsureLighting();
            _environment = boothEnvironmentPrefab != null
                ? Object.Instantiate(boothEnvironmentPrefab)
                : _boothFactory.CreateEnvironment();

            if (_environment != null)
                _environment.name = BoothPropFactory.EnvironmentRootName;

            return this;
        }

        public BalloonThrowBoothBuilder BuildXrRig()
        {
            _xrOrigin = _rigFactory.EnsureXrOrigin();
            _rigFactory.EnsureInteractionManager();
            _rigFactory.EnsureEventSystem();
            return this;
        }

        public BalloonThrowBoothBuilder BuildGameplay(int balloonCount)
        {
            _balloons = _balloonFactory.CreateRow(balloonCount);
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
