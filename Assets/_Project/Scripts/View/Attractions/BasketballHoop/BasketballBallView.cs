using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Fairground.View.Attractions.BasketballHoop
{
    /// <summary>
    /// XR-grabbable basketball. One shot can score at most once (<see cref="AlreadyScored"/>).
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(XRGrabInteractable))]
    public sealed class BasketballBallView : MonoBehaviour
    {
        [SerializeField] float lifetimeSeconds = 8f;

        Rigidbody _rigidbody;
        XRGrabInteractable _grabInteractable;
        bool _throwReported;
        bool _isThrown;
        bool _alreadyScored;
        bool _resolved;
        float _despawnAt = -1f;

        public event Action<BasketballBallView> Thrown;
        public event Action<BasketballBallView> Resolved;
        public bool IsThrown => _isThrown;
        public bool AlreadyScored => _alreadyScored;

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _grabInteractable = GetComponent<XRGrabInteractable>();
            _grabInteractable.throwOnDetach = true;
            _grabInteractable.selectExited.AddListener(OnSelectExited);
        }

        void OnDestroy()
        {
            if (_grabInteractable != null)
                _grabInteractable.selectExited.RemoveListener(OnSelectExited);
            NotifyResolved();
        }

        void Update()
        {
            if (_despawnAt > 0f && Time.time >= _despawnAt)
                ResolveAndDestroy();
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!_isThrown || collision.collider == null)
                return;

            if (collision.collider.GetComponent<BallOutOfPlayZone>() == null)
                return;

            ResolveAndDestroy();
        }

        public void Launch(Vector3 velocity)
        {
            if (_isThrown)
                return;

            PreparePhysicsLaunch(velocity);
            MarkThrown();
        }

        /// <summary>
        /// Marks this ball as having scored. Further calls return false.
        /// </summary>
        public bool TryMarkScored()
        {
            if (_alreadyScored || !_isThrown)
                return false;

            _alreadyScored = true;
            return true;
        }

        void PreparePhysicsLaunch(Vector3 velocity)
        {
            _grabInteractable.enabled = false;
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            _rigidbody.linearVelocity = velocity;
        }

        void OnSelectExited(SelectExitEventArgs args)
        {
            if (args.isCanceled || _throwReported)
                return;

            MarkThrown();
        }

        void MarkThrown()
        {
            if (_throwReported)
                return;

            _throwReported = true;
            _isThrown = true;
            _despawnAt = Time.time + lifetimeSeconds;
            Thrown?.Invoke(this);
        }

        void ResolveAndDestroy()
        {
            NotifyResolved();
            Destroy(gameObject);
        }

        void NotifyResolved()
        {
            if (!_isThrown || _resolved)
                return;

            _resolved = true;
            _isThrown = false;
            Resolved?.Invoke(this);
        }
    }
}
