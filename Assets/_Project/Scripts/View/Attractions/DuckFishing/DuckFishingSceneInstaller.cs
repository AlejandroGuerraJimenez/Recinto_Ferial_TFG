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

        protected override BoothBuilder CreateBuilder()
        {
            return new DuckFishingBoothBuilder(XrOriginPrefab, duckCount);
        }

        protected override void WireGameplay(BoothBuilder builder)
        {
            var booth = (DuckFishingBoothBuilder)builder;
            var go = new GameObject("DuckFishingGame");
            var controller = go.AddComponent<DuckFishingAttractionController>();
            controller.Configure(
                booth.Ducks,
                booth.Rod,
                booth.Hud,
                booth.Pond,
                pointsPerDuck,
                startingAttempts,
                ducksToWin);
            AttachPlaySupport(booth.XrOrigin, controller.Input, DuckFishingDesktopPlay.Profile(booth.Rod));
        }
    }
}
