using System;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// The scene installer is the only place that wires a booth. A missing part stops the match.
    /// </summary>
    static class BoothWiring
    {
        public static T Require<T>(T part, string booth, string partName) where T : class
        {
            if (part == null)
                throw new InvalidOperationException(booth + " needs " + partName + ". The scene installer wires it.");

            return part;
        }

        public static T[] RequireAny<T>(T[] parts, string booth, string partName) where T : class
        {
            if (parts == null || parts.Length == 0 || parts[0] == null)
                throw new InvalidOperationException(booth + " needs " + partName + ". The scene installer wires it.");

            return parts;
        }
    }
}
