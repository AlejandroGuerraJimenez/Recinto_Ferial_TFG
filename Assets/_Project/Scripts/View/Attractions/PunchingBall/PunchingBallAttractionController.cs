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
        AttractionMatchControls _controls;

        public AttractionId AttractionId => AttractionId.PunchingBall;
        public PunchingBallViewModel ViewModel => _viewModel;
        public AttractionInput Input => _controls != null ? _controls.Input : null;

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

        public void Receive(FairgroundSession session) => _controls?.Use(session);

        void Awake() => _controls = new AttractionMatchControls(RestartMatch);

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

            ResolvePad();
            BoothHudBinding.Attach(hud, _viewModel, () => PunchingBallHud.Lines(_viewModel));
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
