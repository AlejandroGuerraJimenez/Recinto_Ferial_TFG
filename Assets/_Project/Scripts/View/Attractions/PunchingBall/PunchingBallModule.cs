using Fairground.Core.Attractions;
using Fairground.View.Attractions.PunchingBall.Builders;
using Fairground.ViewModel.Attractions.PunchingBall;
using UnityEngine;

namespace Fairground.View.Attractions.PunchingBall
{
    /// <summary>
    /// Punch booth. Counts come from <see cref="PunchingBallBalance"/>.
    /// </summary>
    public sealed class PunchingBallModule : IAttractionModule
    {
        public AttractionId Id => AttractionId.PunchingBall;

        public void Assemble(AttractionBuildContext context)
        {
            PunchingBallBoothBuilder booth = BoothModuleShell.Build(
                context,
                prefab => new PunchingBallBoothBuilder(prefab));
            var go = new GameObject("PunchingBallGame");
            var viewModel = new PunchingBallViewModel();
            var host = go.AddComponent<PunchingBallAttractionHost>();
            host.Configure(booth.Pad, booth.Meter, booth.Hud, viewModel);
            host.Receive(context.Visit);
            BoothModuleShell.AttachPlay(
                booth.XrOrigin,
                host.Input,
                PunchingBallDesktopPlay.Profile(booth.Pad));
        }
    }
}
