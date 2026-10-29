using Fairground.Core.Attractions;
using Fairground.View.Attractions.FerrisWheel.Factories;
using Fairground.ViewModel.Attractions.FerrisWheel;
using UnityEngine;

namespace Fairground.View.Attractions.FerrisWheel
{
    /// <summary>
    /// Ferris wheel on the site layout. Speed comes from <see cref="FerrisWheelBalance"/>.
    /// </summary>
    public sealed class FerrisWheelModule : IAttractionModule
    {
        public AttractionId Id => AttractionId.FerrisWheel;

        public void Assemble(AttractionBuildContext context)
        {
            var rigFactory = new XrAttractionRigFactory(
                context.XrOriginPrefab,
                "[FerrisWheel]",
                FerrisWheelDesktopPlay.Sky);
            rigFactory.EnsureInteractionManager();
            rigFactory.EnsureEventSystem();
            GameObject xrOrigin = rigFactory.EnsureXrOrigin();

            var ride = new FerrisWheelSiteFactory();
            FerrisWheelRigView rig = ride.Create(xrOrigin);

            var go = new GameObject("FerrisWheelGame");
            var viewModel = new FerrisWheelViewModel();
            var host = go.AddComponent<FerrisWheelAttractionHost>();
            host.Configure(rig, FerrisWheelSiteLayout.CabinCount, viewModel);
            host.Receive(context.Visit);

            AttractionSceneSupport.CreateVrCamera(xrOrigin);
            AttractionSceneSupport.CreateDesktopPlay(xrOrigin, host.Input, FerrisWheelDesktopPlay.Profile());
        }
    }
}
