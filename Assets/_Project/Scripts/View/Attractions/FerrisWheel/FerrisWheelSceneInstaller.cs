using Fairground.View.Attractions.FerrisWheel.Factories;
using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions.FerrisWheel
{
    /// <summary>
    /// Director: builds the London Eye capsule over Cesium and an opaque VR rig.
    /// The globe and the ride are created at play time.
    /// </summary>
    public sealed class FerrisWheelSceneInstaller : MonoBehaviour
    {
        [SerializeField] GameObject xrOriginPrefab;
        [SerializeField] Material primitiveLitTemplate;
        [SerializeField] float degreesPerSecond = LondonEyeLayout.DegreesPerSecond;
        [SerializeField] int cabinCount = LondonEyeLayout.CabinCount;
        [SerializeField] bool buildOnAwake = true;

        bool _built;

        void Awake()
        {
            if (buildOnAwake)
                Build();
        }

        public void Build()
        {
            if (_built)
                return;

            _built = true;
            PrimitiveMaterialApplier.SetTemplate(ResolvePrimitiveTemplate());
            Assemble();
        }

        Material ResolvePrimitiveTemplate()
        {
            if (primitiveLitTemplate != null)
                return primitiveLitTemplate;

            return Resources.Load<Material>("AttractionPrimitiveLit");
        }

        void Assemble()
        {
            var rigFactory = new XrAttractionRigFactory(xrOriginPrefab);
            rigFactory.EnsureInteractionManager();
            rigFactory.EnsureEventSystem();
            GameObject xrOrigin = rigFactory.EnsureXrOrigin();

            var ride = new LondonEyeRideFactory();
            FerrisWheelRigView rig = ride.Create(xrOrigin);

            var controller = CreateController(rig);
            CreateVrCameraSetup(xrOrigin);
            CreateDesktopSupport(xrOrigin, controller);
        }

        FerrisWheelAttractionController CreateController(FerrisWheelRigView rig)
        {
            var go = new GameObject("FerrisWheelGame");
            var controller = go.AddComponent<FerrisWheelAttractionController>();
            controller.Configure(rig, degreesPerSecond, cabinCount);
            return controller;
        }

        static void CreateVrCameraSetup(GameObject xrOrigin)
        {
            var setup = new GameObject("AttractionVrCameraSetup").AddComponent<AttractionVrCameraSetup>();
            setup.Configure(ResolveCamera(xrOrigin), skybox: false);
        }

        static void CreateDesktopSupport(GameObject xrOrigin, FerrisWheelAttractionController controller)
        {
            var camera = ResolveCamera(xrOrigin);
            var support = new GameObject("DesktopPlaySupport").AddComponent<DesktopPlaySupport>();
            support.Configure(camera != null ? camera.transform : null, controller.Input);
        }

        static Camera ResolveCamera(GameObject xrOrigin)
        {
            return xrOrigin != null ? xrOrigin.GetComponentInChildren<Camera>() : Camera.main;
        }
    }
}
