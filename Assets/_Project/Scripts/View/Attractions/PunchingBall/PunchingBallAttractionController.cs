using Fairground.Core.Attractions;
using Fairground.ViewModel;
using Fairground.ViewModel.Attractions.PunchingBall;
using UnityEngine;

namespace Fairground.View.Attractions.PunchingBall
{
    /// <summary>
    /// View Facade: owns ViewModel and observes punch pad events.
    /// </summary>
    public sealed class PunchingBallAttractionController : MonoBehaviour
    {
        [SerializeField] PunchPadView punchPad;
        [SerializeField] PunchPowerMeterView powerMeter;
        [SerializeField] BoothHudView hud;

        PunchingBallViewModel _viewModel;
        AttractionInput _input;
        FairgroundSession _session;

        public AttractionId AttractionId => AttractionId.PunchingBall;
        public PunchingBallViewModel ViewModel => _viewModel;
        public AttractionInput Input => _input;

        public void Configure(
            PunchPadView pad,
            PunchPowerMeterView meter,
            BoothHudView hudView,
            PunchingBallViewModel match)
        {
            punchPad = pad;
            powerMeter = meter;
            hud = hudView;
            _viewModel = match;
            OpenMatch();
        }

        public void Receive(FairgroundSession session) => _session = session;

        void Awake()
        {
            _input = new AttractionInput();
            _input.Enable();
        }

        void Update()
        {
            if (_input == null)
                return;

            if (_input.RestartPressed)
                RestartMatch();

            if (_input.ReturnToFairgroundPressed)
                AttractionExit.ReturnToFairground(_session);
        }

        void OnDestroy()
        {
            Unsubscribe();
            _input?.Dispose();
            _input = null;
        }

        void OpenMatch()
        {
            if (_viewModel == null)
                return;

            ResolvePad();
            BoothHudBinding.Attach(hud, _viewModel);
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

            if (!_viewModel.TryRegisterPunch(impactMagnitude))
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
    }
}
