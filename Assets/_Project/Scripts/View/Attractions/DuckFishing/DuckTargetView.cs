using System;
using UnityEngine;

namespace Fairground.View.Attractions.DuckFishing
{
    /// <summary>
    /// Floating duck target. Observer: <see cref="Caught"/>.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class DuckTargetView : MonoBehaviour
    {
        [SerializeField] float pathRadius = 1.35f;
        [SerializeField] float pathSpeed = 0.55f;
        [SerializeField] float bobAmplitude = 0.04f;
        [SerializeField] float bobSpeed = 2.4f;
        [SerializeField] float catchScale = 1.25f;
        [SerializeField] float destroyDelay = 0.25f;
        [SerializeField] Renderer duckRenderer;
        [SerializeField] Color caughtColor = new Color(1f, 0.9f, 0.2f, 1f);

        float _angle;
        float _baseY;
        bool _caught;
        Vector3 _center;
        MaterialPropertyBlock _tint;

        public event Action<DuckTargetView> Caught;
        public bool IsCaught => _caught;

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
            if (duckRenderer == null)
                duckRenderer = GetComponentInChildren<Renderer>();
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
            ApplyCatchFeedback();
            Caught?.Invoke(this);
            Destroy(gameObject, destroyDelay);
            return true;
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

        void ApplyCatchFeedback()
        {
            transform.localScale *= catchScale;
            if (duckRenderer == null)
                return;

            _tint ??= new MaterialPropertyBlock();
            duckRenderer.GetPropertyBlock(_tint);
            _tint.SetColor("_BaseColor", caughtColor);
            _tint.SetColor("_Color", caughtColor);
            duckRenderer.SetPropertyBlock(_tint);
        }
    }
}
