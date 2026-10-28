using Fairground.Model.Attractions;

namespace Fairground.ViewModel.Attractions
{
    /// <summary>
    /// Won, lost, and playing lines shared by booth copy. Each booth owns its own sentences.
    /// </summary>
    internal static class MatchHud
    {
        public static string Phase(MatchPhase phase, string playing, string won, string lost)
        {
            switch (phase)
            {
                case MatchPhase.Playing: return playing;
                case MatchPhase.Won: return won;
                case MatchPhase.Lost: return lost;
                default: return "Listo";
            }
        }
    }
}
