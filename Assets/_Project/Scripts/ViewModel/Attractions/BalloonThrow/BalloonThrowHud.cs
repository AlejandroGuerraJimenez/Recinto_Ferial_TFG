using Fairground.Core.Attractions;
using Fairground.Model.Attractions;

namespace Fairground.ViewModel.Attractions.BalloonThrow
{
    /// <summary>
    /// Balloon HUD copy. The layout's first paint and the live binding both call this.
    /// </summary>
    public static class BalloonThrowHud
    {
        public static string[] Opening()
        {
            return Lines(0, BalloonThrowBalance.StartingThrows, BalloonThrowBalance.BalloonCount, MatchPhase.Playing);
        }

        public static string[] Lines(BalloonThrowViewModel viewModel)
        {
            return Lines(viewModel.Score, viewModel.AttemptsRemaining, viewModel.BalloonsRemaining, viewModel.Phase);
        }

        public static string Status(BalloonThrowViewModel viewModel)
        {
            return Status(viewModel.Phase);
        }

        static string[] Lines(int score, int throwsRemaining, int balloonsRemaining, MatchPhase phase)
        {
            return new[]
            {
                $"Puntos: {score}",
                $"Lanzamientos: {throwsRemaining}",
                $"Dianas: {balloonsRemaining}",
                Status(phase),
            };
        }

        static string Status(MatchPhase phase)
        {
            return MatchHud.Phase(phase, "¡A la diana!", "¡Ganaste!", "¡Sin lanzamientos!");
        }
    }
}
