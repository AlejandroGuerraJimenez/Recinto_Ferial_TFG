using Fairground.View.Attractions.PunchingBall.Factories;
using UnityEngine;

namespace Fairground.View.Attractions.PunchingBall.Builders
{
    /// <summary>
    /// Assembles the punch pad, the meter, and the shared booth shell.
    /// </summary>
    public sealed class PunchingBallBoothBuilder
    {
        readonly PunchPadFactory _padFactory = new PunchPadFactory();
        readonly PunchPowerMeterFactory _meterFactory = new PunchPowerMeterFactory();
        readonly ControllerFistFactory _fistFactory = new ControllerFistFactory();
        readonly BoothHudFactory _hudFactory = new BoothHudFactory();
        readonly BoothAssembly _shell;

        PunchPadView _pad;
        PunchPowerMeterView _meter;
        BoothHudView _hud;

        public PunchingBallBoothBuilder(GameObject xrOriginPrefab)
        {
            _shell = new BoothAssembly(
                PunchingBallBoothLayout.Props(),
                xrOriginPrefab,
                PunchingBallBoothLayout.LogTag,
                PunchingBallBoothLayout.Sky);
        }

        public GameObject XrOrigin => _shell.XrOrigin;
        public PunchPadView Pad => _pad;
        public PunchPowerMeterView Meter => _meter;
        public BoothHudView Hud => _hud;

        public PunchingBallBoothBuilder UseBakedEnvironment(Transform bakedRoot)
        {
            _shell.UseBakedEnvironment(bakedRoot);
            return this;
        }

        public PunchingBallBoothBuilder BuildEnvironment(GameObject boothEnvironmentPrefab)
        {
            _shell.BuildEnvironment(boothEnvironmentPrefab);
            return this;
        }

        public PunchingBallBoothBuilder BuildXrRig()
        {
            _shell.BuildXrRig(_fistFactory.AttachFists);
            return this;
        }

        public PunchingBallBoothBuilder BuildGameplay()
        {
            _pad = _padFactory.Create();
            _meter = _meterFactory.Create();
            _hud = _hudFactory.Create(PunchingBallBoothLayout.HudPosition, PunchingBallBoothLayout.HudLines);
            return this;
        }

        public Transform[] BoothSpaceRoots()
        {
            return new[]
            {
                _shell.Environment != null ? _shell.Environment.transform : null,
                _pad != null ? _pad.transform.root : null,
                _meter != null ? _meter.transform.root : null,
                _hud != null ? _hud.transform : null,
            };
        }
    }
}
