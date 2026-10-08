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
        BasketDetectorView _hoop;
        BallSpawnerView _spawner;
        BasketballHoopHudView _hud;

        public BasketballHoopBoothBuilder(GameObject xrOriginPrefab)
        {
            _rigFactory = new XrAttractionRigFactory(xrOriginPrefab);
        }

        public GameObject XrOrigin => _xrOrigin;
        public BasketDetectorView Hoop => _hoop;
        public BallSpawnerView Spawner => _spawner;
        public BasketballHoopHudView Hud => _hud;

        public BasketballHoopBoothBuilder BuildEnvironment()
        {
            _boothFactory.EnsureLighting();
            _boothFactory.CreateGround();
            _boothFactory.CreateArcadeMachine();
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
