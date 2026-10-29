using Fairground.Core.Attractions;
using Fairground.Model.Attractions;

namespace Fairground.ViewModel.Attractions.BasketballHoop
{
    /// <summary>
    /// Hoop HUD copy. The layout's first paint and the live binding both call this.
    /// </summary>
    public static class BasketballHoopHud
    {
        public static string[] Opening()
        {
            return Lines(0, BasketballHoopBalance.StartingBalls, MatchPhase.Playing);
        }

        public static string[] Lines(BasketballHoopViewModel viewModel)
        {
            return Lines(viewModel.Score, viewModel.AttemptsRemaining, viewModel.Phase);
        }

        public static string Status(BasketballHoopViewModel viewModel)
        {
            return Status(viewModel.Phase);
        }

        static string[] Lines(int score, int ballsRemaining, MatchPhase phase)
        {
            return new[]
            {
                $"Puntos: {score}",
                $"Balones: {ballsRemaining}",
                Status(phase),
            };
        }

        static string Status(MatchPhase phase)
        {
            return MatchHud.Phase(phase, "¡A la canasta!", "¡Ganaste!", "¡Sin balones!");
        }
    }
}
