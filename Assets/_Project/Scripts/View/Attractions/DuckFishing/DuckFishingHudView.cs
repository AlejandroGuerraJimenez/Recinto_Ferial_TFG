using Fairground.ViewModel.Attractions.DuckFishing;
using TMPro;
using UnityEngine;

namespace Fairground.View.Attractions.DuckFishing
{
    /// <summary>
    /// HUD Observer bound to <see cref="DuckFishingViewModel"/>.
    /// </summary>
    public sealed class DuckFishingHudView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI scoreText;
        [SerializeField] TextMeshProUGUI attemptsText;
        [SerializeField] TextMeshProUGUI ducksText;
        [SerializeField] TextMeshProUGUI statusText;

        DuckFishingViewModel _viewModel;

        public void AssignTexts(TextMeshProUGUI score, TextMeshProUGUI attempts, TextMeshProUGUI ducks, TextMeshProUGUI status)
        {
            scoreText = score;
            attemptsText = attempts;
            ducksText = ducks;
            statusText = status;
        }

        public void Bind(DuckFishingViewModel viewModel)
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
            SetText(attemptsText, $"Hooks: {_viewModel.AttemptsRemaining}");
            SetText(ducksText, $"Caught: {_viewModel.DucksCaught}/{_viewModel.DucksToWin}");
            SetText(statusText, _viewModel.StatusText);
        }

        static void SetText(TextMeshProUGUI label, string value)
        {
            if (label != null)
                label.text = value;
        }
    }
}
