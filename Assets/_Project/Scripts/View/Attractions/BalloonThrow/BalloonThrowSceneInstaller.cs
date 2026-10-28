using Fairground.Core.Attractions;
using Fairground.View.Attractions.BalloonThrow.Builders;
using Fairground.ViewModel.Attractions.BalloonThrow;
using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow
{
    /// <summary>
    /// Wires balloon targets and throw rules onto the shared booth shell.
    /// </summary>
    public sealed class BalloonThrowSceneInstaller : BoothSceneInstaller
    {
        [SerializeField] int balloonCount = BalloonThrowBalance.BalloonCount;
        [SerializeField] int pointsPerBalloon = BalloonThrowBalance.PointsPerBalloon;
        [SerializeField] int startingThrows = BalloonThrowBalance.StartingThrows;

        protected override BoothBuilder CreateBuilder()
        {
            return new BalloonThrowBoothBuilder(XrOriginPrefab, balloonCount);
        }

        protected override void WireGameplay(BoothBuilder builder)
        {
            var booth = (BalloonThrowBoothBuilder)builder;
            var go = new GameObject("BalloonThrowGame");
            int count = booth.Balloons != null ? Mathf.Max(1, booth.Balloons.Length) : 1;
            var match = new BalloonThrowViewModel(pointsPerBalloon, startingThrows, count);
            var controller = go.AddComponent<BalloonThrowAttractionController>();
            controller.Configure(booth.Balloons, booth.Spawner, booth.Hud, match);
            controller.Receive(Session);
            AttachPlaySupport(booth.XrOrigin, controller.Input, BalloonThrowDesktopPlay.Profile(booth.Spawner));
            booth.EnsureBallReady();
        }
    }
}
