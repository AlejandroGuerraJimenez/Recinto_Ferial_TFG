using UnityEngine;

namespace Fairground.View
{
    public interface IAimRaySource
    {
        bool TryGetRay(out Ray ray);
    }
}
