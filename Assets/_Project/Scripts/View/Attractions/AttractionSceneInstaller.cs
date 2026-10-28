using Fairground.View.Attractions.Rendering;
using Fairground.ViewModel;
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

        protected FairgroundSession Session { get; private set; }

        void Awake()
        {
            if (FairgroundVisit.TryReceive(out var session))
                Receive(session);

            if (buildOnAwake)
                Build();
        }

        /// <summary>
        /// Takes the visit the fairground delivered before this scene loaded.
        /// </summary>
        public void Receive(FairgroundSession session)
        {
            Session = session;
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
