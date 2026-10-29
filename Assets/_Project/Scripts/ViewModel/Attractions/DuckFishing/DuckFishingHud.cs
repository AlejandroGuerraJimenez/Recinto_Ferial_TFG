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
            return Lines(
                0,
                DuckFishingBalance.StartingAttempts,
                0,
                DuckFishingBalance.DucksToWin,
                DuckFishingBalance.DucksToWin,
                MatchPhase.Playing);
        }

        public static string[] Lines(DuckFishingViewModel viewModel)
        {
            return Lines(
                viewModel.Score,
                viewModel.AttemptsRemaining,
                viewModel.DucksCaught,
                viewModel.DucksToWin,
                viewModel.DucksToWin,
                viewModel.Phase);
        }

        public static string Status(DuckFishingViewModel viewModel)
        {
            return Status(viewModel.DucksToWin, viewModel.Phase);
        }

        static string[] Lines(
            int score,
            int attemptsRemaining,
            int ducksCaught,
            int ducksToWin,
            int statusTarget,
            MatchPhase phase)
        {
            return new[]
            {
                $"Puntos: {score}",
                $"Intentos: {attemptsRemaining}",
                $"Patos: {ducksCaught}/{ducksToWin}",
                Status(statusTarget, phase),
            };
        }

        static string Status(int ducksToWin, MatchPhase phase)
        {
            return MatchHud.Phase(phase, $"¡Atrapa {ducksToWin} patos!", "¡Ganaste!", "¡Sin intentos!");
        }
    }
}
