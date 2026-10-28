using Fairground.View.Attractions.DuckFishing.Builders;
using UnityEngine;

namespace Fairground.View.Attractions.DuckFishing
{
    /// <summary>
    /// Wires ducks, the rod, and catch rules onto the shared booth shell.
    /// </summary>
    public sealed class DuckFishingSceneInstaller : BoothSceneInstaller
    {
        [SerializeField] int duckCount = 6;
        [SerializeField] int ducksToWin = 4;
        [SerializeField] int startingAttempts = 8;
        [SerializeField] int pointsPerDuck = 10;

        protected override void Assemble()
        {
            var builder = new DuckFishingBoothBuilder(XrOriginPrefab);
            Transform baked = ResolveBakedEnvironment();
            if (baked != null)
                builder.UseBakedEnvironment(baked);
            else
                builder.BuildEnvironment(BoothEnvironmentPrefab);

            builder.BuildXrRig().BuildGameplay(duckCount);
            AttractionPlacement.TryApplyFromSession(builder.BoothSpaceRoots());

            var go = new GameObject("DuckFishingGame");
            var controller = go.AddComponent<DuckFishingAttractionController>();
            controller.Configure(
                builder.Ducks,
                builder.Rod,
                builder.Hud,
                builder.Pond,
                pointsPerDuck,
                startingAttempts,
                ducksToWin);
            AttachPlaySupport(
                builder.XrOrigin,
                controller.Input,
                DuckFishingDesktopPlay.Profile(builder.Rod));
        }
    }
}
