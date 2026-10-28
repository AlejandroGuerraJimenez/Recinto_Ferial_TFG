using Fairground.View.Attractions.BalloonThrow.Builders;
using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow
{
    /// <summary>
    /// Wires balloon targets and throw rules onto the shared booth shell.
    /// </summary>
    public sealed class BalloonThrowSceneInstaller : BoothSceneInstaller
    {
        [SerializeField] int balloonCount = 6;
        [SerializeField] int pointsPerBalloon = 10;
        [SerializeField] int startingThrows = 8;

        protected override BoothBuilder CreateBuilder()
        {
            return new BalloonThrowBoothBuilder(XrOriginPrefab, balloonCount);
        }

        protected override void WireGameplay(BoothBuilder builder)
        {
            var booth = (BalloonThrowBoothBuilder)builder;
            var go = new GameObject("BalloonThrowGame");
            var controller = go.AddComponent<BalloonThrowAttractionController>();
            controller.Configure(
                booth.Balloons,
                booth.Spawner,
                booth.Hud,
                pointsPerBalloon,
                startingThrows);
            controller.Receive(Session);
            AttachPlaySupport(booth.XrOrigin, controller.Input, BalloonThrowDesktopPlay.Profile(booth.Spawner));
            booth.EnsureBallReady();
        }
    }
}
