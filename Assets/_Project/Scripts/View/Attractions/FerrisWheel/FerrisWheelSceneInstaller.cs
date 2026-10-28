using Fairground.View.Attractions.FerrisWheel.Factories;
using UnityEngine;

namespace Fairground.View.Attractions.FerrisWheel
{
    /// <summary>
    /// Builds the London Eye ride on the shared opaque rig. The globe and the wheel stay in this attraction.
    /// </summary>
    public sealed class FerrisWheelSceneInstaller : AttractionSceneInstaller
    {
        [SerializeField] float degreesPerSecond = LondonEyeLayout.DegreesPerSecond;
        [SerializeField] int cabinCount = LondonEyeLayout.CabinCount;

        protected override void Assemble()
        {
            var rigFactory = new XrAttractionRigFactory(XrOriginPrefab, "[FerrisWheel]", FerrisWheelDesktopPlay.Sky);
            rigFactory.EnsureInteractionManager();
            rigFactory.EnsureEventSystem();
            GameObject xrOrigin = rigFactory.EnsureXrOrigin();

            var ride = new LondonEyeRideFactory();
            FerrisWheelRigView rig = ride.Create(xrOrigin);

            var go = new GameObject("FerrisWheelGame");
            var controller = go.AddComponent<FerrisWheelAttractionController>();
            controller.Configure(rig, degreesPerSecond, cabinCount);
            controller.Receive(Session);

            AttractionSceneSupport.CreateVrCamera(xrOrigin);
            AttractionSceneSupport.CreateDesktopPlay(xrOrigin, controller.Input, FerrisWheelDesktopPlay.Profile());
        }
    }
}
