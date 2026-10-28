using Fairground.Core.Attractions;
using Fairground.ViewModel;
using Fairground.ViewModel.Attractions.BasketballHoop;
using UnityEngine;

namespace Fairground.View.Attractions.BasketballHoop
{
    /// <summary>
    /// Hosts the hoop match: the rim, the shared throw tracker, and the HUD.
    /// </summary>
    public sealed class BasketballHoopAttractionController : MonoBehaviour
    {
        [SerializeField] BasketDetectorView hoop;
        [SerializeField] BallSpawnerView ballSpawner;
        [SerializeField] BoothHudView hud;

        BasketballHoopViewModel _viewModel;
        AttractionMatchControls _controls;
        readonly ThrownPropTracker _props = new ThrownPropTracker();

        public AttractionId AttractionId => AttractionId.BasketballHoop;
        public BasketballHoopViewModel ViewModel => _viewModel;
        public AttractionInput Input => _controls != null ? _controls.Input : null;

        public void Configure(
            BasketDetectorView hoopDetector,
            BallSpawnerView spawner,
            BoothHudView hudView,
            BasketballHoopViewModel match)
        {
            hoop = hoopDetector;
            ballSpawner = spawner;
            hud = hudView;
            _viewModel = match;
            OpenMatch();
        }

        public void Receive(FairgroundSession session) => _controls?.Use(session);

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

            ResolveHoop();
            BoothHudBinding.Attach(hud, _viewModel, () => BasketballHoopHud.Lines(_viewModel));
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

        void Subscribe()
        {
            if (hoop != null)
                hoop.Scored += OnBasket;

            _props.Attach(ballSpawner);
        }

        void Unsubscribe()
        {
            if (hoop != null)
                hoop.Scored -= OnBasket;

            _props.Detach();
        }

        void OnPropThrown() => Forward(_viewModel.TryRegisterThrow);

        void OnPropResolved() => Forward(_viewModel.TryResolveThrow);

        void OnBasket(BasketballBallView ball) => Forward(_viewModel.TryRegisterBasket);

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
            _props.DestroyAll(ballSpawner);
            _viewModel.Restart();
            Subscribe();
            UpdateSpawnerAvailability();
            ballSpawner?.TrySpawnImmediate();
        }
    }
}
