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
        PunchPadView _pad;
        PunchPowerMeterView _meter;
        PunchingBallHudView _hud;

        public PunchingBallBoothBuilder(GameObject xrOriginPrefab)
        {
            _rigFactory = new XrAttractionRigFactory(xrOriginPrefab);
        }

        public GameObject XrOrigin => _xrOrigin;
        public PunchPadView Pad => _pad;
        public PunchPowerMeterView Meter => _meter;
        public PunchingBallHudView Hud => _hud;

        public PunchingBallBoothBuilder BuildEnvironment()
        {
            _boothFactory.EnsureLighting();
            _boothFactory.CreateGround();
            _boothFactory.CreateBoothBack();
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
