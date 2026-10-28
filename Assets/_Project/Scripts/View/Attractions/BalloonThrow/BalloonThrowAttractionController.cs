using Fairground.Core.Attractions;
using Fairground.View.Attractions.BalloonThrow.Factories;
using Fairground.ViewModel;
using Fairground.ViewModel.Attractions.BalloonThrow;
using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow
{
    /// <summary>
    /// Hosts the balloon match: targets, the shared throw tracker, and the HUD.
    /// </summary>
    public sealed class BalloonThrowAttractionController : MonoBehaviour
    {
        [SerializeField] BalloonTargetView[] balloons;
        [SerializeField] BallSpawnerView ballSpawner;
        [SerializeField] BoothHudView hud;

        BalloonThrowViewModel _viewModel;
        AttractionMatchControls _controls;
        readonly ThrownPropTracker _props = new ThrownPropTracker();
        readonly BalloonTargetFactory _balloonFactory = new BalloonTargetFactory();
        Transform _balloonRoot;
        FairgroundSession _session;

        public AttractionId AttractionId => AttractionId.BalloonThrow;
        public BalloonThrowViewModel ViewModel => _viewModel;
        public AttractionInput Input => _controls != null ? _controls.Input : null;

        public void Configure(
            BalloonTargetView[] balloonTargets,
            BallSpawnerView spawner,
            BoothHudView hudView,
            BalloonThrowViewModel match)
        {
            balloons = balloonTargets;
            ballSpawner = spawner;
            hud = hudView;
            _viewModel = match;
            OpenMatch();
        }

        public void Receive(FairgroundSession session)
        {
            _session = session;
            _controls?.Use(session);
        }

        void Awake()
        {
            _controls = new AttractionMatchControls(RestartMatch);
            _props.Thrown += OnPropThrown;
            _props.Resolved += OnPropResolved;
        }

        void Update() => _controls?.Tick();

        void OnDestroy()
        {
            Unsubscribe();
            _controls?.Dispose();
            _controls = null;
        }

        void OpenMatch()
        {
            if (_viewModel == null)
                return;

            ResolveBalloons();
            BoothHudBinding.Attach(hud, _viewModel, () => BalloonThrowHud.Lines(_viewModel));
            Subscribe();
            _viewModel.StartGame();
            UpdateSpawnerAvailability();
        }

        void ResolveBalloons()
        {
            if (balloons == null || balloons.Length == 0)
                balloons = FindObjectsByType<BalloonTargetView>();

            RememberBalloonRoot();
        }

        void Subscribe()
        {
            SubscribeBalloons();
            _props.Attach(ballSpawner);
        }

        void SubscribeBalloons()
        {
            if (balloons == null)
                return;

            foreach (var balloon in balloons)
                if (balloon != null)
                    balloon.Popped += OnBalloonPopped;
        }

        void Unsubscribe()
        {
            UnsubscribeBalloons();
            _props.Detach();
        }

        void UnsubscribeBalloons()
        {
            if (balloons == null)
                return;

            foreach (var balloon in balloons)
                if (balloon != null)
                    balloon.Popped -= OnBalloonPopped;
        }

        void OnPropThrown() => Forward(_viewModel.TryRegisterThrow);

        void OnPropResolved() => Forward(_viewModel.TryResolveThrow);

        void OnBalloonPopped(BalloonTargetView balloon) => Forward(_viewModel.TryRegisterHit);

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
            _props.DestroyAll(ballSpawner);
            _viewModel.Restart();
            RebuildBalloons();
            Subscribe();
            UpdateSpawnerAvailability();
            ballSpawner?.TrySpawnImmediate();
        }

        void RebuildBalloons()
        {
            if (_balloonRoot != null)
                Destroy(_balloonRoot.gameObject);

            balloons = _balloonFactory.CreateRow(_viewModel.BalloonCount);
            RememberBalloonRoot();
            AttractionPlacement.TryApply(_session, _balloonRoot);
        }

        void RememberBalloonRoot()
        {
            if (balloons == null || balloons.Length == 0 || balloons[0] == null)
                return;

            _balloonRoot = balloons[0].transform.parent;
        }
    }
}
