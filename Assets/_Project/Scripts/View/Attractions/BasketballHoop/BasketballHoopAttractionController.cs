using Fairground.Core.Attractions;
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
        [SerializeField] BasketballHoopHudView hud;

        BasketballHoopViewModel _viewModel;
        BasketballHoopInput _input;

        public AttractionId AttractionId => AttractionId.BasketballHoop;
        public BasketballHoopViewModel ViewModel => _viewModel;
        public BasketballHoopInput Input => _input;

        public void Configure(
            BasketDetectorView hoopDetector,
            BallSpawnerView spawner,
            BasketballHoopHudView hudView,
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

        void Awake()
        {
            _input = new BasketballHoopInput();
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
            hud?.Bind(_viewModel);
            Subscribe();
            _viewModel.StartGame();
            UpdateSpawnerAvailability();
        }

        void ResolveHoop()
        {
            if (hoop == null)
                hoop = FindFirstObjectByType<BasketDetectorView>();

            if (ballSpawner == null)
                ballSpawner = FindFirstObjectByType<BallSpawnerView>();
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

            ball.Thrown -= OnBallThrown;
            ball.Thrown += OnBallThrown;
            ball.Resolved -= OnBallResolved;
            ball.Resolved += OnBallResolved;
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
            foreach (var ball in FindObjectsByType<BasketballBallView>(FindObjectsSortMode.None))
            {
                if (ball == null)
                    continue;

                ball.Thrown -= OnBallThrown;
                ball.Resolved -= OnBallResolved;
            }
        }

        void OnBallThrown(BasketballBallView ball) => Forward(_viewModel.NotifyBallThrown);

        void OnBallResolved(BasketballBallView ball) => Forward(_viewModel.NotifyThrowResolved);

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
            foreach (var ball in FindObjectsByType<BasketballBallView>(FindObjectsSortMode.None))
                if (ball != null)
                    Destroy(ball.gameObject);
        }

        public void ReturnToFairground()
        {
            SceneManager.LoadScene(AttractionScenes.Fairground);
        }
    }
}
