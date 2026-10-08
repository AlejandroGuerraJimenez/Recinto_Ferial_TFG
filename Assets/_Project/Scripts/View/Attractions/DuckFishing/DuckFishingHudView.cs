using Fairground.ViewModel.Attractions.DuckFishing;
using UnityEngine;
using UnityEngine.UI;

namespace Fairground.View.Attractions.DuckFishing
{
    /// <summary>
    /// HUD Observer bound to <see cref="DuckFishingViewModel"/>.
    /// </summary>
    public sealed class DuckFishingHudView : MonoBehaviour
    {
        [SerializeField] Text scoreText;
        [SerializeField] Text attemptsText;
        [SerializeField] Text ducksText;
        [SerializeField] Text statusText;

        DuckFishingViewModel _viewModel;

        public void AssignTexts(Text score, Text attempts, Text ducks, Text status)
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

        static void SetText(Text label, string value)
        {
            if (label != null)
                label.text = value;
        }
    }
}
