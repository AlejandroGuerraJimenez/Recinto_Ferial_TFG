using Fairground.Core.Attractions;
using Fairground.View.Attractions.DuckFishing.Factories;
using Fairground.ViewModel;
using Fairground.ViewModel.Attractions.DuckFishing;
using UnityEngine;

namespace Fairground.View.Attractions.DuckFishing
{
    /// <summary>
    /// View Facade: owns ViewModel and observes hook/duck events.
    /// </summary>
    public sealed class DuckFishingAttractionHost : MonoBehaviour
    {
        [SerializeField] DuckTargetView[] ducks;
        [SerializeField] FishingRodView rod;
        [SerializeField] BoothHudView hud;
        [SerializeField] Transform pond;

        DuckFishingViewModel _viewModel;
        AttractionMatchControls _controls;
        readonly DuckTargetFactory _duckFactory = new DuckTargetFactory();
        Transform _duckRoot;

        public AttractionId AttractionId => AttractionId.DuckFishing;
        public DuckFishingViewModel ViewModel => _viewModel;
        public AttractionInput Input => _controls != null ? _controls.Input : null;

        public void Configure(
            DuckTargetView[] duckTargets,
            FishingRodView fishingRod,
            BoothHudView hudView,
            Transform pondRoot,
            DuckFishingViewModel match)
        {
            ducks = BoothWiring.RequireAny(duckTargets, "Duck fishing", "ducks");
            rod = BoothWiring.Require(fishingRod, "Duck fishing", "a rod");
            hud = BoothWiring.Require(hudView, "Duck fishing", "a HUD");
            pond = BoothWiring.Require(pondRoot, "Duck fishing", "a pond");
            _viewModel = BoothWiring.Require(match, "Duck fishing", "a match");
            OpenMatch();
        }

        public void Receive(FairgroundVisitState session) => _controls?.Use(session);

        void Awake() => _controls = new AttractionMatchControls(RestartMatch, headsetTriggers: true);

        void Update()
        {
            if (_controls == null)
                return;

            _controls.Tick();
            if (_controls.Input.PlayPressed)
                rod?.Hook?.TryStartDip();
        }

        void OnDestroy()
        {
            Unsubscribe();
            _controls?.Dispose();
            _controls = null;
        }

        void OpenMatch()
        {
            RememberDuckRoot();
            BoothHudBinding.Attach(hud, _viewModel, () => DuckFishingHud.Lines(_viewModel));
            Subscribe();
            _viewModel.StartMatch();
            UpdateHookAvailability();
        }

        void Subscribe()
        {
            if (rod?.Hook != null)
            {
                rod.Hook.DipStarted += OnDipStarted;
                rod.Hook.DipResolved += OnDipResolved;
                rod.Hook.DuckCaught += OnDuckCaught;
            }
        }

        void Unsubscribe()
        {
            if (rod?.Hook == null)
                return;

            rod.Hook.DipStarted -= OnDipStarted;
            rod.Hook.DipResolved -= OnDipResolved;
            rod.Hook.DuckCaught -= OnDuckCaught;
        }

        void OnDipStarted() => Forward(_viewModel.TryRegisterDip);

        void OnDipResolved() => Forward(_viewModel.TryResolveDip);

        void OnDuckCaught(DuckTargetView duck) => Forward(_viewModel.TryRegisterCatch);

        void Forward(System.Func<bool> action)
        {
            if (_viewModel == null)
                return;

            if (action())
                UpdateHookAvailability();
        }

        void UpdateHookAvailability()
        {
            if (rod?.Hook == null || _viewModel == null)
                return;

            rod.Hook.SetEnabled(_viewModel.CanDip);
        }

        public void RestartMatch()
        {
            if (_viewModel == null)
                return;

            Unsubscribe();
            _viewModel.Restart();
            RebuildDucks();
            Subscribe();
            UpdateHookAvailability();
        }

        void RebuildDucks()
        {
            if (_duckRoot != null)
                Destroy(_duckRoot.gameObject);

            Transform pondRoot = pond != null ? pond : transform;
            ducks = _duckFactory.CreateRing(pondRoot, _viewModel.DuckCount);
            RememberDuckRoot();
        }

        void RememberDuckRoot()
        {
            if (ducks == null || ducks.Length == 0 || ducks[0] == null)
                return;

            _duckRoot = ducks[0].transform.parent;
        }
    }
}
