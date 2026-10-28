using Fairground.Core.Attractions;
using Fairground.View.Attractions.PunchingBall.Builders;
using Fairground.ViewModel.Attractions.PunchingBall;
using UnityEngine;

namespace Fairground.View.Attractions.PunchingBall
{
    /// <summary>
    /// Wires the punch pad and score rules onto the shared booth shell.
    /// </summary>
    public sealed class PunchingBallSceneInstaller : BoothSceneInstaller<PunchingBallBoothBuilder>
    {
        [SerializeField] int startingPunches = PunchingBallBalance.StartingPunches;
        [SerializeField] int winScoreThreshold = PunchingBallBalance.WinScoreThreshold;
        [SerializeField] int maxScore = PunchingBallBalance.MaxScore;
        [SerializeField] float maxImpactSpeed = PunchingBallBalance.MaxImpactSpeed;

        protected override PunchingBallBoothBuilder CreateBuilder()
        {
            return new PunchingBallBoothBuilder(XrOriginPrefab);
        }

        protected override void WireGameplay(PunchingBallBoothBuilder booth)
        {
            var go = new GameObject("PunchingBallGame");
            var match = new PunchingBallViewModel(startingPunches, winScoreThreshold, maxScore, maxImpactSpeed);
            var controller = go.AddComponent<PunchingBallAttractionController>();
            controller.Configure(booth.Pad, booth.Meter, booth.Hud, match);
            controller.Receive(Session);
            AttachPlaySupport(booth.XrOrigin, controller.Input, PunchingBallDesktopPlay.Profile(booth.Pad));
        }
    }
}
