using Fairground.ViewModel.Attractions.BalloonThrow;
using TMPro;
using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow
{
    /// <summary>
    /// HUD Observer bound to <see cref="BalloonThrowViewModel"/>.
    /// </summary>
    public sealed class BalloonThrowHudView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI scoreText;
        [SerializeField] TextMeshProUGUI throwsText;
        [SerializeField] TextMeshProUGUI balloonsText;
        [SerializeField] TextMeshProUGUI statusText;

        BalloonThrowViewModel _viewModel;

        public void AssignTexts(TextMeshProUGUI score, TextMeshProUGUI throwsRemaining, TextMeshProUGUI balloons, TextMeshProUGUI status)
        {
            scoreText = score;
            throwsText = throwsRemaining;
            balloonsText = balloons;
            statusText = status;
        }

        public void Bind(BalloonThrowViewModel viewModel)
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
            SetText(throwsText, $"Throws: {_viewModel.ThrowsRemaining}");
            SetText(balloonsText, $"Balloons: {_viewModel.BalloonsRemaining}");
            SetText(statusText, _viewModel.StatusText);
        }

        static void SetText(TextMeshProUGUI label, string value)
        {
            if (label != null)
                label.text = value;
        }
    }
}
