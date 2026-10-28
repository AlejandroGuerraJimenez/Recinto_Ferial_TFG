using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Steps every attraction scene shares: lit template, baked booth lookup, opaque camera, desktop play.
    /// </summary>
    public static class AttractionSceneSupport
    {
        public static Material ResolvePrimitiveTemplate(Material assigned)
        {
            if (assigned != null)
                return assigned;

            return Resources.Load<Material>("AttractionPrimitiveLit");
        }

        public static Transform ResolveBakedEnvironment(Transform assigned)
        {
            if (assigned != null)
                return assigned;

            var existing = GameObject.Find(BoothPropFactory.EnvironmentRootName);
            return existing != null ? existing.transform : null;
        }

        public static Camera ResolveCamera(GameObject xrOrigin)
        {
            return xrOrigin != null ? xrOrigin.GetComponentInChildren<Camera>() : Camera.main;
        }

        public static void CreateVrCamera(GameObject xrOrigin)
        {
            var setup = new GameObject("AttractionVrCameraSetup").AddComponent<AttractionVrCameraSetup>();
            setup.Configure(ResolveCamera(xrOrigin), skybox: false);
        }

        public static void CreateDesktopPlay(GameObject xrOrigin, AttractionInput input, DesktopPlayProfile profile)
        {
            Camera camera = ResolveCamera(xrOrigin);
            var support = new GameObject("DesktopPlaySupport").AddComponent<DesktopPlaySupport>();
            support.Configure(camera != null ? camera.transform : null, input, profile);
        }
    }
}
