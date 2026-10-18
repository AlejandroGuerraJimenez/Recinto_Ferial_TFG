using Fairground.View.Attractions.PunchingBall.Factories;
using UnityEngine;

namespace Fairground.View.Attractions.PunchingBall.Builders
{
    /// <summary>
    /// Builder: assembles booth environment + gameplay objects.
    /// </summary>
    public sealed class PunchingBallBoothBuilder
    {
        readonly BoothPropFactory _boothFactory = new BoothPropFactory();
        readonly PunchPadFactory _padFactory = new PunchPadFactory();
        readonly PunchPowerMeterFactory _meterFactory = new PunchPowerMeterFactory();
        readonly PunchingBallHudFactory _hudFactory = new PunchingBallHudFactory();
        readonly ControllerFistFactory _fistFactory = new ControllerFistFactory();
        readonly XrAttractionRigFactory _rigFactory;

        GameObject _xrOrigin;
        GameObject _environment;
        PunchPadView _pad;
        PunchPowerMeterView _meter;
        PunchingBallHudView _hud;

        public PunchingBallBoothBuilder(GameObject xrOriginPrefab)
        {
            _rigFactory = new XrAttractionRigFactory(xrOriginPrefab);
        }

        public GameObject XrOrigin => _xrOrigin;
        public GameObject Environment => _environment;
        public PunchPadView Pad => _pad;
        public PunchPowerMeterView Meter => _meter;
        public PunchingBallHudView Hud => _hud;

        /// <summary>
        /// Lighting only — booth mesh already present in the baked scene.
        /// </summary>
        public PunchingBallBoothBuilder UseBakedEnvironment(Transform bakedRoot)
        {
            _boothFactory.EnsureLighting();
            _environment = bakedRoot != null ? bakedRoot.gameObject : null;
            return this;
        }

        /// <summary>
        /// Instantiates a prebuilt booth prefab, or falls back to CreatePrimitive factory.
        /// </summary>
        public PunchingBallBoothBuilder BuildEnvironment(GameObject boothEnvironmentPrefab)
        {
            _boothFactory.EnsureLighting();
            _environment = boothEnvironmentPrefab != null
                ? Object.Instantiate(boothEnvironmentPrefab)
                : _boothFactory.CreateEnvironment();

            if (_environment != null)
                _environment.name = BoothPropFactory.EnvironmentRootName;

            return this;
        }

        public PunchingBallBoothBuilder BuildXrRig()
        {
            _xrOrigin = _rigFactory.EnsureXrOrigin();
            _rigFactory.EnsureInteractionManager();
            _rigFactory.EnsureEventSystem();
            _fistFactory.AttachFists(_xrOrigin);
            return this;
        }

        public PunchingBallBoothBuilder BuildGameplay()
        {
            _pad = _padFactory.Create();
            _meter = _meterFactory.Create();
            _hud = _hudFactory.Create();
            return this;
        }
    }
}
