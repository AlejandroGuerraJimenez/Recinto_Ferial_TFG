using UnityEngine;

namespace Fairground.View
{
    public sealed class EditorCameraAim : IAimRaySource
    {
        public bool TryGetRay(out Ray ray)
        {
            var camera = Camera.main;
            if (camera == null)
                return AimRays.Miss(out ray);

            ray = new Ray(camera.transform.position, camera.transform.forward);
            return true;
        }
    }
}
