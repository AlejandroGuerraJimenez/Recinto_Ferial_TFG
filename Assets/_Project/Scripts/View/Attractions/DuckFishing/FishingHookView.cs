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
        [SerializeField] float reelSeconds = 0.85f;

        bool _dipping;
        bool _caughtThisDip;
        bool _reeling;
        bool _enabled = true;
        float _dipEndsAt;
        float _reelEndsAt;
        float _nextDipAt;
        DuckTargetView _hitched;
        Transform _tip;
        Vector3 _hitchOffset;

        public void AssignTip(Transform tip) => _tip = tip;

        Vector3 CatchPosition => _tip != null ? _tip.position : transform.position;

        public event Action DipStarted;
        public event Action DipResolved;
        public event Action<DuckTargetView> DuckCaught;
        public event Action<DuckTargetView> DuckSettled;

        public bool IsDipping => _dipping;
        public bool IsReeling => _reeling;
        public bool CanStartDip => _enabled && !_dipping && !_reeling && Time.time >= _nextDipAt;

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
            if (_reeling && Time.time >= _reelEndsAt)
                EndReel();

            if (!_dipping || Time.time < _dipEndsAt)
                return;

            FinishDip();
        }

        void LateUpdate()
        {
            if (_hitched == null)
                return;

            if (!_hitched.IsCaught)
            {
                _hitched = null;
                _reeling = false;
                return;
            }

            _hitched.Ride(CatchPosition + _hitchOffset);
        }

        void OnTriggerEnter(Collider other) => TryCatchCollider(other);

        void OnTriggerStay(Collider other) => TryCatchCollider(other);

        void TryCatchCollider(Collider other)
        {
            if (!_dipping || _caughtThisDip || other == null)
                return;

            var duck = other.GetComponentInParent<DuckTargetView>();
            TryCatch(duck);
        }

        public bool TryCatch(DuckTargetView duck)
        {
            if (!_dipping || _caughtThisDip || duck == null || duck.IsCaught)
                return false;

            if (!duck.TryCatch())
                return false;

            _caughtThisDip = true;
            Hitch(duck);
            DuckCaught?.Invoke(duck);
            FinishDip();
            return true;
        }

        void Hitch(DuckTargetView duck)
        {
            if (_hitched != null)
            {
                DuckTargetView previous = _hitched;
                _hitched = null;
                if (previous.IsCaught)
                    DuckSettled?.Invoke(previous);
            }

            _hitched = duck;
            _hitchOffset = duck.transform.position - duck.RingPosition;
            _reeling = true;
            _reelEndsAt = Time.time + reelSeconds;
        }

        void EndReel()
        {
            _reeling = false;
            if (_hitched == null)
                return;

            DuckTargetView duck = _hitched;
            _hitched = null;
            if (duck.IsCaught)
                DuckSettled?.Invoke(duck);
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
