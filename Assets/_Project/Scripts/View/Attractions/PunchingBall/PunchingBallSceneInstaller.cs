using Fairground.View.Attractions.PunchingBall.Builders;
using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions.PunchingBall
{
    /// <summary>
    /// Director: builds a playable Punching Ball booth (opaque VR, no AR).
    /// </summary>
    public sealed class PunchingBallSceneInstaller : MonoBehaviour
    {
        [SerializeField] GameObject xrOriginPrefab;
        [SerializeField] Material primitiveLitTemplate;
        [SerializeField] int startingPunches = 5;
        [SerializeField] int winScoreThreshold = 700;
        [SerializeField] int maxScore = 999;
        [SerializeField] float maxImpactSpeed = 8f;
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
            var builder = new PunchingBallBoothBuilder(xrOriginPrefab)
                .BuildEnvironment()
                .BuildXrRig()
                .BuildGameplay();

            WireGameplay(builder);
        }

        void WireGameplay(PunchingBallBoothBuilder builder)
        {
            CreateController(builder);
            CreateVrCameraSetup(builder.XrOrigin);
        }

        void CreateController(PunchingBallBoothBuilder builder)
        {
            var go = new GameObject("PunchingBallGame");
            var controller = go.AddComponent<PunchingBallAttractionController>();
            controller.Configure(
                builder.Pad,
                builder.Meter,
                builder.Hud,
                startingPunches,
                winScoreThreshold,
                maxScore,
                maxImpactSpeed);
            CreateDesktopSupport(builder.XrOrigin, builder.Pad, controller);
        }

        static void CreateDesktopSupport(GameObject xrOrigin, PunchPadView pad, PunchingBallAttractionController controller)
        {
            var camera = ResolveCamera(xrOrigin);
            var support = new GameObject("DesktopPlaySupport").AddComponent<DesktopPlaySupport>();
            support.Configure(camera != null ? camera.transform : null, pad, controller.Input);
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
