using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace Fairground.View.Attractions.BalloonThrow.Factories
{
    /// <summary>
    /// Factory: opaque VR XR Origin + interaction stack (no AR passthrough).
    /// </summary>
    public sealed class XrAttractionRigFactory
    {
        readonly GameObject _xrOriginPrefab;

        public XrAttractionRigFactory(GameObject xrOriginPrefab)
        {
            _xrOriginPrefab = xrOriginPrefab;
        }

        public GameObject EnsureXrOrigin()
        {
            var existing = GameObject.Find("XR Origin");
            if (existing != null)
                return existing;

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
                Debug.LogError("[BalloonThrow] XR Origin prefab is not assigned.");
                return null;
            }

            var instance = Object.Instantiate(_xrOriginPrefab);
            instance.name = "XR Origin";
            instance.transform.position = Vector3.zero;
            ConfigureOpaqueCamera(instance.GetComponentInChildren<Camera>());
            return instance;
        }

        static void ConfigureOpaqueCamera(Camera camera)
        {
            if (camera == null)
                return;

            camera.clearFlags = CameraClearFlags.Skybox;
            var background = camera.backgroundColor;
            background.a = 1f;
            camera.backgroundColor = background;
            StripArBehaviours(camera);
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
