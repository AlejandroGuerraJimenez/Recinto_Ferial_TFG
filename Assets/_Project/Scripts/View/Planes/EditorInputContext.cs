using System;

namespace Fairground.View
{
    public readonly struct EditorInputContext
    {
        public EditorInputContext(Action restart, Action confirm, Func<bool> isAwaiting, PlaneAimController aim, float distance)
        {
            Restart = restart;
            Confirm = confirm;
            IsAwaiting = isAwaiting;
            Aim = aim;
            Distance = distance;
        }

        public Action Restart { get; }

        public Action Confirm { get; }

        public Func<bool> IsAwaiting { get; }

        public PlaneAimController Aim { get; }

        public float Distance { get; }
    }
}
