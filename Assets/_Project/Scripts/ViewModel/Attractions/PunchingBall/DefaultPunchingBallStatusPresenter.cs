using Fairground.Model.Attractions.PunchingBall;

namespace Fairground.ViewModel.Attractions.PunchingBall
{
    public sealed class DefaultPunchingBallStatusPresenter : IPunchingBallStatusPresenter
    {
        public string Present(PunchingBallPhase phase, int winScoreThreshold)
        {
            switch (phase)
            {
                case PunchingBallPhase.Playing:
                    return $"Hit {winScoreThreshold}+ to win!";
                case PunchingBallPhase.Won:
                    return "Strongman!";
                case PunchingBallPhase.Lost:
                    return "Too weak!";
                default:
                    return "Ready";
            }
        }
    }
}
