using UnityEngine;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Booth scenes add a baked environment and the shared play support on top of <see cref="AttractionSceneInstaller"/>.
    /// </summary>
    public abstract class BoothSceneInstaller : AttractionSceneInstaller
    {
        [SerializeField] GameObject boothEnvironmentPrefab;
        [SerializeField] Transform bakedBoothEnvironment;

        protected GameObject BoothEnvironmentPrefab => boothEnvironmentPrefab;

        protected Transform ResolveBakedEnvironment()
        {
            return AttractionSceneSupport.ResolveBakedEnvironment(bakedBoothEnvironment);
        }

        protected void AttachPlaySupport(GameObject xrOrigin, AttractionInput input, DesktopPlayProfile profile)
        {
            AttractionSceneSupport.CreateDesktopPlay(xrOrigin, input, profile);
            AttractionSceneSupport.CreateVrCamera(xrOrigin);
        }
    }
}
