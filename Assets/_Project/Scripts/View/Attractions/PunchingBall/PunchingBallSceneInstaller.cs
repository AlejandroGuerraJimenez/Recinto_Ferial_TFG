using Fairground.View.Attractions.PunchingBall.Builders;
using UnityEngine;

namespace Fairground.View.Attractions.PunchingBall
{
    /// <summary>
    /// Wires the punch pad and score rules onto the shared booth shell.
    /// </summary>
    public sealed class PunchingBallSceneInstaller : BoothSceneInstaller
    {
        [SerializeField] int startingPunches = 5;
        [SerializeField] int winScoreThreshold = 700;
        [SerializeField] int maxScore = 999;
        [SerializeField] float maxImpactSpeed = 8f;

        protected override void Assemble()
        {
            var builder = new PunchingBallBoothBuilder(XrOriginPrefab);
            Transform baked = ResolveBakedEnvironment();
            if (baked != null)
                builder.UseBakedEnvironment(baked);
            else
                builder.BuildEnvironment(BoothEnvironmentPrefab);

            builder.BuildXrRig().BuildGameplay();
            AttractionPlacement.TryApplyFromSession(builder.BoothSpaceRoots());

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
            AttachPlaySupport(
                builder.XrOrigin,
                controller.Input,
                PunchingBallDesktopPlay.Profile(builder.Pad));
        }
    }
}
