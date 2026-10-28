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
    public sealed class BalloonThrowAttractionHost : MonoBehaviour
    {
        [SerializeField] BalloonTargetView[] balloons;
        [SerializeField] BallSpawnerView ballSpawner;
        [SerializeField] BoothHudView hud;

        BalloonThrowViewModel _viewModel;
        AttractionMatchControls _controls;
        readonly ThrownPropTracker _props = new ThrownPropTracker();
        readonly BalloonTargetFactory _balloonFactory = new BalloonTargetFactory();
        Transform _balloonRoot;
        FairgroundVisitState _visit;

        public AttractionId AttractionId => AttractionId.BalloonThrow;
        public BalloonThrowViewModel ViewModel => _viewModel;
        public AttractionInput Input => _controls != null ? _controls.Input : null;

        public void Configure(
            BalloonTargetView[] balloonTargets,
            BallSpawnerView spawner,
            BoothHudView hudView,
            BalloonThrowViewModel match)
        {
            balloons = BoothWiring.RequireAny(balloonTargets, "Balloon throw", "balloons");
            ballSpawner = BoothWiring.Require(spawner, "Balloon throw", "a ball spawner");
            hud = BoothWiring.Require(hudView, "Balloon throw", "a HUD");
            _viewModel = BoothWiring.Require(match, "Balloon throw", "a match");
            OpenMatch();
        }

        public void Receive(FairgroundVisitState session)
        {
            _visit = session;
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
            RememberBalloonRoot();
            BoothHudBinding.Attach(hud, _viewModel, () => BalloonThrowHud.Lines(_viewModel));
            Subscribe();
            _viewModel.StartMatch();
            UpdateSpawnerAvailability();
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

        void OnBalloonPopped(BalloonTargetView balloon) => Forward(_viewModel.TryRegisterPop);

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

            ballSpawner.SetSpawningEnabled(_viewModel.CanSpendThrow);
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
            AttractionPlacement.TryApply(_visit, _balloonRoot);
        }

        void RememberBalloonRoot()
        {
            if (balloons == null || balloons.Length == 0 || balloons[0] == null)
                return;

            _balloonRoot = balloons[0].transform.parent;
        }
    }
}
