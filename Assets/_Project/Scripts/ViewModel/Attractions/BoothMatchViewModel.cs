using System;

namespace Fairground.ViewModel.Attractions
{
    /// <summary>
    /// Booth presentation shell. The session owns the match; this type publishes the change.
    /// </summary>
    public abstract class BoothMatchViewModel
    {
        public event Action StateChanged;

        public abstract string[] HudLines { get; protected set; }

        public abstract void StartGame();

        public abstract void Restart();

        protected bool Apply(Func<bool> action)
        {
            if (!action())
                return false;

            Notify();
            return true;
        }

        protected void Notify()
        {
            PublishState();
            StateChanged?.Invoke();
        }

        protected abstract void PublishState();
    }
}
