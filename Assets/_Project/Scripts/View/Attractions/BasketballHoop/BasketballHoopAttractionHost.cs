using Fairground.Core.Attractions;
using Fairground.ViewModel;
using Fairground.ViewModel.Attractions.BasketballHoop;
using UnityEngine;

namespace Fairground.View.Attractions.BasketballHoop
{
    /// <summary>
    /// Hosts the hoop match: the rim, the shared throw session, and the HUD.
    /// </summary>
    public sealed class BasketballHoopAttractionHost : MonoBehaviour
    {
        [SerializeField] BasketDetectorView hoop;
        [SerializeField] BallSpawnerView ballSpawner;
        [SerializeField] BoothHudView hud;

        BasketballHoopViewModel _viewModel;
        ThrownMatchSession _match;

        public AttractionId AttractionId => AttractionId.BasketballHoop;
        public BasketballHoopViewModel ViewModel => _viewModel;
        public AttractionInput Input => _match != null ? _match.Input : null;

        public void Configure(
            BasketDetectorView hoopDetector,
            BallSpawnerView spawner,
            BoothHudView hudView,
            BasketballHoopViewModel viewModel)
        {
            hoop = BoothWiring.Require(hoopDetector, "Basketball hoop", "a hoop");
            ballSpawner = BoothWiring.Require(spawner, "Basketball hoop", "a ball spawner");
            hud = BoothWiring.Require(hudView, "Basketball hoop", "a HUD");
            _viewModel = BoothWiring.Require(viewModel, "Basketball hoop", "a match");
            OpenMatch();
        }

        public void Receive(FairgroundVisitState visit) => _match?.Use(visit);

        void Awake()
        {
            _match = new ThrownMatchSession(RestartMatch);
            _match.Bind();
        }

        void Update()
        {
            _match?.Tick();
            if (_viewModel == null || !_viewModel.IsPlaying)
                return;

            _viewModel.Tick(Time.deltaTime);
            if (!_viewModel.IsPlaying)
                ballSpawner?.SetSpawningEnabled(false);
        }

        void OnDestroy()
        {
            Unsubscribe();
            _match?.Dispose();
        }

        void OpenMatch()
        {
            Subscribe();
            _match.Open(
                _viewModel,
                ballSpawner,
                hud,
                () => BasketballHoopHud.Lines(_viewModel),
                _viewModel.TryRegisterThrow,
                _viewModel.TryResolveThrow,
                () => _viewModel.CanThrow);
        }

        void Subscribe()
        {
            if (hoop != null)
                hoop.Scored += OnBasket;
        }

        void Unsubscribe()
        {
            if (hoop != null)
                hoop.Scored -= OnBasket;
        }

        void OnBasket(BasketballBallView ball) => _match.ApplyAndRefresh(_viewModel.TryRegisterBasket);

        public void RestartMatch()
        {
            if (_viewModel == null)
                return;

            _match.Restart(null);
        }
    }
}
