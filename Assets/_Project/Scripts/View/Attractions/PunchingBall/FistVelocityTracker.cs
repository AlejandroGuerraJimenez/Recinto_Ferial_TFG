using UnityEngine;

namespace Fairground.View.Attractions.PunchingBall
{
    /// <summary>
    /// Samples world-space speed for XR controller fists (kinematic bodies).
    /// </summary>
    public sealed class FistVelocityTracker : MonoBehaviour
    {
        Vector3 _previousPosition;
        float _speed;
        bool _initialized;

        public float Speed => _speed;

        void LateUpdate()
        {
            Vector3 position = transform.position;
            if (!_initialized)
            {
                _previousPosition = position;
                _initialized = true;
                return;
            }

            float dt = Time.deltaTime;
            _speed = dt > 0f ? (position - _previousPosition).magnitude / dt : 0f;
            _previousPosition = position;
        }
    }
}
