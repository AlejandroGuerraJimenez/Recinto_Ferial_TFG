using Fairground.ViewModel.Attractions;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Paints the booth lines whenever the match publishes a new state.
    /// </summary>
    public static class BoothHudBinding
    {
        public static void Attach(BoothHudView hud, BoothMatchViewModel match)
        {
            if (hud == null || match == null)
                return;

            hud.Bind(
                () => match.HudLines,
                handler => match.StateChanged += handler,
                handler => match.StateChanged -= handler);
        }
    }
}
