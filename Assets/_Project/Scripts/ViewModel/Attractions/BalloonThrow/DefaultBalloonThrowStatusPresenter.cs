using Fairground.Model.Attractions;

namespace Fairground.ViewModel.Attractions.BalloonThrow
{
    public sealed class DefaultBalloonThrowStatusPresenter : IBalloonThrowStatusPresenter
    {
        public string Present(MatchPhase phase)
        {
            switch (phase)
            {
                case MatchPhase.Playing: return "Pop the balloons!";
                case MatchPhase.Won: return "You win!";
                case MatchPhase.Lost: return "Out of throws!";
                default: return "Ready";
            }
        }
    }
}
