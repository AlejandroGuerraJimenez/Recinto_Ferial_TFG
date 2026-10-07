using UnityEngine;

namespace Fairground.View.Attractions.PunchingBall
{
    /// <summary>
    /// Visual strength tower: puck rises with last punch power.
    /// </summary>
    public sealed class PunchPowerMeterView : MonoBehaviour
    {
        [SerializeField] Transform puck;
        [SerializeField] float minLocalY = 0.15f;
        [SerializeField] float maxLocalY = 2.4f;
        [SerializeField] float moveSpeed = 6f;

        float _targetLocalY;

        public void Configure(Transform meterPuck, float minY, float maxY)
        {
            puck = meterPuck;
            minLocalY = minY;
            maxLocalY = maxY;
            _targetLocalY = minLocalY;
            SnapToTarget();
        }

        public void SetPower(int power, int maxScore)
        {
            float t = maxScore > 0 ? Mathf.Clamp01(power / (float)maxScore) : 0f;
            _targetLocalY = Mathf.Lerp(minLocalY, maxLocalY, t);
        }

        public void ResetMeter()
        {
            _targetLocalY = minLocalY;
            SnapToTarget();
        }

        void Update()
        {
            if (puck == null)
                return;

            Vector3 pos = puck.localPosition;
            pos.y = Mathf.Lerp(pos.y, _targetLocalY, Time.deltaTime * moveSpeed);
            puck.localPosition = pos;
        }

        void SnapToTarget()
        {
            if (puck == null)
                return;

            Vector3 pos = puck.localPosition;
            pos.y = _targetLocalY;
            puck.localPosition = pos;
        }
    }
}
