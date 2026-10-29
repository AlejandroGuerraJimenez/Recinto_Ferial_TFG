using Fairground.Core.Attractions;
using Fairground.Model.Attractions;

namespace Fairground.ViewModel.Attractions.PunchingBall
{
    /// <summary>
    /// Punch HUD copy. The layout's first paint and the live binding both call this.
    /// </summary>
    public static class PunchingBallHud
    {
        public static string[] Opening()
        {
            return Lines(0, PunchingBallBalance.StartingPunches, 0, PunchingBallBalance.WinScoreThreshold, MatchPhase.Playing);
        }

        public static string[] Lines(PunchingBallViewModel viewModel)
        {
            return Lines(
                viewModel.BestScore,
                viewModel.PunchesRemaining,
                viewModel.LastPunchPower,
                viewModel.WinScoreThreshold,
                viewModel.Phase);
        }

        public static string Status(PunchingBallViewModel viewModel)
        {
            return Status(viewModel.WinScoreThreshold, viewModel.Phase);
        }

        static string[] Lines(int bestScore, int punchesRemaining, int lastPunch, int winScoreThreshold, MatchPhase phase)
        {
            return new[]
            {
                $"Mejor: {bestScore}",
                $"Golpes: {punchesRemaining}",
                $"Último: {lastPunch}",
                Status(winScoreThreshold, phase),
            };
        }

        static string Status(int winScoreThreshold, MatchPhase phase)
        {
            return MatchHud.Phase(phase, $"¡Supera {winScoreThreshold}!", "¡Fuerza!", "¡Muy flojo!");
        }
    }
}
