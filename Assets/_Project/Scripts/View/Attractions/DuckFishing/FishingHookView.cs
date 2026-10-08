using System;
using UnityEngine;

namespace Fairground.View.Attractions.DuckFishing
{
    /// <summary>
    /// Hook tip that can catch ducks while dipping. Observer: Dip/Catch events.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class FishingHookView : MonoBehaviour
    {
        [SerializeField] float dipDuration = 0.55f;
        [SerializeField] float cooldownSeconds = 0.2f;

        bool _dipping;
        bool _caughtThisDip;
        bool _enabled = true;
        float _dipEndsAt;
        float _nextDipAt;

        public event Action DipStarted;
        public event Action DipResolved;
        public event Action<DuckTargetView> DuckCaught;

        public bool IsDipping => _dipping;
        public bool CanStartDip => _enabled && !_dipping && Time.time >= _nextDipAt;

        public void SetEnabled(bool enabled) => _enabled = enabled;

        public bool TryStartDip()
        {
            if (!CanStartDip)
                return false;

            _dipping = true;
            _caughtThisDip = false;
            _dipEndsAt = Time.time + dipDuration;
            DipStarted?.Invoke();
            return true;
        }

        void Update()
        {
            if (!_dipping)
                return;

            if (Time.time < _dipEndsAt)
                return;

            FinishDip();
        }

        void OnTriggerEnter(Collider other) => TryCatchCollider(other);

        void OnTriggerStay(Collider other) => TryCatchCollider(other);

        void TryCatchCollider(Collider other)
        {
            if (!_dipping || _caughtThisDip || other == null)
                return;

            var duck = other.GetComponentInParent<DuckTargetView>();
            if (duck == null || duck.IsCaught)
                return;

            if (!duck.TryCatch())
                return;

            _caughtThisDip = true;
            DuckCaught?.Invoke(duck);
            FinishDip();
        }

        void FinishDip()
        {
            if (!_dipping)
                return;

            _dipping = false;
            _nextDipAt = Time.time + cooldownSeconds;
            DipResolved?.Invoke();
        }
    }
}
