using System;
using UnityEngine;

namespace Fairground.View.Attractions.DuckFishing
{
    /// <summary>
    /// Floating duck. The ring on its back is what the hook can catch.
    /// </summary>
    public sealed class DuckTargetView : MonoBehaviour
    {
        [SerializeField] float pathRadius = 1.35f;
        [SerializeField] float pathSpeed = 0.55f;
        [SerializeField] float bobAmplitude = 0.04f;
        [SerializeField] float bobSpeed = 2.4f;

        float _angle;
        float _baseY;
        bool _caught;
        Vector3 _center;
        Vector3 _restScale;
        Transform _ring;
        Collider _collider;
        Renderer[] _renderers;

        public event Action<DuckTargetView> Caught;
        public bool IsCaught => _caught;
        public Vector3 RingPosition => _ring != null ? _ring.position : transform.position;

        public void AssignRing(Transform ring)
        {
            _ring = ring;
            var ringCollider = ring != null ? ring.GetComponent<Collider>() : null;
            if (ringCollider != null)
                _collider = ringCollider;
        }

        public void Configure(Vector3 center, float angle, float radius, float speed)
        {
            _center = center;
            _angle = angle;
            pathRadius = radius;
            pathSpeed = speed;
            _baseY = transform.position.y;
            ApplyOrbitPose(0f);
        }

        void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>(true);
            _restScale = transform.localScale;
            _collider = GetComponent<Collider>();
            _center = transform.position;
            _baseY = transform.position.y;
        }

        void Update()
        {
            if (_caught)
                return;

            ApplyOrbitPose(Time.deltaTime);
        }

        public bool TryCatch()
        {
            if (_caught)
                return false;

            _caught = true;
            Caught?.Invoke(this);
            if (_collider != null)
                _collider.enabled = false;

            return true;
        }

        public void Ride(Vector3 worldPosition) => transform.position = worldPosition;

        public void Park(Vector3 worldPosition, Quaternion worldRotation)
        {
            transform.localScale = _restScale;
            transform.SetPositionAndRotation(worldPosition, worldRotation);
            SetRenderersEnabled(true);
            ClearTint();
        }

        public void Rearm()
        {
            _caught = false;
            transform.localScale = _restScale;
            if (_collider != null)
                _collider.enabled = true;

            SetRenderersEnabled(true);
            ClearTint();
        }

        void ApplyOrbitPose(float deltaTime)
        {
            _angle += pathSpeed * deltaTime;
            float x = _center.x + Mathf.Cos(_angle) * pathRadius;
            float z = _center.z + Mathf.Sin(_angle) * pathRadius;
            float y = _baseY + Mathf.Sin((Time.time + _angle) * bobSpeed) * bobAmplitude;
            transform.position = new Vector3(x, y, z);

            Vector3 tangent = new Vector3(-Mathf.Sin(_angle), 0f, Mathf.Cos(_angle));
            if (tangent.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(tangent, Vector3.up);
        }

        void SetRenderersEnabled(bool enabled)
        {
            if (_renderers == null)
                return;

            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i] != null)
                    _renderers[i].enabled = enabled;
            }
        }

        void ClearTint()
        {
            if (_renderers == null)
                return;

            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i] != null)
                    _renderers[i].SetPropertyBlock(null);
            }
        }
    }
}
