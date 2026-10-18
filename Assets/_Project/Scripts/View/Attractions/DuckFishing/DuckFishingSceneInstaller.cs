using Fairground.View.Attractions;
using Fairground.View.Attractions.DuckFishing.Builders;
using Fairground.View.Attractions.DuckFishing.Factories;
using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions.DuckFishing
{
    /// <summary>
    /// Director: wires a playable Duck Fishing booth (opaque VR, no AR).
    /// Static booth props prefer baked scene/prefab over runtime CreatePrimitive.
    /// </summary>
    public sealed class DuckFishingSceneInstaller : MonoBehaviour
    {
        [SerializeField] GameObject xrOriginPrefab;
        [SerializeField] Material primitiveLitTemplate;
        [SerializeField] GameObject boothEnvironmentPrefab;
        [SerializeField] Transform bakedBoothEnvironment;
        [SerializeField] int duckCount = 6;
        [SerializeField] int ducksToWin = 4;
        [SerializeField] int startingAttempts = 8;
        [SerializeField] int pointsPerDuck = 10;
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
            var builder = new DuckFishingBoothBuilder(xrOriginPrefab);
            BuildOrUseEnvironment(builder);
            builder.BuildXrRig().BuildGameplay(duckCount);
            AttractionPlacement.TryApplyFromSession(builder.BoothSpaceRoots());

            WireGameplay(builder);
        }

        void BuildOrUseEnvironment(DuckFishingBoothBuilder builder)
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

        void WireGameplay(DuckFishingBoothBuilder builder)
        {
            CreateController(builder);
            CreateVrCameraSetup(builder.XrOrigin);
        }

        void CreateController(DuckFishingBoothBuilder builder)
        {
            var go = new GameObject("DuckFishingGame");
            var controller = go.AddComponent<DuckFishingAttractionController>();
            controller.Configure(
                builder.Ducks,
                builder.Rod,
                builder.Hud,
                builder.Pond,
                pointsPerDuck,
                startingAttempts,
                ducksToWin);
            CreateDesktopSupport(builder.XrOrigin, builder.Rod, controller);
        }

        static void CreateDesktopSupport(GameObject xrOrigin, FishingRodView rod, DuckFishingAttractionController controller)
        {
            var camera = ResolveCamera(xrOrigin);
            var support = new GameObject("DesktopPlaySupport").AddComponent<DesktopPlaySupport>();
            support.Configure(camera != null ? camera.transform : null, rod, controller.Input);
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
