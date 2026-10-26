using UnityEngine;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Head pose when the surface was confirmed. The next scene places the booth from this offset,
    /// because a reloaded XR origin does not keep the previous world coordinates.
    /// </summary>
    public static class AttractionViewpoint
    {
        public static bool HasValue { get; private set; }

        public static Vector3 Position { get; private set; }

        public static float Yaw { get; private set; }

        public static void Remember(Vector3 position, float yaw)
        {
            Position = position;
            Yaw = yaw;
            HasValue = true;
        }

        public static void Clear() => HasValue = false;
    }
}
