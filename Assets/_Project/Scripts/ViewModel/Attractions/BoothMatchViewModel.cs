using System;

namespace Fairground.ViewModel.Attractions
{
    /// <summary>
    /// Booth match shell. The match owns the counters and reports that they changed.
    /// Copy for the HUD lives next to the booth, not here.
    /// </summary>
    public abstract class BoothMatchViewModel
    {
        public event Action StateChanged;

        public abstract void StartMatch();

        public abstract void Restart();

        protected bool Apply(Func<bool> action)
        {
            if (!action())
                return false;

            Notify();
            return true;
        }

        protected void Notify() => StateChanged?.Invoke();
    }
}
