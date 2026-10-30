using System;
using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow
{
    /// <summary>
    /// Balloon target. Observer: <see cref="Popped"/>.
    /// A hit hides every ring. The collider stays on this object so the bullseye still catches the dart.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class BalloonTargetView : MonoBehaviour
    {
        [SerializeField] float popScale = 1.35f;

        bool _popped;
        Vector3 _restScale;
        Collider _collider;
        Renderer[] _parts;
        bool[] _drawn;

        public event Action<BalloonTargetView> Popped;
        public bool IsPopped => _popped;

        void Awake()
        {
            _restScale = transform.localScale;
            _collider = GetComponent<Collider>();
            _parts = GetComponentsInChildren<Renderer>(true);
            _drawn = new bool[_parts.Length];
            for (int i = 0; i < _parts.Length; i++)
                _drawn[i] = _parts[i] != null && _parts[i].enabled;
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

            SetDrawn(true);
        }

        void ApplyPop()
        {
            _popped = true;
            transform.localScale = _restScale * popScale;
            Popped?.Invoke(this);
            if (_collider != null)
                _collider.enabled = false;

            SetDrawn(false);
        }

        bool CanPop(Collider other)
        {
            if (_popped || other == null)
                return false;

            var ball = other.GetComponentInParent<ThrowableBallView>();
            return ball != null && ball.IsThrown;
        }

        void SetDrawn(bool visible)
        {
            if (_parts == null)
                return;

            for (int i = 0; i < _parts.Length; i++)
            {
                if (_parts[i] != null)
                    _parts[i].enabled = visible && _drawn[i];
            }
        }
    }
}
