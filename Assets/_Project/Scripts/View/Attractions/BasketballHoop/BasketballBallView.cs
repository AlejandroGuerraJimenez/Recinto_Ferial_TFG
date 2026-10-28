using UnityEngine;

namespace Fairground.View.Attractions.BasketballHoop
{
    /// <summary>
    /// Throwable basketball. One shot can score at most once (<see cref="AlreadyScored"/>).
    /// Leaving the court resolves the shot.
    /// </summary>
    public sealed class BasketballBallView : ThrownPropView
    {
        bool _alreadyScored;

        public bool AlreadyScored => _alreadyScored;

        /// <summary>
        /// Marks this ball as having scored. Further calls return false.
        /// </summary>
        public bool TryMarkScored()
        {
            if (_alreadyScored || !IsThrown)
                return false;

            _alreadyScored = true;
            return true;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!IsThrown || collision.collider == null)
                return;

            if (collision.collider.GetComponent<BallOutOfPlayZone>() == null)
                return;

            ResolveAndDestroy();
        }
    }
}
