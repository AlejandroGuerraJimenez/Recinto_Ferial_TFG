using Fairground.Core.Attractions;
using Fairground.Model.Attractions.BalloonThrow;
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
        [SerializeField] KeyCode restartKey = KeyCode.R;

        BalloonThrowViewModel _viewModel;

        public AttractionId AttractionId => AttractionId.BalloonThrow;
        public BalloonThrowViewModel ViewModel => _viewModel;

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

        void Start()
        {
            if (_viewModel != null)
                return;

            BootstrapMatch();
        }

        void Update()
        {
            if (Input.GetKeyDown(restartKey))
                RestartMatch();
        }

        void OnDestroy() => Unsubscribe();

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
        }

        BalloonThrowViewModel CreateViewModel()
        {
            int balloonCount = balloons != null ? Mathf.Max(1, balloons.Length) : 1;
            var rules = new BalloonThrowRules(pointsPerBalloon, startingThrows, balloonCount);
            return new BalloonThrowViewModel(rules);
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

            ball.Thrown -= OnBallThrown;
            ball.Thrown += OnBallThrown;
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
            foreach (var ball in FindObjectsByType<ThrowableBallView>(FindObjectsSortMode.None))
                if (ball != null)
                    ball.Thrown -= OnBallThrown;
        }

        void OnBallThrown(ThrowableBallView ball) => HandleGameplayAction(_viewModel.NotifyBallThrown);

        void OnBalloonPopped(BalloonTargetView balloon) => HandleGameplayAction(_viewModel.NotifyBalloonHit);

        void HandleGameplayAction(System.Func<bool> action)
        {
            if (_viewModel == null || !_viewModel.IsPlaying)
                return;

            action();
            UpdateSpawnerAvailability();
        }

        void UpdateSpawnerAvailability()
        {
            if (ballSpawner == null || _viewModel == null)
                return;

            bool canThrow = _viewModel.IsPlaying && _viewModel.ThrowsRemaining > 0;
            ballSpawner.SetSpawningEnabled(canThrow);
        }

        public void RestartMatch()
        {
            SceneManager.LoadScene(AttractionScenes.BalloonThrow);
        }
    }
}
