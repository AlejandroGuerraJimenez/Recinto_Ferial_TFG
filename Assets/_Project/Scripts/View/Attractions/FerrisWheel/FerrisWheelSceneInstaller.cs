using Fairground.Model.Attractions.FerrisWheel;
using Fairground.View.Attractions.FerrisWheel.Factories;
using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions.FerrisWheel
{
    /// <summary>
    /// Director: builds the basic wheel and an opaque VR rig. The mesh is created at play time.
    /// </summary>
    public sealed class FerrisWheelSceneInstaller : MonoBehaviour
    {
        [SerializeField] GameObject xrOriginPrefab;
        [SerializeField] Material primitiveLitTemplate;
        [SerializeField] float degreesPerSecond = FerrisWheelRules.DefaultDegreesPerSecond;
        [SerializeField] int cabinCount = FerrisWheelRules.DefaultCabinCount;
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

            var wheelFactory = new FerrisWheelFactory();
            wheelFactory.EnsureLighting();
            FerrisWheelRigView rig = wheelFactory.Create(cabinCount);
            AttractionPlacement.TryApplyFromSession(rig.transform);

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
