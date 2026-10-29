using Fairground.ViewModel;
using UnityEngine;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// What a module needs to build one attraction scene. Balance is not here.
    /// </summary>
    public sealed class AttractionBuildContext
    {
        public AttractionBuildContext(
            GameObject xrOriginPrefab,
            Material primitiveTemplate,
            GameObject boothEnvironmentPrefab,
            Transform bakedEnvironment,
            FairgroundVisitState visit)
        {
            XrOriginPrefab = xrOriginPrefab;
            PrimitiveTemplate = primitiveTemplate;
            BoothEnvironmentPrefab = boothEnvironmentPrefab;
            BakedEnvironment = bakedEnvironment;
            Visit = visit;
        }

        public GameObject XrOriginPrefab { get; }

        public Material PrimitiveTemplate { get; }

        public GameObject BoothEnvironmentPrefab { get; }

        public Transform BakedEnvironment { get; }

        public FairgroundVisitState Visit { get; }
    }
}
