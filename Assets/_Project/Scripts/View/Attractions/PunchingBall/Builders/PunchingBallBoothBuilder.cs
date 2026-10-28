using Fairground.View.Attractions.PunchingBall.Factories;
using UnityEngine;

namespace Fairground.View.Attractions.PunchingBall.Builders
{
    /// <summary>
    /// Punch pad and power meter. The shared shell builds the booth around them.
    /// </summary>
    public sealed class PunchingBallBoothBuilder : BoothBuilder
    {
        readonly PunchPadFactory _padFactory = new PunchPadFactory();
        readonly PunchPowerMeterFactory _meterFactory = new PunchPowerMeterFactory();
        readonly ControllerFistFactory _fistFactory = new ControllerFistFactory();

        public PunchPadView Pad { get; private set; }

        public PunchPowerMeterView Meter { get; private set; }

        public PunchingBallBoothBuilder(GameObject xrOriginPrefab)
            : base(PunchingBallBoothLayout.Props(), xrOriginPrefab)
        {
        }

        protected override void OnRigReady(GameObject xrOrigin)
        {
            _fistFactory.AttachFists(xrOrigin);
        }

        protected override void BuildGameplay()
        {
            Pad = _padFactory.Create();
            Track(Pad != null ? Pad.transform.root : null);

            Meter = _meterFactory.Create();
            Track(Meter != null ? Meter.transform.root : null);
        }
    }
}
