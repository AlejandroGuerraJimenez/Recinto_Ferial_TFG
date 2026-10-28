using Fairground.Model.Attractions;

namespace Fairground.ViewModel.Attractions.DuckFishing
{
    /// <summary>
    /// Strategy: maps match phase to HUD status copy.
    /// </summary>
    public interface IDuckFishingStatusPresenter
    {
        string Present(MatchPhase phase, int ducksToWin);
    }
}
