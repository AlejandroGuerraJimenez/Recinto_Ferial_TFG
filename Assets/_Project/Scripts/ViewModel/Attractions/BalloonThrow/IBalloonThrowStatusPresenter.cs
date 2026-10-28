using Fairground.Model.Attractions;

namespace Fairground.ViewModel.Attractions.BalloonThrow
{
    /// <summary>
    /// Strategy: maps match phase to HUD status copy.
    /// </summary>
    public interface IBalloonThrowStatusPresenter
    {
        string Present(MatchPhase phase);
    }
}
