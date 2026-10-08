using Fairground.Model.Attractions.DuckFishing;

namespace Fairground.ViewModel.Attractions.DuckFishing
{
    /// <summary>
    /// Strategy: maps match phase to HUD status copy.
    /// </summary>
    public interface IDuckFishingStatusPresenter
    {
        string Present(DuckFishingPhase phase, int ducksToWin);
    }
}
