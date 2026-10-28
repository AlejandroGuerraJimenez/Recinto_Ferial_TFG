using System;
using Fairground.Core.Attractions;

namespace Fairground.Model.Attractions.FerrisWheel
{
    /// <summary>
    /// Immutable spin policy. How many cabins the wheel has lives in the view.
    /// </summary>
    public sealed class FerrisWheelRules
    {
        public const float DefaultDegreesPerSecond = FerrisWheelBalance.DegreesPerSecond;

        public FerrisWheelRules(float degreesPerSecond = DefaultDegreesPerSecond)
        {
            if (degreesPerSecond <= 0f)
                throw new ArgumentOutOfRangeException(nameof(degreesPerSecond));

            DegreesPerSecond = degreesPerSecond;
        }

        public float DegreesPerSecond { get; }
    }
}
