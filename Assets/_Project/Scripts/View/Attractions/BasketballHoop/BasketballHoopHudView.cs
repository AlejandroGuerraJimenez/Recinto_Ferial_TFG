using Fairground.ViewModel.Attractions.BasketballHoop;
using TMPro;
using UnityEngine;

namespace Fairground.View.Attractions.BasketballHoop
{
    /// <summary>
    /// HUD Observer bound to <see cref="BasketballHoopViewModel"/>.
    /// </summary>
    public sealed class BasketballHoopHudView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI scoreText;
        [SerializeField] TextMeshProUGUI ballsText;
        [SerializeField] TextMeshProUGUI statusText;

        BasketballHoopViewModel _viewModel;

        public void AssignTexts(TextMeshProUGUI score, TextMeshProUGUI ballsRemaining, TextMeshProUGUI status)
        {
            scoreText = score;
            ballsText = ballsRemaining;
            statusText = status;
        }

        public void Bind(BasketballHoopViewModel viewModel)
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

            SetText(scoreText, $"Score: {_viewModel.Score}");
            SetText(ballsText, $"Balls: {_viewModel.BallsRemaining}");
            SetText(statusText, _viewModel.StatusText);
        }

        static void SetText(TextMeshProUGUI label, string value)
        {
            if (label != null)
                label.text = value;
        }
    }
}
