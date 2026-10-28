using Fairground.Model.Attractions.PunchingBall;
using Fairground.View.Attractions.PunchingBall.Builders;
using UnityEngine;

namespace Fairground.View.Attractions.PunchingBall
{
    /// <summary>
    /// Wires the punch pad and score rules onto the shared booth shell.
    /// </summary>
    public sealed class PunchingBallSceneInstaller : BoothSceneInstaller
    {
        [SerializeField] int startingPunches = PunchingBallRules.DefaultStartingPunches;
        [SerializeField] int winScoreThreshold = PunchingBallRules.DefaultWinScoreThreshold;
        [SerializeField] int maxScore = PunchingBallRules.DefaultMaxScore;
        [SerializeField] float maxImpactSpeed = PunchingBallRules.DefaultMaxImpactSpeed;

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
            controller.Receive(Session);
            AttachPlaySupport(booth.XrOrigin, controller.Input, PunchingBallDesktopPlay.Profile(booth.Pad));
        }
    }
}
