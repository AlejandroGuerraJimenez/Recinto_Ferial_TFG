using Fairground.View.Attractions.BalloonThrow.Builders;
using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow
{
    /// <summary>
    /// Director: builds a playable Balloon Throw booth (opaque VR, no AR).
    /// </summary>
    public sealed class BalloonThrowSceneInstaller : MonoBehaviour
    {
        [SerializeField] GameObject xrOriginPrefab;
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
            AssembleBooth();
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
            CreateDesktopSupport(builder.XrOrigin, builder.Spawner);
            CreateVrCameraSetup(builder.XrOrigin);
        }

        void CreateController(BalloonThrowBoothBuilder builder)
        {
            var go = new GameObject("BalloonThrowGame");
            var controller = go.AddComponent<BalloonThrowAttractionController>();
            controller.Configure(builder.Balloons, builder.Spawner, builder.Hud, 10, startingThrows);
        }

        static void CreateDesktopSupport(GameObject xrOrigin, BallSpawnerView spawner)
        {
            var camera = ResolveCamera(xrOrigin);
            var support = new GameObject("DesktopPlaySupport").AddComponent<DesktopPlaySupport>();
            support.Configure(camera != null ? camera.transform : null, spawner);
        }

        static void CreateVrCameraSetup(GameObject xrOrigin)
        {
            var setup = new GameObject("AttractionVrCameraSetup").AddComponent<AttractionVrCameraSetup>();
            setup.Configure(ResolveCamera(xrOrigin), true);
        }

        static Camera ResolveCamera(GameObject xrOrigin)
        {
            return xrOrigin != null ? xrOrigin.GetComponentInChildren<Camera>() : Camera.main;
        }
    }
}
