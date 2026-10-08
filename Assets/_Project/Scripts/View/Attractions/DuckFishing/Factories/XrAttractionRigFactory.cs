using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace Fairground.View.Attractions.DuckFishing.Factories
{
    /// <summary>
    /// Factory: opaque VR XR Origin + interaction stack (no AR passthrough).
    /// </summary>
    public sealed class XrAttractionRigFactory
    {
        static readonly Color OpaqueSky = new Color(0.4f, 0.65f, 0.85f, 1f);

        readonly GameObject _xrOriginPrefab;

        public XrAttractionRigFactory(GameObject xrOriginPrefab)
        {
            _xrOriginPrefab = xrOriginPrefab;
        }

        public GameObject EnsureXrOrigin()
        {
            var existing = GameObject.Find("XR Origin");
            if (existing != null)
            {
                ConfigureOpaqueCamera(existing.GetComponentInChildren<Camera>());
                return existing;
            }

            return SpawnXrOrigin();
        }

        public void EnsureInteractionManager()
        {
            if (Object.FindFirstObjectByType<XRInteractionManager>() != null)
                return;

            new GameObject("XR Interaction Manager").AddComponent<XRInteractionManager>();
        }

        public void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null)
                return;

            var go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
            go.AddComponent<XRUIInputModule>();
        }

        GameObject SpawnXrOrigin()
        {
            if (_xrOriginPrefab == null)
            {
                Debug.LogError("[DuckFishing] XR Origin prefab is not assigned.");
                return null;
            }

            var instance = Object.Instantiate(_xrOriginPrefab);
            instance.name = "XR Origin";
            instance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            ConfigureOpaqueCamera(instance.GetComponentInChildren<Camera>());
            return instance;
        }

        static void ConfigureOpaqueCamera(Camera camera)
        {
            if (camera == null)
                return;

            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = OpaqueSky;
            camera.enabled = true;
            StripArBehaviours(camera);

            if (camera.GetUniversalAdditionalCameraData() is { } urp)
                urp.renderType = CameraRenderType.Base;
        }

        static void StripArBehaviours(Camera camera)
        {
            foreach (var behaviour in camera.GetComponents<Behaviour>())
            {
                if (IsArBehaviour(behaviour))
                    Object.Destroy(behaviour);
            }
        }

        static bool IsArBehaviour(Behaviour behaviour)
        {
            if (behaviour == null)
                return false;

            string typeName = behaviour.GetType().Name;
            return typeName.Contains("ARCamera") || typeName.Contains("AROcclusion");
        }
    }
}
