using Fairground.Core.Attractions;
using Fairground.View.Attractions.DuckFishing.Factories;
using Fairground.ViewModel.Attractions.DuckFishing;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fairground.View.Attractions.DuckFishing
{
    /// <summary>
    /// View Facade: owns ViewModel and observes hook/duck events.
    /// </summary>
    public sealed class DuckFishingAttractionController : MonoBehaviour
    {
        [SerializeField] int pointsPerDuck = 10;
        [SerializeField] int startingAttempts = 8;
        [SerializeField] int ducksToWin = 4;
        [SerializeField] DuckTargetView[] ducks;
        [SerializeField] FishingRodView rod;
        [SerializeField] DuckFishingHudView hud;
        [SerializeField] Transform pond;

        DuckFishingViewModel _viewModel;
        DuckFishingInput _input;
        readonly DuckTargetFactory _duckFactory = new DuckTargetFactory();
        Transform _duckRoot;

        public AttractionId AttractionId => AttractionId.DuckFishing;
        public DuckFishingViewModel ViewModel => _viewModel;
        public DuckFishingInput Input => _input;

        public void Configure(
            DuckTargetView[] duckTargets,
            FishingRodView fishingRod,
            DuckFishingHudView hudView,
            Transform pondRoot,
            int points,
            int attempts,
            int winCount)
        {
            ducks = duckTargets;
            rod = fishingRod;
            hud = hudView;
            pond = pondRoot;
            pointsPerDuck = points;
            startingAttempts = attempts;
            ducksToWin = winCount;
        }

        void Awake()
        {
            _input = new DuckFishingInput();
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

            if (_input.DipPressed)
                rod?.Hook?.TryStartDip();
        }

        void OnDestroy()
        {
            Unsubscribe();
            _input?.Dispose();
            _input = null;
        }

        void BootstrapMatch()
        {
            ResolveRefs();
            _viewModel = CreateViewModel();
            hud?.Bind(_viewModel);
            Subscribe();
            _viewModel.StartGame();
            UpdateHookAvailability();
        }

        void ResolveRefs()
        {
            if (ducks == null || ducks.Length == 0)
                ducks = FindObjectsByType<DuckTargetView>(FindObjectsSortMode.None);

            if (rod == null)
                rod = FindFirstObjectByType<FishingRodView>();

            if (pond == null)
            {
                var pondGo = GameObject.Find("Pond");
                pond = pondGo != null ? pondGo.transform : null;
            }

            RememberDuckRoot();
        }

        DuckFishingViewModel CreateViewModel()
        {
            int duckCount = ducks != null ? Mathf.Max(1, ducks.Length) : 1;
            int winCount = Mathf.Clamp(ducksToWin, 1, duckCount);
            return new DuckFishingViewModel(pointsPerDuck, startingAttempts, duckCount, winCount);
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

        void OnDipStarted() => Forward(_viewModel.NotifyDipStarted);

        void OnDipResolved() => Forward(_viewModel.NotifyDipResolved);

        void OnDuckCaught(DuckTargetView duck) => Forward(_viewModel.NotifyDuckCaught);

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

        public void ReturnToFairground()
        {
            SceneManager.LoadScene(AttractionScenes.Fairground);
        }
    }
}
