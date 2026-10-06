using UnityEngine;

namespace Fairground.View
{
    public static class AimRays
    {
        public static bool Miss(out Ray ray)
        {
            ray = default;
            return false;
        }
    }
}
