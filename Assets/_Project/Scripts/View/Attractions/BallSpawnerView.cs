using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Spawns the current throwable and replaces it after a throw. Balloon and basketball booths both use this.
    /// </summary>
    public sealed class BallSpawnerView : MonoBehaviour
    {
        [SerializeField] ThrownPropView ballPrefab;
        [SerializeField] Transform spawnPoint;
        [SerializeField] float respawnDelay = 0.35f;

        readonly HashSet<ThrownPropView> _live = new HashSet<ThrownPropView>();
        ThrownPropView _current;
        float _respawnAt = -1f;
        bool _spawningEnabled = true;

        public event Action<IThrownProp> BallSpawned;
        public ThrownPropView CurrentBall => _current;

        public void Configure(ThrownPropView prefab, Transform point)
        {
            ballPrefab = prefab;
            spawnPoint = point != null ? point : transform;
            if (isActiveAndEnabled && _current == null)
                TrySpawnImmediate();
        }

        public void SetSpawningEnabled(bool enabled)
        {
            _spawningEnabled = enabled;
            if (!enabled)
                ClearIdleBall();
        }

        void Start() => TrySpawnImmediate();

        void Update()
        {
            if (ShouldRespawn())
                TrySpawnImmediate();
        }

        public ThrownPropView TrySpawnImmediate()
        {
            if (!CanSpawn())
                return _current;

            _current = CreateBall();
            BallSpawned?.Invoke(_current);
            return _current;
        }

        public void LaunchCurrentBall(Vector3 velocity)
        {
            if (_current == null)
                TrySpawnImmediate();

            if (_current == null)
                return;

            DisableGrab(_current);
            _current.Launch(velocity);
        }

        bool ShouldRespawn()
        {
            return _spawningEnabled
                   && ballPrefab != null
                   && _current == null
                   && _respawnAt > 0f
                   && Time.time >= _respawnAt;
        }

        bool CanSpawn() => _spawningEnabled && ballPrefab != null && _current == null;

        ThrownPropView CreateBall()
        {
            Transform point = spawnPoint != null ? spawnPoint : transform;
            ThrownPropView ball = Instantiate(ballPrefab, point.position, point.rotation);
            ball.gameObject.SetActive(true);
            Register(ball);
            ball.Thrown += OnThrown;
            ResetPhysics(ball);
            return ball;
        }

        void Register(ThrownPropView ball)
        {
            if (ball == null || !_live.Add(ball))
                return;

            ball.Destroyed += OnDestroyed;
        }

        void OnDestroyed(IThrownProp ball)
        {
            if (ball != null)
            {
                ball.Thrown -= OnThrown;
                ball.Destroyed -= OnDestroyed;
            }

            _live.Remove(ball as ThrownPropView);
            if (ReferenceEquals(_current, ball))
                _current = null;
        }

        static void ResetPhysics(ThrownPropView ball)
        {
            var body = ball.GetComponent<Rigidbody>();
            if (body == null)
                return;

            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }

        void OnThrown(IThrownProp ball)
        {
            if (ball != null)
                ball.Thrown -= OnThrown;

            if (ReferenceEquals(_current, ball))
                _current = null;

            _respawnAt = Time.time + respawnDelay;
        }

        public void Reset()
        {
            _respawnAt = -1f;
            _spawningEnabled = true;
            _current = null;

            if (_live.Count == 0)
                return;

            var snapshot = new List<ThrownPropView>(_live);
            foreach (var ball in snapshot)
            {
                if (ball == null)
                    continue;

                ball.Thrown -= OnThrown;
                ball.Destroyed -= OnDestroyed;
                Destroy(ball.gameObject);
            }

            _live.Clear();
        }

        void ClearIdleBall()
        {
            if (_current == null || _current.IsThrown)
                return;

            var ball = _current;
            _current = null;
            Destroy(ball.gameObject);
        }

        static void DisableGrab(ThrownPropView ball)
        {
            var grab = ball.GetComponent<XRGrabInteractable>();
            if (grab != null)
                grab.enabled = false;
        }
    }
}
