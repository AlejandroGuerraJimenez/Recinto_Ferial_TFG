using System;
using Fairground.ViewModel.Attractions;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Paints the booth lines whenever the match publishes a new state.
    /// The caller supplies the lines, so the match does not own the copy.
    /// </summary>
    public static class BoothHudBinding
    {
        public static void Attach(BoothHudView hud, BoothMatchViewModel match, Func<string[]> lines)
        {
            if (hud == null || match == null)
                return;

            hud.Bind(
                lines,
                handler => match.StateChanged += handler,
                handler => match.StateChanged -= handler);
        }
    }
}
