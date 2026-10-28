using Fairground.Core.Attractions;
using Fairground.ViewModel;
using Fairground.ViewModel.Attractions.PunchingBall;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fairground.View.Attractions.PunchingBall
{
    /// <summary>
    /// View Facade: owns ViewModel and observes punch pad events.
    /// </summary>
    public sealed class PunchingBallAttractionController : MonoBehaviour
    {
        [SerializeField] int startingPunches = 5;
        [SerializeField] int winScoreThreshold = 700;
        [SerializeField] int maxScore = 999;
        [SerializeField] float maxImpactSpeed = 8f;
        [SerializeField] PunchPadView punchPad;
        [SerializeField] PunchPowerMeterView powerMeter;
        [SerializeField] BoothHudView hud;

        PunchingBallViewModel _viewModel;
        AttractionInput _input;

        public AttractionId AttractionId => AttractionId.PunchingBall;
        public PunchingBallViewModel ViewModel => _viewModel;
        public AttractionInput Input => _input;

        public void Configure(
            PunchPadView pad,
            PunchPowerMeterView meter,
            BoothHudView hudView,
            int punches,
            int winThreshold,
            int scoreCap,
            float impactCap)
        {
            punchPad = pad;
            powerMeter = meter;
            hud = hudView;
            startingPunches = punches;
            winScoreThreshold = winThreshold;
            maxScore = scoreCap;
            maxImpactSpeed = impactCap;
        }

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
            ResolvePad();
            _viewModel = CreateViewModel();
            hud?.Bind(
                () => PunchingBallBoothLayout.FormatHud(_viewModel),
                handler => _viewModel.StateChanged += handler,
                handler => _viewModel.StateChanged -= handler);
            Subscribe();
            _viewModel.StartGame();
            UpdatePadAvailability();
            powerMeter?.ResetMeter();
        }

        void ResolvePad()
        {
            if (punchPad == null)
                punchPad = FindAnyObjectByType<PunchPadView>();

            if (powerMeter == null)
                powerMeter = FindAnyObjectByType<PunchPowerMeterView>();
        }

        PunchingBallViewModel CreateViewModel()
        {
            return new PunchingBallViewModel(startingPunches, winScoreThreshold, maxScore, maxImpactSpeed);
        }

        void Subscribe()
        {
            if (punchPad != null)
                punchPad.Punched += OnPunched;
        }

        void Unsubscribe()
        {
            if (punchPad != null)
                punchPad.Punched -= OnPunched;
        }

        void OnPunched(PunchPadView pad, float impactMagnitude)
        {
            if (_viewModel == null)
                return;

            if (!_viewModel.NotifyPunch(impactMagnitude))
                return;

            powerMeter?.SetPower(_viewModel.LastPunchPower, _viewModel.MaxScore);
            UpdatePadAvailability();
        }

        void UpdatePadAvailability()
        {
            if (punchPad == null || _viewModel == null)
                return;

            punchPad.SetAcceptingPunches(_viewModel.CanPunch);
        }

        public void RestartMatch()
        {
            if (_viewModel == null)
                return;

            _viewModel.Restart();
            punchPad?.ResetVisual();
            powerMeter?.ResetMeter();
            UpdatePadAvailability();
        }

        public void ReturnToFairground()
        {
            FairgroundSession.LeaveAttraction();
            SceneManager.LoadScene(AttractionScenes.Fairground);
        }
    }
}
