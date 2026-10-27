using System.Collections.Generic;
using Fairground.Core.Attractions;
using Fairground.View.Attractions.BalloonThrow.Factories;
using Fairground.ViewModel;
using Fairground.ViewModel.Attractions.BalloonThrow;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fairground.View.Attractions.BalloonThrow
{
    /// <summary>
    /// View Facade: owns ViewModel and observes ball/balloon events.
    /// </summary>
    public sealed class BalloonThrowAttractionController : MonoBehaviour
    {
        [SerializeField] int pointsPerBalloon = 10;
        [SerializeField] int startingThrows = 8;
        [SerializeField] BalloonTargetView[] balloons;
        [SerializeField] BallSpawnerView ballSpawner;
        [SerializeField] BalloonThrowHudView hud;

        BalloonThrowViewModel _viewModel;
        BalloonThrowInput _input;
        readonly BalloonTargetFactory _balloonFactory = new BalloonTargetFactory();
        readonly HashSet<ThrowableBallView> _liveBalls = new HashSet<ThrowableBallView>();
        Transform _balloonRoot;

        public AttractionId AttractionId => AttractionId.BalloonThrow;
        public BalloonThrowViewModel ViewModel => _viewModel;
        public BalloonThrowInput Input => _input;

        public void Configure(
            BalloonTargetView[] balloonTargets,
            BallSpawnerView spawner,
            BalloonThrowHudView hudView,
            int points,
            int throws)
        {
            balloons = balloonTargets;
            ballSpawner = spawner;
            hud = hudView;
            pointsPerBalloon = points;
            startingThrows = throws;
        }

        void Awake()
        {
            _input = new BalloonThrowInput();
            _input.Enable();
        }

        void Start()
        {
            if (_viewModel != null)
                return;

            BootstrapMatch();
        }

        void Update()
        {
            if (_input == null)
                return;

            if (_input.RestartPressed)
                RestartMatch();

            if (_input.ReturnToFairgroundPressed)
                ReturnToFairground();
        }

        void OnDestroy()
        {
            Unsubscribe();
            _input?.Dispose();
            _input = null;
        }

        void BootstrapMatch()
        {
            ResolveBalloons();
            _viewModel = CreateViewModel();
            hud?.Bind(_viewModel);
            Subscribe();
            _viewModel.StartGame();
            UpdateSpawnerAvailability();
        }

        void ResolveBalloons()
        {
            if (balloons == null || balloons.Length == 0)
                balloons = FindObjectsByType<BalloonTargetView>(FindObjectsSortMode.None);

            RememberBalloonRoot();
        }

        BalloonThrowViewModel CreateViewModel()
        {
            int balloonCount = balloons != null ? Mathf.Max(1, balloons.Length) : 1;
            return new BalloonThrowViewModel(pointsPerBalloon, startingThrows, balloonCount);
        }

        void Subscribe()
        {
            SubscribeBalloons();
            SubscribeSpawner();
        }

        void SubscribeBalloons()
        {
            if (balloons == null)
                return;

            foreach (var balloon in balloons)
                if (balloon != null)
                    balloon.Popped += OnBalloonPopped;
        }

        void SubscribeSpawner()
        {
            if (ballSpawner == null)
                return;

            ballSpawner.BallSpawned += OnBallSpawned;
            if (ballSpawner.CurrentBall != null)
                OnBallSpawned(ballSpawner.CurrentBall);
        }

        void OnBallSpawned(ThrowableBallView ball)
        {
            if (ball == null)
                return;

            RegisterBall(ball);
            ball.Thrown -= OnBallThrown;
            ball.Thrown += OnBallThrown;
            ball.Resolved -= OnBallResolved;
            ball.Resolved += OnBallResolved;
        }

        void RegisterBall(ThrowableBallView ball)
        {
            if (!_liveBalls.Add(ball))
                return;

            ball.Destroyed += OnBallDestroyed;
        }

        void OnBallDestroyed(ThrowableBallView ball)
        {
            UnregisterBall(ball);
        }

        void UnregisterBall(ThrowableBallView ball)
        {
            if (ball != null)
            {
                ball.Thrown -= OnBallThrown;
                ball.Resolved -= OnBallResolved;
                ball.Destroyed -= OnBallDestroyed;
            }

            _liveBalls.Remove(ball);
        }

        void Unsubscribe()
        {
            if (ballSpawner != null)
                ballSpawner.BallSpawned -= OnBallSpawned;

            UnsubscribeBalloons();
            UnsubscribeBalls();
        }

        void UnsubscribeBalloons()
        {
            if (balloons == null)
                return;

            foreach (var balloon in balloons)
                if (balloon != null)
                    balloon.Popped -= OnBalloonPopped;
        }

        void UnsubscribeBalls()
        {
            if (_liveBalls.Count == 0)
                return;

            var snapshot = new List<ThrowableBallView>(_liveBalls);
            foreach (var ball in snapshot)
                UnregisterBall(ball);
        }

        void OnBallThrown(ThrowableBallView ball) => Forward(_viewModel.NotifyBallThrown);

        void OnBallResolved(ThrowableBallView ball)
        {
            Forward(_viewModel.NotifyThrowResolved);
            UnregisterBall(ball);
        }

        void OnBalloonPopped(BalloonTargetView balloon) => Forward(_viewModel.NotifyBalloonHit);

        void Forward(System.Func<bool> action)
        {
            if (_viewModel == null)
                return;

            if (action())
                UpdateSpawnerAvailability();
        }

        void UpdateSpawnerAvailability()
        {
            if (ballSpawner == null || _viewModel == null)
                return;

            ballSpawner.SetSpawningEnabled(_viewModel.CanSpawnBall);
        }

        public void RestartMatch()
        {
            if (_viewModel == null)
                return;

            Unsubscribe();
            ClearBalls();
            _viewModel.Restart();
            RebuildBalloons();
            Subscribe();
            UpdateSpawnerAvailability();
            ballSpawner?.TrySpawnImmediate();
        }

        void ClearBalls()
        {
            ballSpawner?.Reset();

            if (_liveBalls.Count == 0)
                return;

            var snapshot = new List<ThrowableBallView>(_liveBalls);
            _liveBalls.Clear();
            foreach (var ball in snapshot)
            {
                if (ball == null)
                    continue;

                ball.Thrown -= OnBallThrown;
                ball.Resolved -= OnBallResolved;
                ball.Destroyed -= OnBallDestroyed;
                Destroy(ball.gameObject);
            }
        }

        void RebuildBalloons()
        {
            if (_balloonRoot != null)
                Destroy(_balloonRoot.gameObject);

            balloons = _balloonFactory.CreateRow(_viewModel.BalloonCount);
            RememberBalloonRoot();
            global::Fairground.View.Attractions.AttractionPlacement.TryApplyFromSession(_balloonRoot);
        }

        void RememberBalloonRoot()
        {
            if (balloons == null || balloons.Length == 0 || balloons[0] == null)
                return;

            _balloonRoot = balloons[0].transform.parent;
        }

        public void ReturnToFairground()
        {
            FairgroundSession.LeaveAttraction();
            SceneManager.LoadScene(AttractionScenes.Fairground);
        }
    }
}
