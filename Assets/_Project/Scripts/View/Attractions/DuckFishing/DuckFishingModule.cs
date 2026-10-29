using Fairground.Core.Attractions;
using Fairground.View.Attractions.DuckFishing.Builders;
using Fairground.ViewModel.Attractions.DuckFishing;
using UnityEngine;

namespace Fairground.View.Attractions.DuckFishing
{
    /// <summary>
    /// Duck booth. Counts come from <see cref="DuckFishingBalance"/>.
    /// </summary>
    public sealed class DuckFishingModule : IAttractionModule
    {
        public AttractionId Id => AttractionId.DuckFishing;

        public void Assemble(AttractionBuildContext context)
        {
            DuckFishingBoothBuilder booth = BoothModuleShell.Build(
                context,
                prefab => new DuckFishingBoothBuilder(prefab));
            var go = new GameObject("DuckFishingGame");
            var viewModel = new DuckFishingViewModel();
            var host = go.AddComponent<DuckFishingAttractionHost>();
            host.Configure(booth.Ducks, booth.Rod, booth.Hud, booth.Pond, viewModel);
            host.Receive(context.Visit);
            BoothModuleShell.AttachPlay(
                booth.XrOrigin,
                host.Input,
                DuckFishingDesktopPlay.Profile(booth.Rod));
        }
    }
}
