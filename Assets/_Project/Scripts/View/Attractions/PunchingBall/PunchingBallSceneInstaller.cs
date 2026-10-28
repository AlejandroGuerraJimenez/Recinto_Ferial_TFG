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

        protected override BoothBuilder CreateBuilder()
        {
            return new PunchingBallBoothBuilder(XrOriginPrefab);
        }

        protected override void WireGameplay(BoothBuilder builder)
        {
            var booth = (PunchingBallBoothBuilder)builder;
            var go = new GameObject("PunchingBallGame");
            var controller = go.AddComponent<PunchingBallAttractionController>();
            controller.Configure(
                booth.Pad,
                booth.Meter,
                booth.Hud,
                startingPunches,
                winScoreThreshold,
                maxScore,
                maxImpactSpeed);
            AttachPlaySupport(booth.XrOrigin, controller.Input, PunchingBallDesktopPlay.Profile(booth.Pad));
        }
    }
}
