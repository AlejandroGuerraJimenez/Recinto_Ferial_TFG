using System;
using UnityEngine;

namespace Fairground.View.Attractions.PunchingBall
{
    /// <summary>
    /// Punch target. Observer: <see cref="Punched"/> with impact magnitude.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class PunchPadView : MonoBehaviour
    {
        [SerializeField] float minImpactSpeed = 1.2f;
        [SerializeField] float cooldownSeconds = 0.45f;
        [SerializeField] float hitSquash = 0.82f;
        [SerializeField] float recoverSpeed = 8f;
        [SerializeField] Renderer padRenderer;
        [SerializeField] Color hitColor = new Color(1f, 0.35f, 0.2f, 1f);

        Color _baseColor = new Color(0.75f, 0.12f, 0.12f, 1f);
        Vector3 _restScale;
        float _nextPunchAt;
        bool _accepting = true;
        MaterialPropertyBlock _tint;

        public event Action<PunchPadView, float> Punched;
        public bool CanAcceptPunch => _accepting && Time.time >= _nextPunchAt;

        void Awake()
        {
            if (padRenderer == null)
                padRenderer = GetComponentInChildren<Renderer>();
            _restScale = transform.localScale;
            CaptureBaseColor();
        }

        void Update()
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _restScale, Time.deltaTime * recoverSpeed);
        }

        public void SetAcceptingPunches(bool enabled) => _accepting = enabled;

        public void ResetVisual()
        {
            transform.localScale = _restScale;
            ApplyTint(_baseColor);
        }

        public bool TryApplyImpact(float impactMagnitude)
        {
            if (!CanAcceptPunch || impactMagnitude < minImpactSpeed)
                return false;

            _nextPunchAt = Time.time + cooldownSeconds;
            ApplyHitFeedback();
            Punched?.Invoke(this, impactMagnitude);
            return true;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision == null)
                return;

            TryApplyImpact(ResolveImpactSpeed(collision.collider, collision.relativeVelocity.magnitude));
        }

        void OnTriggerEnter(Collider other)
        {
            if (other == null)
                return;

            TryApplyImpact(ResolveImpactSpeed(other, 0f));
        }

        static float ResolveImpactSpeed(Collider other, float fallback)
        {
            var tracker = other.GetComponentInParent<FistVelocityTracker>();
            if (tracker != null && tracker.Speed > fallback)
                return tracker.Speed;

            var body = other.attachedRigidbody;
            if (body != null && !body.isKinematic)
                return Mathf.Max(fallback, body.linearVelocity.magnitude);

            return fallback;
        }

        void ApplyHitFeedback()
        {
            transform.localScale = new Vector3(_restScale.x * hitSquash, _restScale.y, _restScale.z * hitSquash);
            ApplyTint(hitColor);
        }

        void CaptureBaseColor()
        {
            if (padRenderer == null || padRenderer.sharedMaterial == null)
                return;

            var mat = padRenderer.sharedMaterial;
            if (mat.HasProperty("_BaseColor"))
                _baseColor = mat.GetColor("_BaseColor");
            else
                _baseColor = mat.color;
        }

        void ApplyTint(Color color)
        {
            if (padRenderer == null)
                return;

            _tint ??= new MaterialPropertyBlock();
            padRenderer.GetPropertyBlock(_tint);
            _tint.SetColor("_BaseColor", color);
            _tint.SetColor("_Color", color);
            padRenderer.SetPropertyBlock(_tint);
        }
    }
}
