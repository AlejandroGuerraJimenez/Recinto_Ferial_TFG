using Fairground.Model.Attractions.BalloonThrow;

namespace Fairground.ViewModel.Attractions.BalloonThrow
{
    public sealed class DefaultBalloonThrowStatusPresenter : IBalloonThrowStatusPresenter
    {
        public string Present(BalloonThrowPhase phase)
        {
            switch (phase)
            {
                case BalloonThrowPhase.Playing: return "Pop the balloons!";
                case BalloonThrowPhase.Won: return "You win!";
                case BalloonThrowPhase.Lost: return "Out of throws!";
                default: return "Ready";
            }
        }
    }
}
