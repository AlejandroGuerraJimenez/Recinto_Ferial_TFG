using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Awake and material template for every attraction scene. The subclass builds what is unique.
    /// </summary>
    public abstract class AttractionSceneInstaller : MonoBehaviour
    {
        [SerializeField] GameObject xrOriginPrefab;
        [SerializeField] Material primitiveLitTemplate;
        [SerializeField] bool buildOnAwake = true;

        bool _built;

        protected GameObject XrOriginPrefab => xrOriginPrefab;

        void Awake()
        {
            if (buildOnAwake)
                Build();
        }

        public void Build()
        {
            if (_built)
                return;

            _built = true;
            PrimitiveMaterialApplier.SetTemplate(AttractionSceneSupport.ResolvePrimitiveTemplate(primitiveLitTemplate));
            Assemble();
        }

        protected abstract void Assemble();
    }
}
