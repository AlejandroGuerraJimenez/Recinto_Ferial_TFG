using Fairground.Core.Attractions;
using UnityEngine;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// One installer for every attraction scene. The loaded scene name picks the module.
    /// </summary>
    public sealed class AttractionModuleHost : AttractionSceneInstaller
    {
        [SerializeField] GameObject boothEnvironmentPrefab;
        [SerializeField] Transform bakedBoothEnvironment;

        protected override void Assemble()
        {
            string sceneName = gameObject.scene.name;
            if (!AttractionScenes.TryGetAttractionId(sceneName, out AttractionId attractionId)
                || !AttractionModules.TryGet(attractionId, out IAttractionModule module))
            {
                Debug.LogError("[Fairground] No attraction module for scene " + sceneName + ".");
                return;
            }

            module.Assemble(new AttractionBuildContext(
                XrOriginPrefab,
                AttractionSceneSupport.ResolvePrimitiveTemplate(PrimitiveTemplate),
                boothEnvironmentPrefab,
                bakedBoothEnvironment,
                Visit));
        }
    }
}
