using System.Collections.Generic;

namespace Fairground.Core.Planes
{
    public static class PlaneArea
    {
        public static float Of(IReadOnlyList<PlanePoint> boundary)
        {
            if (boundary == null || boundary.Count < 3)
                return 0f;

            double sum = 0d;
            for (var i = 0; i < boundary.Count; i++)
            {
                var next = boundary[(i + 1) % boundary.Count];
                var current = boundary[i];
                sum += (double)current.X * next.Y - (double)next.X * current.Y;
            }

            return (float)(System.Math.Abs(sum) * 0.5d);
        }
    }
}
