using Fairground.Model.Attractions;

namespace Fairground.ViewModel.Attractions.DuckFishing
{
    public sealed class DefaultDuckFishingStatusPresenter : IDuckFishingStatusPresenter
    {
        public string Present(MatchPhase phase, int ducksToWin)
        {
            switch (phase)
            {
                case MatchPhase.Playing:
                    return $"Catch {ducksToWin} ducks!";
                case MatchPhase.Won:
                    return "You win!";
                case MatchPhase.Lost:
                    return "Out of hooks!";
                default:
                    return "Ready";
            }
        }
    }
}
