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

        public static string[] Lines(BalloonThrowViewModel match)
        {
            return Lines(match.Score, match.AttemptsRemaining, match.BalloonsRemaining, match.Phase);
        }

        public static string Status(BalloonThrowViewModel match)
        {
            return Status(match.Phase);
        }

        static string[] Lines(int score, int throwsRemaining, int balloonsRemaining, MatchPhase phase)
        {
            return new[]
            {
                $"Puntos: {score}",
                $"Lanzamientos: {throwsRemaining}",
                $"Globos: {balloonsRemaining}",
                Status(phase),
            };
        }

        static string Status(MatchPhase phase)
        {
            return MatchHud.Phase(phase, "¡Explota los globos!", "¡Ganaste!", "¡Sin lanzamientos!");
        }
    }
}
