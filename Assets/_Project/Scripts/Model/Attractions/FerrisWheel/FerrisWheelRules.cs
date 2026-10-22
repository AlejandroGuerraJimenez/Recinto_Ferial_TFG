using System;

namespace Fairground.Model.Attractions.FerrisWheel
{
    /// <summary>
    /// Immutable spin policy for the basic wheel. Geometry lives in the view.
    /// </summary>
    public sealed class FerrisWheelRules
    {
        public const float DefaultDegreesPerSecond = 16f;
        public const int DefaultCabinCount = 8;

        public FerrisWheelRules(
            float degreesPerSecond = DefaultDegreesPerSecond,
            int cabinCount = DefaultCabinCount)
        {
            if (degreesPerSecond <= 0f)
                throw new ArgumentOutOfRangeException(nameof(degreesPerSecond));
            if (cabinCount < 1)
                throw new ArgumentOutOfRangeException(nameof(cabinCount));

            DegreesPerSecond = degreesPerSecond;
            CabinCount = cabinCount;
        }

        public float DegreesPerSecond { get; }
        public int CabinCount { get; }
    }
}
