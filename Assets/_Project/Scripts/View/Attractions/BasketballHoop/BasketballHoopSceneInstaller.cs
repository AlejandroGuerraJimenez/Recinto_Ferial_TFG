using Fairground.View.Attractions.BasketballHoop.Builders;
using Fairground.View.Attractions.BasketballHoop.Factories;
using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions.BasketballHoop
{
    /// <summary>
    /// Director: wires a playable basketball arcade booth (opaque VR, no AR).
    /// Static booth props prefer baked scene/prefab over runtime CreatePrimitive.
    /// </summary>
    public sealed class BasketballHoopSceneInstaller : MonoBehaviour
    {
        [SerializeField] GameObject xrOriginPrefab;
        [SerializeField] Material primitiveLitTemplate;
        [SerializeField] GameObject boothEnvironmentPrefab;
        [SerializeField] Transform bakedBoothEnvironment;
        [SerializeField] int pointsPerBasket = 2;
        [SerializeField] int startingBalls = 10;
        [SerializeField] int winScoreThreshold = 5;
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
            AssembleBooth();
        }

        Material ResolvePrimitiveTemplate()
        {
            if (primitiveLitTemplate != null)
                return primitiveLitTemplate;

            return Resources.Load<Material>("AttractionPrimitiveLit");
        }

        void AssembleBooth()
        {
            var builder = new BasketballHoopBoothBuilder(xrOriginPrefab);
            BuildOrUseEnvironment(builder);
            builder.BuildXrRig().BuildGameplay();

            WireGameplay(builder);
            builder.EnsureBallReady();
        }

        void BuildOrUseEnvironment(BasketballHoopBoothBuilder builder)
        {
            var baked = ResolveBakedEnvironment();
            if (baked != null)
            {
                builder.UseBakedEnvironment(baked);
                return;
            }

            builder.BuildEnvironment(boothEnvironmentPrefab);
        }

        Transform ResolveBakedEnvironment()
        {
            if (bakedBoothEnvironment != null)
                return bakedBoothEnvironment;

            var existing = GameObject.Find(BoothPropFactory.EnvironmentRootName);
            return existing != null ? existing.transform : null;
        }

        void WireGameplay(BasketballHoopBoothBuilder builder)
        {
            CreateController(builder);
            CreateVrCameraSetup(builder.XrOrigin);
        }

        void CreateController(BasketballHoopBoothBuilder builder)
        {
            var go = new GameObject("BasketballHoopGame");
            var controller = go.AddComponent<BasketballHoopAttractionController>();
            controller.Configure(
                builder.Hoop,
                builder.Spawner,
                builder.Hud,
                pointsPerBasket,
                startingBalls,
                winScoreThreshold);
            CreateDesktopSupport(builder.XrOrigin, builder.Spawner, controller);
        }

        static void CreateDesktopSupport(GameObject xrOrigin, BallSpawnerView spawner, BasketballHoopAttractionController controller)
        {
            var camera = ResolveCamera(xrOrigin);
            var support = new GameObject("DesktopPlaySupport").AddComponent<DesktopPlaySupport>();
            support.Configure(camera != null ? camera.transform : null, spawner, controller.Input);
        }

        static void CreateVrCameraSetup(GameObject xrOrigin)
        {
            var setup = new GameObject("AttractionVrCameraSetup").AddComponent<AttractionVrCameraSetup>();
            setup.Configure(ResolveCamera(xrOrigin), skybox: false);
        }

        static Camera ResolveCamera(GameObject xrOrigin)
        {
            return xrOrigin != null ? xrOrigin.GetComponentInChildren<Camera>() : Camera.main;
        }
    }
}
