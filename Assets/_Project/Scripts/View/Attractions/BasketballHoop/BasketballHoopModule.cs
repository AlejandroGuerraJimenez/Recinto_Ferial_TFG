using Fairground.Core.Attractions;
using Fairground.View.Attractions.BasketballHoop.Builders;
using Fairground.ViewModel.Attractions.BasketballHoop;
using UnityEngine;

namespace Fairground.View.Attractions.BasketballHoop
{
    /// <summary>
    /// Hoop booth. Counts come from <see cref="BasketballHoopBalance"/>.
    /// </summary>
    public sealed class BasketballHoopModule : IAttractionModule
    {
        public AttractionId Id => AttractionId.BasketballHoop;

        public void Assemble(AttractionBuildContext context)
        {
            BasketballHoopBoothBuilder booth = BoothModuleShell.Build(
                context,
                prefab => new BasketballHoopBoothBuilder(prefab));
            var go = new GameObject("BasketballHoopGame");
            var viewModel = new BasketballHoopViewModel();
            var host = go.AddComponent<BasketballHoopAttractionHost>();
            host.Configure(booth.Hoop, booth.Spawner, booth.Hud, viewModel);
            host.Receive(context.Visit);
            BoothModuleShell.AttachPlay(
                booth.XrOrigin,
                host.Input,
                BasketballHoopDesktopPlay.Profile(booth.Spawner));
            booth.EnsureBallReady();
        }
    }
}
