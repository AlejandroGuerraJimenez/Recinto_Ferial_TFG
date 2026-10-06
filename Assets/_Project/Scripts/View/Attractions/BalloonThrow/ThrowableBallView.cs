using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Fairground.View.Attractions.BalloonThrow
{
    /// <summary>
    /// XR-grabbable ball with throw detection (Observer: <see cref="Thrown"/>).
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(XRGrabInteractable))]
    public sealed class ThrowableBallView : MonoBehaviour
    {
        [SerializeField] float minThrowSpeed = 0.75f;
        [SerializeField] float lifetimeSeconds = 8f;

        Rigidbody _rigidbody;
        XRGrabInteractable _grabInteractable;
        bool _throwReported;
        bool _isThrown;
        float _despawnAt = -1f;

        public event Action<ThrowableBallView> Thrown;
        public bool IsThrown => _isThrown;

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
        }

        void Update()
        {
            if (_despawnAt > 0f && Time.time >= _despawnAt)
                Destroy(gameObject);
        }

        public void Launch(Vector3 velocity)
        {
            if (_isThrown)
                return;

            PreparePhysicsLaunch(velocity);
            MarkThrown();
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

            if (_rigidbody.linearVelocity.magnitude < minThrowSpeed)
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
    }
}
