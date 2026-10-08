using Fairground.Model.Attractions.BasketballHoop;

namespace Fairground.ViewModel.Attractions.BasketballHoop
{
    /// <summary>
    /// Strategy: maps match phase to HUD status copy.
    /// </summary>
    public interface IBasketballHoopStatusPresenter
    {
        string Present(BasketballHoopPhase phase);
    }
}
