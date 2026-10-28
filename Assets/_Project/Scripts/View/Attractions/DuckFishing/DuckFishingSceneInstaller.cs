using Fairground.Core.Attractions;
using Fairground.View.Attractions.DuckFishing.Builders;
using Fairground.ViewModel.Attractions.DuckFishing;
using UnityEngine;

namespace Fairground.View.Attractions.DuckFishing
{
    /// <summary>
    /// Wires ducks, the rod, and catch rules onto the shared booth shell.
    /// </summary>
    public sealed class DuckFishingSceneInstaller : BoothSceneInstaller
    {
        [SerializeField] int duckCount = DuckFishingBalance.DuckCount;
        [SerializeField] int ducksToWin = DuckFishingBalance.DucksToWin;
        [SerializeField] int startingAttempts = DuckFishingBalance.StartingAttempts;
        [SerializeField] int pointsPerDuck = DuckFishingBalance.PointsPerDuck;

        protected override BoothBuilder CreateBuilder()
        {
            return new DuckFishingBoothBuilder(XrOriginPrefab, duckCount);
        }

        protected override void WireGameplay(BoothBuilder builder)
        {
            var booth = (DuckFishingBoothBuilder)builder;
            var go = new GameObject("DuckFishingGame");
            int count = booth.Ducks != null ? Mathf.Max(1, booth.Ducks.Length) : 1;
            int win = Mathf.Clamp(ducksToWin, 1, count);
            var match = new DuckFishingViewModel(pointsPerDuck, startingAttempts, count, win);
            var controller = go.AddComponent<DuckFishingAttractionController>();
            controller.Configure(booth.Ducks, booth.Rod, booth.Hud, booth.Pond, match);
            controller.Receive(Session);
            AttachPlaySupport(booth.XrOrigin, controller.Input, DuckFishingDesktopPlay.Profile(booth.Rod));
        }
    }
}
