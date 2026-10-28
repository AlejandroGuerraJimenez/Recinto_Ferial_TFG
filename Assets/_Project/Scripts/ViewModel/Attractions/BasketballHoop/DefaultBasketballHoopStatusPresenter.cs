using Fairground.Model.Attractions;

namespace Fairground.ViewModel.Attractions.BasketballHoop
{
    public sealed class DefaultBasketballHoopStatusPresenter : IBasketballHoopStatusPresenter
    {
        public string Present(MatchPhase phase)
        {
            switch (phase)
            {
                case MatchPhase.Playing: return "Shoot the hoop!";
                case MatchPhase.Won: return "You win!";
                case MatchPhase.Lost: return "Out of balls!";
                default: return "Ready";
            }
        }
    }
}
