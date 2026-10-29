using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Grab, launch, lifetime, and the thrown/resolved/destroyed signals. A booth adds its own scoring rule.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(XRGrabInteractable))]
    public abstract class ThrownPropView : MonoBehaviour, IThrownProp
    {
        [SerializeField] float lifetimeSeconds = 8f;

        Rigidbody _rigidbody;
        XRGrabInteractable _grabInteractable;
        HeldThrowRelease _release;
        bool _throwReported;
        bool _isThrown;
        bool _resolved;
        bool _startKinematic;
        bool _startGravity;
        float _despawnAt = -1f;
        Action<ThrownPropView> _returnToPool;

        public event Action<IThrownProp> Thrown;
        public event Action<IThrownProp> Resolved;
        public event Action<IThrownProp> Destroyed;
        public bool IsThrown => _isThrown;

        /// <summary>
        /// A release below this booth's threshold does not count as a throw. Launch() always counts.
        /// </summary>
        protected virtual bool CountsAsThrow(Vector3 velocity) => true;

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _grabInteractable = GetComponent<XRGrabInteractable>();
            _release = GetComponent<HeldThrowRelease>();
            _startKinematic = _rigidbody.isKinematic;
            _startGravity = _rigidbody.useGravity;
            HeldThrowGrab.Apply(_grabInteractable);
            _grabInteractable.selectExited.AddListener(OnSelectExited);
        }

        void OnDestroy()
        {
            if (_grabInteractable != null)
                _grabInteractable.selectExited.RemoveListener(OnSelectExited);

            NotifyResolved();
            Destroyed?.Invoke(this);
        }

        void Update()
        {
            if (_despawnAt > 0f && Time.time >= _despawnAt)
                ResolveAndDestroy();
        }

        public void UsePool(Action<ThrownPropView> returnToPool) => _returnToPool = returnToPool;

        public void Rearm(Vector3 position, Quaternion rotation)
        {
            _throwReported = false;
            _isThrown = false;
            _resolved = false;
            _despawnAt = -1f;
            transform.SetPositionAndRotation(position, rotation);
            _rigidbody.isKinematic = _startKinematic;
            _rigidbody.useGravity = _startGravity;
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _grabInteractable.enabled = true;
            gameObject.SetActive(true);
        }

        public void Launch(Vector3 velocity)
        {
            if (_isThrown)
                return;

            PreparePhysicsLaunch(velocity);
            MarkThrown();
        }

        protected void ResolveAndDestroy()
        {
            NotifyResolved();
            if (_returnToPool != null)
            {
                _returnToPool(this);
                return;
            }

            Destroy(gameObject);
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

            _release?.TryApply(args);
            if (!CountsAsThrow(_rigidbody.linearVelocity))
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
