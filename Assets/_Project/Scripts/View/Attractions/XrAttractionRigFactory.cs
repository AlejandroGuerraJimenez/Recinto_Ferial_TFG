using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Opaque VR XR Origin and the interaction stack. Each attraction supplies the sky and the log tag.
    /// </summary>
    public sealed class XrAttractionRigFactory
    {
        readonly GameObject _xrOriginPrefab;
        readonly string _logTag;
        readonly Color _sky;

        public XrAttractionRigFactory(GameObject xrOriginPrefab, string logTag, Color sky)
        {
            _xrOriginPrefab = xrOriginPrefab;
            _logTag = string.IsNullOrEmpty(logTag) ? "[Attraction]" : logTag;
            _sky = sky;
        }

        public GameObject EnsureXrOrigin()
        {
            var existing = GameObject.Find("XR Origin");
            if (existing != null)
            {
                ConfigureOpaqueCamera(existing.GetComponentInChildren<Camera>());
                AttractionXrRigSetup.Apply(existing);
                return existing;
            }

            return SpawnXrOrigin();
        }

        public void EnsureInteractionManager()
        {
            if (Object.FindAnyObjectByType<XRInteractionManager>() != null)
                return;

            new GameObject("XR Interaction Manager").AddComponent<XRInteractionManager>();
        }

        public void EnsureEventSystem()
        {
            if (Object.FindAnyObjectByType<EventSystem>() != null)
                return;

            var go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
            go.AddComponent<XRUIInputModule>();
        }

        GameObject SpawnXrOrigin()
        {
            if (_xrOriginPrefab == null)
            {
                Debug.LogError($"{_logTag} XR Origin prefab is not assigned.");
                return null;
            }

            var instance = Object.Instantiate(_xrOriginPrefab);
            instance.name = "XR Origin";
            instance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            ConfigureOpaqueCamera(instance.GetComponentInChildren<Camera>());
            AttractionXrRigSetup.Apply(instance);
            return instance;
        }

        void ConfigureOpaqueCamera(Camera camera)
        {
            if (camera == null)
                return;

            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = _sky;
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
