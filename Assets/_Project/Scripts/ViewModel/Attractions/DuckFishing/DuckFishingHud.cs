using Fairground.Core.Attractions;
using Fairground.Model.Attractions;

namespace Fairground.ViewModel.Attractions.DuckFishing
{
    /// <summary>
    /// Duck HUD copy. The layout's first paint and the live binding both call this.
    /// </summary>
    public static class DuckFishingHud
    {
        public static string[] Opening()
        {
            return Lines(0, DuckFishingBalance.RoundSeconds, 0, DuckFishingBalance.DuckCount, MatchPhase.Playing);
        }

        public static string[] Lines(DuckFishingViewModel viewModel)
        {
            return Lines(
                viewModel.Score,
                viewModel.SecondsLeft,
                viewModel.DucksCaught,
                viewModel.DuckCount,
                viewModel.Phase);
        }

        public static string Status(DuckFishingViewModel viewModel)
        {
            return Status(viewModel.Phase);
        }

        static string[] Lines(int score, int secondsLeft, int ducksCaught, int duckCount, MatchPhase phase)
        {
            return new[]
            {
                $"Puntos: {score}",
                $"Tiempo: {secondsLeft}",
                $"Patos: {ducksCaught}/{duckCount}",
                Status(phase),
            };
        }

        static string Status(MatchPhase phase)
        {
            return MatchHud.Phase(phase, "¡Engancha el aro!", "¡Tiempo!", "¡Tiempo!");
        }
    }
}
