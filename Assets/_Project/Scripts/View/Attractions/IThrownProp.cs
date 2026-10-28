using System;
using UnityEngine;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Something a booth can launch, then resolve. Balloons and basketballs share this.
    /// </summary>
    public interface IThrownProp
    {
        event Action<IThrownProp> Thrown;
        event Action<IThrownProp> Resolved;
        event Action<IThrownProp> Destroyed;
        bool IsThrown { get; }
        void Launch(Vector3 velocity);
    }
}
