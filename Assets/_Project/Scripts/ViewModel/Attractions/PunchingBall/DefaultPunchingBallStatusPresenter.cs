using Fairground.Model.Attractions;

namespace Fairground.ViewModel.Attractions.PunchingBall
{
    public sealed class DefaultPunchingBallStatusPresenter : IPunchingBallStatusPresenter
    {
        public string Present(MatchPhase phase, int winScoreThreshold)
        {
            switch (phase)
            {
                case MatchPhase.Playing:
                    return $"Hit {winScoreThreshold}+ to win!";
                case MatchPhase.Won:
                    return "Strongman!";
                case MatchPhase.Lost:
                    return "Too weak!";
                default:
                    return "Ready";
            }
        }
    }
}
