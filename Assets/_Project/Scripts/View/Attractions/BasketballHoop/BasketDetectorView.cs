using System;
using UnityEngine;

namespace Fairground.View.Attractions.BasketballHoop
{
    /// <summary>
    /// Rim trigger. Scores only downward entries and only once per ball.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class BasketDetectorView : MonoBehaviour
    {
        [SerializeField] float minDownwardSpeed = 0.25f;
        [SerializeField] float minDownwardAlignment = 0.05f;

        public event Action<BasketballBallView> Scored;

        void OnTriggerEnter(Collider other)
        {
            var ball = other.GetComponentInParent<BasketballBallView>();
            if (!IsDownwardEntry(ball))
                return;

            TryRegisterScoreFrom(ball);
        }

        /// <summary>
        /// Registers a basket from an already-thrown ball (PlayMode/tests and forced contacts).
        /// </summary>
        public bool TryRegisterScoreFrom(BasketballBallView ball)
        {
            if (ball == null || !ball.IsThrown)
                return false;

            if (!ball.TryMarkScored())
                return false;

            Scored?.Invoke(ball);
            return true;
        }

        bool IsDownwardEntry(BasketballBallView ball)
        {
            if (ball == null || ball.AlreadyScored || !ball.IsThrown)
                return false;

            var body = ball.GetComponent<Rigidbody>();
            if (body == null)
                return false;

            Vector3 velocity = body.linearVelocity;
            if (velocity.sqrMagnitude < 0.0001f)
                return false;

            return velocity.y <= -minDownwardSpeed
                   && Vector3.Dot(velocity.normalized, Vector3.down) >= minDownwardAlignment;
        }
    }
}
