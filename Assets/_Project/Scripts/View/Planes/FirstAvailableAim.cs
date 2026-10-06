using UnityEngine;

namespace Fairground.View
{
    public sealed class FirstAvailableAim : IAimRaySource
    {
        readonly IAimRaySource _first;
        readonly IAimRaySource _second;

        public FirstAvailableAim(IAimRaySource first, IAimRaySource second)
        {
            _first = first;
            _second = second;
        }

        public bool TryGetRay(out Ray ray)
        {
            if (_first.TryGetRay(out ray))
                return true;

            return _second.TryGetRay(out ray);
        }
    }
}
