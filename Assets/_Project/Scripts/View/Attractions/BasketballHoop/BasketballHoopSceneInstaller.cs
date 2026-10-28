using Fairground.Model.Attractions.BasketballHoop;
using Fairground.View.Attractions.BasketballHoop.Builders;
using UnityEngine;

namespace Fairground.View.Attractions.BasketballHoop
{
    /// <summary>
    /// Wires the hoop and shot rules onto the shared booth shell.
    /// </summary>
    public sealed class BasketballHoopSceneInstaller : BoothSceneInstaller
    {
        [SerializeField] int pointsPerBasket = BasketballHoopRules.DefaultPointsPerBasket;
        [SerializeField] int startingBalls = BasketballHoopRules.DefaultStartingBalls;
        [SerializeField] int winScoreThreshold = BasketballHoopRules.DefaultWinScoreThreshold;

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
            controller.Receive(Session);
            AttachPlaySupport(
                booth.XrOrigin,
                controller.Input,
                BasketballHoopDesktopPlay.Profile(booth.Spawner));
            booth.EnsureBallReady();
        }
    }
}
