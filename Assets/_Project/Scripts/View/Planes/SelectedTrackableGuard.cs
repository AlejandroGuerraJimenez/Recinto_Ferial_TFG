using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Fairground.View
{
    public sealed class SelectedTrackableGuard
    {
        public ARPlane Plane { get; private set; }

        public void Hold(ARPlane plane)
        {
            Plane = plane;
            if (Plane != null)
                Plane.destroyOnRemoval = false;
        }

        public void Release(ARPlaneManager manager)
        {
            if (Plane == null)
                return;

            Plane.destroyOnRemoval = true;
            DestroyIfOrphan(manager);
            Plane = null;
        }

        void DestroyIfOrphan(ARPlaneManager manager)
        {
            if (!IsTracked(manager, Plane))
                Object.Destroy(Plane.gameObject);
        }

        static bool IsTracked(ARPlaneManager manager, ARPlane plane)
        {
            if (manager == null || plane == null)
                return false;

            foreach (var tracked in manager.trackables)
                if (tracked == plane)
                    return true;

            return false;
        }
    }
}
