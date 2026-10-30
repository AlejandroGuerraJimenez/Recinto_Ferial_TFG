using System;
using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow
{
    /// <summary>
    /// Cartoon bomb. A thrown dart blows it up and the match hears <see cref="Exploded"/>.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class BombTargetView : MonoBehaviour
    {
        bool _exploded;
        Vector3 _restScale;
        Collider _collider;
        Renderer[] _parts;
        bool[] _drawn;
        GameObject _burst;

        public event Action<BombTargetView> Exploded;
        public bool IsExploded => _exploded;

        void Awake()
        {
            _restScale = transform.localScale;
            _collider = GetComponent<Collider>();
            CacheParts();
        }

        void OnCollisionEnter(Collision collision) => TryExplode(collision.collider);

        void OnTriggerEnter(Collider other) => TryExplode(other);

        public bool TryRegisterHitFrom(ThrowableBallView dart)
        {
            if (_exploded || dart == null || !dart.IsThrown)
                return false;

            ApplyExplosion();
            return true;
        }

        public void Rearm()
        {
            _exploded = false;
            transform.localScale = _restScale;
            if (_burst != null)
                Destroy(_burst);

            if (_collider != null)
                _collider.enabled = true;

            SetDrawn(true);
        }

        void TryExplode(Collider other)
        {
            if (_exploded || other == null)
                return;

            var dart = other.GetComponentInParent<ThrowableBallView>();
            if (dart == null || !dart.IsThrown)
                return;

            ApplyExplosion();
        }

        void ApplyExplosion()
        {
            _exploded = true;
            if (_collider != null)
                _collider.enabled = false;

            SetDrawn(false);
            if (_burst != null)
                Destroy(_burst);

            _burst = BombBurst.Create(transform.position);
            Exploded?.Invoke(this);
        }

        void CacheParts()
        {
            _parts = GetComponentsInChildren<Renderer>(true);
            _drawn = new bool[_parts.Length];
            for (int i = 0; i < _parts.Length; i++)
                _drawn[i] = _parts[i] != null && _parts[i].enabled;
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
