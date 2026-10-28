using System;
using UnityEngine;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Shared booth shell: environment props and the opaque XR rig. Gameplay objects stay in each attraction.
    /// </summary>
    public sealed class BoothAssembly
    {
        readonly BoothPropFactory _props;
        readonly XrAttractionRigFactory _rig;

        public BoothAssembly(BoothLayout layout, GameObject xrOriginPrefab, string logTag, Color sky)
        {
            _props = new BoothPropFactory(layout);
            _rig = new XrAttractionRigFactory(xrOriginPrefab, logTag, sky);
        }

        public GameObject Environment { get; private set; }

        public GameObject XrOrigin { get; private set; }

        public void UseBakedEnvironment(Transform bakedRoot)
        {
            _props.EnsureLighting();
            Environment = bakedRoot != null ? bakedRoot.gameObject : null;
        }

        public void BuildEnvironment(GameObject boothEnvironmentPrefab)
        {
            _props.EnsureLighting();
            Environment = boothEnvironmentPrefab != null
                ? UnityEngine.Object.Instantiate(boothEnvironmentPrefab)
                : _props.CreateEnvironment();

            if (Environment != null)
                Environment.name = BoothPropFactory.EnvironmentRootName;
        }

        public void BuildXrRig(Action<GameObject> afterRig = null)
        {
            XrOrigin = _rig.EnsureXrOrigin();
            _rig.EnsureInteractionManager();
            _rig.EnsureEventSystem();
            afterRig?.Invoke(XrOrigin);
        }
    }
}
