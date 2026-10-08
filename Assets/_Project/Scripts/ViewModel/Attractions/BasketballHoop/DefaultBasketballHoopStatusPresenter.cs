using Fairground.Model.Attractions.BasketballHoop;

namespace Fairground.ViewModel.Attractions.BasketballHoop
{
    public sealed class DefaultBasketballHoopStatusPresenter : IBasketballHoopStatusPresenter
    {
        public string Present(BasketballHoopPhase phase)
        {
            switch (phase)
            {
                case BasketballHoopPhase.Playing: return "Shoot the hoop!";
                case BasketballHoopPhase.Won: return "You win!";
                case BasketballHoopPhase.Lost: return "Out of balls!";
                default: return "Ready";
            }
        }
    }
}
