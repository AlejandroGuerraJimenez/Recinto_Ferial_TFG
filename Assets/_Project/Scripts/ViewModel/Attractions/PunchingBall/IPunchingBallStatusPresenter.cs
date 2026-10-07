using Fairground.Model.Attractions.PunchingBall;

namespace Fairground.ViewModel.Attractions.PunchingBall
{
    /// <summary>
    /// Strategy: maps match phase to HUD status copy.
    /// </summary>
    public interface IPunchingBallStatusPresenter
    {
        string Present(PunchingBallPhase phase, int winScoreThreshold);
    }
}
