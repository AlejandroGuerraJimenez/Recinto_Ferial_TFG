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
        [SerializeField] int startingThrows = 8;

        protected override void Assemble()
        {
            var builder = new BalloonThrowBoothBuilder(XrOriginPrefab);
            Transform baked = ResolveBakedEnvironment();
            if (baked != null)
                builder.UseBakedEnvironment(baked);
            else
                builder.BuildEnvironment(BoothEnvironmentPrefab);

            builder.BuildXrRig().BuildGameplay(balloonCount);
            AttractionPlacement.TryApplyFromSession(builder.BoothSpaceRoots());

            var go = new GameObject("BalloonThrowGame");
            var controller = go.AddComponent<BalloonThrowAttractionController>();
            controller.Configure(builder.Balloons, builder.Spawner, builder.Hud, 10, startingThrows);
            AttachPlaySupport(
                builder.XrOrigin,
                controller.Input,
                BalloonThrowDesktopPlay.Profile(builder.Spawner));
            builder.EnsureBallReady();
        }
    }
}
