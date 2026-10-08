using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Fairground.View.Attractions.BasketballHoop
{
    /// <summary>
    /// Spawns throwable basketballs. Observer: <see cref="BallSpawned"/>.
    /// </summary>
    public sealed class BallSpawnerView : MonoBehaviour
    {
        [SerializeField] BasketballBallView ballPrefab;
        [SerializeField] Transform spawnPoint;
        [SerializeField] float respawnDelay = 0.35f;

        BasketballBallView _currentBall;
        float _respawnAt = -1f;
        bool _spawningEnabled = true;

        public event Action<BasketballBallView> BallSpawned;
        public BasketballBallView CurrentBall => _currentBall;

        public void Configure(BasketballBallView prefab, Transform point)
        {
            ballPrefab = prefab;
            spawnPoint = point != null ? point : transform;
            if (isActiveAndEnabled && _currentBall == null)
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

        public BasketballBallView TrySpawnImmediate()
        {
            if (!CanSpawn())
                return _currentBall;

            _currentBall = CreateBall();
            BallSpawned?.Invoke(_currentBall);
            return _currentBall;
        }

        public void LaunchCurrentBall(Vector3 velocity)
        {
            if (_currentBall == null)
                TrySpawnImmediate();

            if (_currentBall == null)
                return;

            DisableGrab(_currentBall);
            _currentBall.Launch(velocity);
        }

        bool ShouldRespawn()
        {
            return _spawningEnabled
                   && ballPrefab != null
                   && _currentBall == null
                   && _respawnAt > 0f
                   && Time.time >= _respawnAt;
        }

        bool CanSpawn() => _spawningEnabled && ballPrefab != null && _currentBall == null;

        BasketballBallView CreateBall()
        {
            Transform point = spawnPoint != null ? spawnPoint : transform;
            var ball = Instantiate(ballPrefab, point.position, point.rotation);
            ball.gameObject.SetActive(true);
            ball.Thrown += OnBallThrown;
            ResetPhysics(ball);
            return ball;
        }

        static void ResetPhysics(BasketballBallView ball)
        {
            var body = ball.GetComponent<Rigidbody>();
            if (body == null)
                return;

            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }

        void OnBallThrown(BasketballBallView ball)
        {
            if (ball != null)
                ball.Thrown -= OnBallThrown;

            if (_currentBall == ball)
                _currentBall = null;

            _respawnAt = Time.time + respawnDelay;
        }

        public void Reset()
        {
            if (_currentBall != null)
            {
                _currentBall.Thrown -= OnBallThrown;
                Destroy(_currentBall.gameObject);
                _currentBall = null;
            }

            _respawnAt = -1f;
            _spawningEnabled = true;
        }

        void ClearIdleBall()
        {
            if (_currentBall == null || _currentBall.IsThrown)
                return;

            Destroy(_currentBall.gameObject);
            _currentBall = null;
        }

        static void DisableGrab(BasketballBallView ball)
        {
            var grab = ball.GetComponent<XRGrabInteractable>();
            if (grab != null)
                grab.enabled = false;
        }
    }
}
