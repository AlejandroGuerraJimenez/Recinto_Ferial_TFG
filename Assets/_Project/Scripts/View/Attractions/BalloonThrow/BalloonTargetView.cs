using System;
using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow
{
    /// <summary>
    /// Balloon target. Observer: <see cref="Popped"/>.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class BalloonTargetView : MonoBehaviour
    {
        [SerializeField] Renderer balloonRenderer;
        [SerializeField] Color poppedColor = new Color(1f, 0.85f, 0.2f, 1f);
        [SerializeField] float popScale = 1.35f;

        bool _popped;
        MaterialPropertyBlock _popTint;
        Vector3 _restScale;
        Collider _collider;

        public event Action<BalloonTargetView> Popped;
        public bool IsPopped => _popped;

        void Awake()
        {
            if (balloonRenderer == null)
                balloonRenderer = GetComponentInChildren<Renderer>();

            _restScale = transform.localScale;
            _collider = GetComponent<Collider>();
        }

        void OnCollisionEnter(Collision collision) => TryPop(collision.collider);

        void OnTriggerEnter(Collider other) => TryPop(other);

        /// <summary>
        /// Registers a hit from an already-thrown ball (PlayMode/tests and forced contacts).
        /// </summary>
        public bool TryRegisterPopFrom(ThrowableBallView ball)
        {
            if (_popped || ball == null || !ball.IsThrown)
                return false;

            ApplyPop();
            return true;
        }

        void TryPop(Collider other)
        {
            if (!CanPop(other))
                return;

            ApplyPop();
        }

        public void Rearm()
        {
            _popped = false;
            transform.localScale = _restScale;
            if (_collider != null)
                _collider.enabled = true;

            if (balloonRenderer == null)
                return;

            balloonRenderer.enabled = true;
            balloonRenderer.SetPropertyBlock(null);
        }

        void ApplyPop()
        {
            _popped = true;
            ApplyPopFeedback();
            Popped?.Invoke(this);
            if (_collider != null)
                _collider.enabled = false;

            if (balloonRenderer != null)
                balloonRenderer.enabled = false;
        }

        bool CanPop(Collider other)
        {
            if (_popped || other == null)
                return false;

            var ball = other.GetComponentInParent<ThrowableBallView>();
            return ball != null && ball.IsThrown;
        }

        void ApplyPopFeedback()
        {
            transform.localScale *= popScale;
            if (balloonRenderer == null)
                return;

            _popTint ??= new MaterialPropertyBlock();
            balloonRenderer.GetPropertyBlock(_popTint);
            _popTint.SetColor("_BaseColor", poppedColor);
            _popTint.SetColor("_Color", poppedColor);
            balloonRenderer.SetPropertyBlock(_popTint);
        }
    }
}
