using Fairground.ViewModel.Attractions.PunchingBall;
using UnityEngine;
using UnityEngine.UI;

namespace Fairground.View.Attractions.PunchingBall
{
    /// <summary>
    /// HUD Observer bound to <see cref="PunchingBallViewModel"/>.
    /// </summary>
    public sealed class PunchingBallHudView : MonoBehaviour
    {
        [SerializeField] Text scoreText;
        [SerializeField] Text punchesText;
        [SerializeField] Text lastPunchText;
        [SerializeField] Text statusText;

        PunchingBallViewModel _viewModel;

        public void AssignTexts(Text score, Text punches, Text lastPunch, Text status)
        {
            scoreText = score;
            punchesText = punches;
            lastPunchText = lastPunch;
            statusText = status;
        }

        public void Bind(PunchingBallViewModel viewModel)
        {
            Unbind();
            _viewModel = viewModel;
            if (_viewModel == null)
                return;

            _viewModel.StateChanged += Refresh;
            Refresh();
        }

        void OnDestroy() => Unbind();

        void Unbind()
        {
            if (_viewModel == null)
                return;

            _viewModel.StateChanged -= Refresh;
            _viewModel = null;
        }

        void Refresh()
        {
            if (_viewModel == null)
                return;

            SetText(scoreText, $"Best: {_viewModel.BestScore}");
            SetText(punchesText, $"Punches: {_viewModel.PunchesRemaining}");
            SetText(lastPunchText, $"Last: {_viewModel.LastPunchPower}");
            SetText(statusText, _viewModel.StatusText);
        }

        static void SetText(Text label, string value)
        {
            if (label != null)
                label.text = value;
        }
    }
}
