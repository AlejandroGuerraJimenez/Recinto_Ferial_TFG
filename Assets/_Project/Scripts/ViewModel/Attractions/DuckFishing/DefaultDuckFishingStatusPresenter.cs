using Fairground.Model.Attractions.DuckFishing;

namespace Fairground.ViewModel.Attractions.DuckFishing
{
    public sealed class DefaultDuckFishingStatusPresenter : IDuckFishingStatusPresenter
    {
        public string Present(DuckFishingPhase phase, int ducksToWin)
        {
            switch (phase)
            {
                case DuckFishingPhase.Playing:
                    return $"Catch {ducksToWin} ducks!";
                case DuckFishingPhase.Won:
                    return "You win!";
                case DuckFishingPhase.Lost:
                    return "Out of hooks!";
                default:
                    return "Ready";
            }
        }
    }
}
