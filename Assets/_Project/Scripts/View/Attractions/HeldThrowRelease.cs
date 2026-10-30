using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Applies release speed from the fastest part of the swing.
    /// The toolkit averages the last frames, which are already slow when the grip opens,
    /// so a real throw falls short of the booth.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(XRGrabInteractable))]
    public sealed class HeldThrowRelease : MonoBehaviour
    {
        const int Capacity = 48;
        const float IgnoreGrabSeconds = 0.12f;
        const float WindowSeconds = 0.2f;
        const float MinSpanSeconds = 0.03f;
        const float MaxSpanSeconds = 0.1f;

        [SerializeField] float velocityScale = 3.2f;
        [SerializeField] float maxSpeed = 13f;
        [SerializeField] float minGestureSpeed = 0.85f;
        [SerializeField] float arc = 0f;

        readonly Sample[] _samples = new Sample[Capacity];
        XRGrabInteractable _grab;
        Rigidbody _body;
        int _count;
        float _sampleAfter;

        public void Configure(float scale, float speedLimit, float upwardArc)
        {
            velocityScale = Mathf.Max(0.1f, scale);
            maxSpeed = Mathf.Max(1f, speedLimit);
            arc = Mathf.Max(0f, upwardArc);
        }

        void Awake()
        {
            _grab = GetComponent<XRGrabInteractable>();
            _body = GetComponent<Rigidbody>();
            _grab.selectEntered.AddListener(OnSelected);
        }

        void OnDestroy()
        {
            if (_grab != null)
                _grab.selectEntered.RemoveListener(OnSelected);
        }

        void Update()
        {
            if (_grab == null || !_grab.isSelected || Time.time < _sampleAfter)
                return;

            Push(transform.position);
        }

        /// <summary>
        /// Writes the swing velocity onto the rigidbody. Call from select-exit before reading speed.
        /// </summary>
        public bool TryApply(SelectExitEventArgs args)
        {
            if (args.isCanceled)
            {
                _count = 0;
                return false;
            }

            Vector3 gesture = PeakVelocity();
            _count = 0;
            if (_body == null)
                return false;

            if (gesture.magnitude < minGestureSpeed)
            {
                Drop();
                return false;
            }

            Vector3 velocity = Vector3.ClampMagnitude(gesture * velocityScale, maxSpeed);
            velocity = ApplyArc(velocity);
            _body.isKinematic = false;
            _body.useGravity = true;
            _body.linearVelocity = velocity;
            _body.angularVelocity = Vector3.zero;
            return true;
        }

        void Drop()
        {
            _body.isKinematic = false;
            _body.useGravity = true;
            _body.linearVelocity = Vector3.zero;
            _body.angularVelocity = Vector3.zero;
        }

        void OnSelected(SelectEnterEventArgs args)
        {
            _count = 0;
            _sampleAfter = Time.time + IgnoreGrabSeconds;
        }

        void Push(Vector3 position)
        {
            if (_count == _samples.Length)
            {
                Array.Copy(_samples, 1, _samples, 0, _count - 1);
                _count--;
            }

            _samples[_count++] = new Sample(Time.time, position);

            float cutoff = Time.time - WindowSeconds;
            int drop = 0;
            while (drop < _count && _samples[drop].Time < cutoff)
                drop++;

            if (drop <= 0)
                return;

            Array.Copy(_samples, drop, _samples, 0, _count - drop);
            _count -= drop;
        }

        Vector3 PeakVelocity()
        {
            Vector3 best = Vector3.zero;
            float bestSq = 0f;
            for (int i = 0; i < _count; i++)
            {
                for (int j = i + 1; j < _count; j++)
                {
                    float dt = _samples[j].Time - _samples[i].Time;
                    if (dt < MinSpanSeconds)
                        continue;
                    if (dt > MaxSpanSeconds)
                        break;

                    Vector3 velocity = (_samples[j].Position - _samples[i].Position) / dt;
                    float sq = velocity.sqrMagnitude;
                    if (sq <= bestSq)
                        continue;

                    bestSq = sq;
                    best = velocity;
                }
            }

            return best;
        }

        Vector3 ApplyArc(Vector3 velocity)
        {
            if (arc <= 0f)
                return velocity;

            float horizontal = new Vector3(velocity.x, 0f, velocity.z).magnitude;
            float targetUp = horizontal * arc;
            if (velocity.y < targetUp)
                velocity.y = Mathf.Lerp(velocity.y, targetUp, 0.8f);

            return Vector3.ClampMagnitude(velocity, maxSpeed);
        }

        readonly struct Sample
        {
            public Sample(float time, Vector3 position)
            {
                Time = time;
                Position = position;
            }

            public float Time { get; }
            public Vector3 Position { get; }
        }
    }
}
