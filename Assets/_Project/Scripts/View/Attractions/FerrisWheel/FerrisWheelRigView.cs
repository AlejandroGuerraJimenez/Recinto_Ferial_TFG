using UnityEngine;

namespace Fairground.View.Attractions.FerrisWheel
{
    /// <summary>
    /// Applies the ride angle. Cabins are children of the wheel and counter-rotate so they stay upright.
    /// </summary>
    public sealed class FerrisWheelRigView : MonoBehaviour
    {
        Transform _wheel;
        Transform[] _cabins;

        public Transform Wheel => _wheel;
        public int CabinCount => _cabins == null ? 0 : _cabins.Length;

        public void Configure(Transform wheel, Transform[] cabins)
        {
            _wheel = wheel;
            _cabins = cabins;
        }

        public Transform GetCabin(int index)
        {
            return _cabins[index];
        }

        public void ApplyAngle(float angleDegrees)
        {
            if (_wheel == null)
                return;

            _wheel.localRotation = Quaternion.Euler(0f, 0f, angleDegrees);
            if (_cabins == null)
                return;

            var upright = Quaternion.Euler(0f, 0f, -angleDegrees);
            for (int i = 0; i < _cabins.Length; i++)
            {
                if (_cabins[i] != null)
                    _cabins[i].localRotation = upright;
            }
        }
    }
}
