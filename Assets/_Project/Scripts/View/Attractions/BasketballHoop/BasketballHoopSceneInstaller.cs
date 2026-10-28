using Fairground.Core.Attractions;
using Fairground.View.Attractions.BasketballHoop.Builders;
using Fairground.ViewModel.Attractions.BasketballHoop;
using UnityEngine;

namespace Fairground.View.Attractions.BasketballHoop
{
    /// <summary>
    /// Wires the hoop and shot rules onto the shared booth shell.
    /// </summary>
    public sealed class BasketballHoopSceneInstaller : BoothSceneInstaller
    {
        [SerializeField] int pointsPerBasket = BasketballHoopBalance.PointsPerBasket;
        [SerializeField] int startingBalls = BasketballHoopBalance.StartingBalls;
        [SerializeField] int winScoreThreshold = BasketballHoopBalance.WinScoreThreshold;

        protected override BoothBuilder CreateBuilder()
        {
            return new BasketballHoopBoothBuilder(XrOriginPrefab);
        }

        protected override void WireGameplay(BoothBuilder builder)
        {
            var booth = (BasketballHoopBoothBuilder)builder;
            var go = new GameObject("BasketballHoopGame");
            var match = new BasketballHoopViewModel(pointsPerBasket, startingBalls, winScoreThreshold);
            var controller = go.AddComponent<BasketballHoopAttractionController>();
            controller.Configure(booth.Hoop, booth.Spawner, booth.Hud, match);
            controller.Receive(Session);
            AttachPlaySupport(
                booth.XrOrigin,
                controller.Input,
                BasketballHoopDesktopPlay.Profile(booth.Spawner));
            booth.EnsureBallReady();
        }
    }
}
