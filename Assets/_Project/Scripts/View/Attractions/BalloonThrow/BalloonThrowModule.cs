using Fairground.Core.Attractions;
using Fairground.View.Attractions.BalloonThrow.Builders;
using Fairground.ViewModel.Attractions.BalloonThrow;
using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow
{
    /// <summary>
    /// Balloon booth. Counts come from <see cref="BalloonThrowBalance"/>.
    /// </summary>
    public sealed class BalloonThrowModule : IAttractionModule
    {
        public AttractionId Id => AttractionId.BalloonThrow;

        public void Assemble(AttractionBuildContext context)
        {
            BalloonThrowBoothBuilder booth = BoothModuleShell.Build(
                context,
                prefab => new BalloonThrowBoothBuilder(prefab));
            var go = new GameObject("BalloonThrowGame");
            var viewModel = new BalloonThrowViewModel();
            var host = go.AddComponent<BalloonThrowAttractionHost>();
            host.Configure(booth.Balloons, booth.Spawner, booth.Hud, viewModel);
            host.Receive(context.Visit);
            BoothModuleShell.AttachPlay(
                booth.XrOrigin,
                host.Input,
                BalloonThrowDesktopPlay.Profile(booth.Spawner));
            booth.EnsureBallReady();
        }
    }
}
