using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow
{
    /// <summary>
    /// Throwable ball. A release slower than <see cref="minThrowSpeed"/> does not count.
    /// </summary>
    public sealed class ThrowableBallView : ThrownPropView
    {
        [SerializeField] float minThrowSpeed = 0.75f;

        protected override bool CountsAsThrow(Vector3 velocity) => velocity.magnitude >= minThrowSpeed;
    }
}
