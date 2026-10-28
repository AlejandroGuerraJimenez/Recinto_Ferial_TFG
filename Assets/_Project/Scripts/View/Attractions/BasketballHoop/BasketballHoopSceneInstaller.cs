using Fairground.View.Attractions.BasketballHoop.Builders;
using UnityEngine;

namespace Fairground.View.Attractions.BasketballHoop
{
    /// <summary>
    /// Wires the hoop and shot rules onto the shared booth shell.
    /// </summary>
    public sealed class BasketballHoopSceneInstaller : BoothSceneInstaller
    {
        [SerializeField] int pointsPerBasket = 2;
        [SerializeField] int startingBalls = 10;
        [SerializeField] int winScoreThreshold = 5;

        protected override BoothBuilder CreateBuilder()
        {
            return new BasketballHoopBoothBuilder(XrOriginPrefab);
        }

        protected override void WireGameplay(BoothBuilder builder)
        {
            var booth = (BasketballHoopBoothBuilder)builder;
            var go = new GameObject("BasketballHoopGame");
            var controller = go.AddComponent<BasketballHoopAttractionController>();
            controller.Configure(
                booth.Hoop,
                booth.Spawner,
                booth.Hud,
                pointsPerBasket,
                startingBalls,
                winScoreThreshold);
            AttachPlaySupport(
                booth.XrOrigin,
                controller.Input,
                BasketballHoopDesktopPlay.Profile(booth.Spawner));
            booth.EnsureBallReady();
        }
    }
}
