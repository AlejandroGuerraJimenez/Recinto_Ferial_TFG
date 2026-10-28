using Fairground.Core.Attractions;
using Fairground.View.Attractions.BalloonThrow.Builders;
using Fairground.ViewModel.Attractions.BalloonThrow;
using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow
{
    /// <summary>
    /// Wires balloon targets and throw rules onto the shared booth shell.
    /// </summary>
    public sealed class BalloonThrowSceneInstaller : BoothSceneInstaller<BalloonThrowBoothBuilder>
    {
        [SerializeField] int balloonCount = BalloonThrowBalance.BalloonCount;
        [SerializeField] int pointsPerBalloon = BalloonThrowBalance.PointsPerBalloon;
        [SerializeField] int startingThrows = BalloonThrowBalance.StartingThrows;

        protected override BalloonThrowBoothBuilder CreateBuilder()
        {
            return new BalloonThrowBoothBuilder(XrOriginPrefab, balloonCount);
        }

        protected override void WireGameplay(BalloonThrowBoothBuilder booth)
        {
            var go = new GameObject("BalloonThrowGame");
            int count = booth.Balloons != null ? Mathf.Max(1, booth.Balloons.Length) : 1;
            var match = new BalloonThrowViewModel(pointsPerBalloon, startingThrows, count);
            var controller = go.AddComponent<BalloonThrowAttractionHost>();
            controller.Configure(booth.Balloons, booth.Spawner, booth.Hud, match);
            controller.Receive(Visit);
            AttachPlaySupport(booth.XrOrigin, controller.Input, BalloonThrowDesktopPlay.Profile(booth.Spawner));
            booth.EnsureBallReady();
        }
    }
}
