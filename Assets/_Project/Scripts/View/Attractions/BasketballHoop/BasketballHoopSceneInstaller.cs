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

        protected override void Assemble()
        {
            var builder = new BasketballHoopBoothBuilder(XrOriginPrefab);
            Transform baked = ResolveBakedEnvironment();
            if (baked != null)
                builder.UseBakedEnvironment(baked);
            else
                builder.BuildEnvironment(BoothEnvironmentPrefab);

            builder.BuildXrRig().BuildGameplay();
            AttractionPlacement.TryApplyFromSession(builder.BoothSpaceRoots());

            var go = new GameObject("BasketballHoopGame");
            var controller = go.AddComponent<BasketballHoopAttractionController>();
            controller.Configure(
                builder.Hoop,
                builder.Spawner,
                builder.Hud,
                pointsPerBasket,
                startingBalls,
                winScoreThreshold);
            AttachPlaySupport(
                builder.XrOrigin,
                controller.Input,
                BasketballHoopDesktopPlay.Profile(builder.Spawner));
            builder.EnsureBallReady();
        }
    }
}
