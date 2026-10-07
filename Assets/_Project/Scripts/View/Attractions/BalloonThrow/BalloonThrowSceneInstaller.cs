using Fairground.View.Attractions.BalloonThrow.Builders;
using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow
{
    /// <summary>
    /// Director: builds a playable Balloon Throw booth (opaque VR, no AR).
    /// </summary>
    public sealed class BalloonThrowSceneInstaller : MonoBehaviour
    {
        [SerializeField] GameObject xrOriginPrefab;
        [SerializeField] Material primitiveLitTemplate;
        [SerializeField] int balloonCount = 6;
        [SerializeField] int startingThrows = 8;
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
            var builder = new BalloonThrowBoothBuilder(xrOriginPrefab)
                .BuildEnvironment()
                .BuildXrRig()
                .BuildGameplay(balloonCount);

            WireGameplay(builder);
            builder.EnsureBallReady();
        }

        void WireGameplay(BalloonThrowBoothBuilder builder)
        {
            CreateController(builder);
            CreateVrCameraSetup(builder.XrOrigin);
        }

        void CreateController(BalloonThrowBoothBuilder builder)
        {
            var go = new GameObject("BalloonThrowGame");
            var controller = go.AddComponent<BalloonThrowAttractionController>();
            controller.Configure(builder.Balloons, builder.Spawner, builder.Hud, 10, startingThrows);
            CreateDesktopSupport(builder.XrOrigin, builder.Spawner, controller);
        }

        static void CreateDesktopSupport(GameObject xrOrigin, BallSpawnerView spawner, BalloonThrowAttractionController controller)
        {
            var camera = ResolveCamera(xrOrigin);
            var support = new GameObject("DesktopPlaySupport").AddComponent<DesktopPlaySupport>();
            support.Configure(camera != null ? camera.transform : null, spawner, controller.Input);
        }

        static void CreateVrCameraSetup(GameObject xrOrigin)
        {
            var setup = new GameObject("AttractionVrCameraSetup").AddComponent<AttractionVrCameraSetup>();
            // Solid opaque clear: reliable in Editor Game View and Quest (no passthrough).
            setup.Configure(ResolveCamera(xrOrigin), skybox: false);
        }

        static Camera ResolveCamera(GameObject xrOrigin)
        {
            return xrOrigin != null ? xrOrigin.GetComponentInChildren<Camera>() : Camera.main;
        }
    }
}
