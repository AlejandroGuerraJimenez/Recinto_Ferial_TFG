using System.Collections.Generic;
using Fairground.Core.Attractions;
using Fairground.ViewModel;
using Fairground.ViewModel.Attractions.BasketballHoop;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fairground.View.Attractions.BasketballHoop
{
    /// <summary>
    /// View Facade: owns ViewModel and observes ball/hoop events.
    /// </summary>
    public sealed class BasketballHoopAttractionController : MonoBehaviour
    {
        [SerializeField] int pointsPerBasket = 2;
        [SerializeField] int startingBalls = 10;
        [SerializeField] int winScoreThreshold = 5;
        [SerializeField] BasketDetectorView hoop;
        [SerializeField] BallSpawnerView ballSpawner;
        [SerializeField] BoothHudView hud;

        BasketballHoopViewModel _viewModel;
        AttractionInput _input;
        FairgroundSession _session;
        readonly HashSet<BasketballBallView> _liveBalls = new HashSet<BasketballBallView>();

        public AttractionId AttractionId => AttractionId.BasketballHoop;
        public BasketballHoopViewModel ViewModel => _viewModel;
        public AttractionInput Input => _input;

        public void Configure(
            BasketDetectorView hoopDetector,
            BallSpawnerView spawner,
            BoothHudView hudView,
            int points,
            int balls,
            int winThreshold)
        {
            hoop = hoopDetector;
            ballSpawner = spawner;
            hud = hudView;
            pointsPerBasket = points;
            startingBalls = balls;
            winScoreThreshold = winThreshold;
        }

        public void Receive(FairgroundSession session) => _session = session;

        void Awake()
        {
            _input = new AttractionInput();
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
            ResolveHoop();
            _viewModel = CreateViewModel();
            hud?.Bind(
                () => BasketballHoopBoothLayout.FormatHud(_viewModel),
                handler => _viewModel.StateChanged += handler,
                handler => _viewModel.StateChanged -= handler);
            Subscribe();
            _viewModel.StartGame();
            UpdateSpawnerAvailability();
        }

        void ResolveHoop()
        {
            if (hoop == null)
                hoop = FindAnyObjectByType<BasketDetectorView>();

            if (ballSpawner == null)
                ballSpawner = FindAnyObjectByType<BallSpawnerView>();
        }

        BasketballHoopViewModel CreateViewModel()
        {
            return new BasketballHoopViewModel(pointsPerBasket, startingBalls, winScoreThreshold);
        }

        void Subscribe()
        {
            SubscribeHoop();
            SubscribeSpawner();
        }

        void SubscribeHoop()
        {
            if (hoop != null)
                hoop.Scored += OnBasket;
        }

        void SubscribeSpawner()
        {
            if (ballSpawner == null)
                return;

            ballSpawner.BallSpawned += OnBallSpawned;
            if (ballSpawner.CurrentBall != null)
                OnBallSpawned(ballSpawner.CurrentBall);
        }

        void OnBallSpawned(BasketballBallView ball)
        {
            if (ball == null)
                return;

            RegisterBall(ball);
            ball.Thrown -= OnBallThrown;
            ball.Thrown += OnBallThrown;
            ball.Resolved -= OnBallResolved;
            ball.Resolved += OnBallResolved;
        }

        void RegisterBall(BasketballBallView ball)
        {
            if (!_liveBalls.Add(ball))
                return;

            ball.Destroyed += OnBallDestroyed;
        }

        void OnBallDestroyed(BasketballBallView ball)
        {
            UnregisterBall(ball);
        }

        void UnregisterBall(BasketballBallView ball)
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
            if (hoop != null)
                hoop.Scored -= OnBasket;

            if (ballSpawner != null)
                ballSpawner.BallSpawned -= OnBallSpawned;

            UnsubscribeBalls();
        }

        void UnsubscribeBalls()
        {
            if (_liveBalls.Count == 0)
                return;

            var snapshot = new List<BasketballBallView>(_liveBalls);
            foreach (var ball in snapshot)
                UnregisterBall(ball);
        }

        void OnBallThrown(BasketballBallView ball) => Forward(_viewModel.NotifyBallThrown);

        void OnBallResolved(BasketballBallView ball)
        {
            Forward(_viewModel.NotifyThrowResolved);
            UnregisterBall(ball);
        }

        void OnBasket(BasketballBallView ball) => Forward(_viewModel.NotifyBasket);

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

            ballSpawner.SetSpawningEnabled(_viewModel.CanThrow);
        }

        public void RestartMatch()
        {
            if (_viewModel == null)
                return;

            Unsubscribe();
            ClearBalls();
            _viewModel.Restart();
            Subscribe();
            UpdateSpawnerAvailability();
            ballSpawner?.TrySpawnImmediate();
        }

        void ClearBalls()
        {
            ballSpawner?.Reset();

            if (_liveBalls.Count == 0)
                return;

            var snapshot = new List<BasketballBallView>(_liveBalls);
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

        public void ReturnToFairground()
        {
            _session?.LeaveAttraction();
            SceneManager.LoadScene(AttractionScenes.Fairground);
        }
    }
}
