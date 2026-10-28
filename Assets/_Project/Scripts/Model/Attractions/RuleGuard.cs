using System;

namespace Fairground.Model.Attractions
{
    /// <summary>
    /// Range checks shared by booth rules. A new booth calls these instead of copying the guards.
    /// </summary>
    internal static class RuleGuard
    {
        public static void NonNegative(int value, string name)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(name);
        }

        public static void Positive(int value, string name)
        {
            if (value < 1)
                throw new ArgumentOutOfRangeException(name);
        }

        public static void Positive(float value, string name)
        {
            if (value <= 0f)
                throw new ArgumentOutOfRangeException(name);
        }
    }
}
