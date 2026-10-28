using UnityEngine;

namespace Fairground.View.Attractions.PunchingBall
{
    /// <summary>
    /// Editor punch: hold the play button to charge, release to hit the pad.
    /// </summary>
    public static class PunchingBallDesktopPlay
    {
        const float ChargePerSecond = 1.4f;
        const float MaxImpactSpeed = 8f;

        public static DesktopPlayProfile Profile(PunchPadView pad)
        {
            var charge = new Charge(pad);
            return new DesktopPlayProfile
            {
                FallbackSky = PunchingBallBoothLayout.Sky,
                EyePosition = new Vector3(0f, 1.5f, 0.4f),
                OnFrame = (_, input) => charge.Tick(input),
            };
        }

        sealed class Charge
        {
            readonly PunchPadView _pad;
            float _charge;
            bool _charging;

            public Charge(PunchPadView pad)
            {
                _pad = pad;
            }

            public void Tick(AttractionInput input)
            {
                if (input == null)
                    return;

                if (input.PlayHeld)
                {
                    _charging = true;
                    _charge = Mathf.Clamp01(_charge + ChargePerSecond * Time.deltaTime);
                    return;
                }

                if (!_charging)
                    return;

                float impact = Mathf.Lerp(1.5f, MaxImpactSpeed, _charge);
                _pad?.TryApplyImpact(impact);
                _charge = 0f;
                _charging = false;
            }
        }
    }
}
