using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fairground.View
{
    /// <summary>
    /// Shows the controller mesh the Quest runtime supplies for the connected Touch model.
    /// OVRRuntimeController only spawns that mesh after OVRInput.Update, and this rig has no
    /// OVRManager, so the input poll runs here. The generic toolkit mesh stays until the
    /// runtime mesh is actually drawing.
    /// </summary>
    public sealed class ControllerPresence : MonoBehaviour
    {
        const string LeftName = "Left Controller";
        const string RightName = "Right Controller";
        const string RuntimeName = "Meta Runtime Controller";
        const string LeftModelPath = "/model_fb/controller/left";
        const string RightModelPath = "/model_fb/controller/right";

        static readonly Color FallbackColor = new Color(0.92f, 0.93f, 0.95f, 1f);

        static bool _installed;
        static Shader _modelShader;
        static Material _fallbackMaterial;

        readonly Hand _left = new Hand();
        readonly Hand _right = new Hand();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Install()
        {
            if (_installed)
                return;

            _installed = true;
            var root = new GameObject(nameof(ControllerPresence));
            DontDestroyOnLoad(root);
            root.AddComponent<ControllerPresence>();
        }

        void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

        void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _left.Clear();
            _right.Clear();
        }

        void LateUpdate()
        {
            PumpInput();
            Show(_left, LeftName, LeftModelPath, OVRInput.Controller.LTouch);
            Show(_right, RightName, RightModelPath, OVRInput.Controller.RTouch);
        }

        static void PumpInput()
        {
            if (!OVRPlugin.initialized)
                return;

            OVRInput.Update();
        }

        static void Show(Hand hand, string controllerName, string modelPath, OVRInput.Controller kind)
        {
            if (hand.Controller == null)
                hand.Controller = FindController(controllerName);

            if (hand.Controller == null)
                return;

            if (hand.Runtime == null && RuntimeHasModel(modelPath))
                hand.Runtime = EnsureRuntime(hand.Controller, kind);

            KeepModelShown(hand.Runtime);

            if (ModelIsVisible(hand.Runtime))
            {
                SetStarterVisual(hand.Controller, false);
                return;
            }

            SetStarterVisual(hand.Controller, true);
            if (!hand.StarterTinted)
                hand.StarterTinted = TintStarter(hand.Controller);
        }

        static bool RuntimeHasModel(string modelPath)
        {
            if (!OVRPlugin.initialized)
                return false;

            var paths = OVRPlugin.GetRenderModelPaths();
            if (paths == null)
                return false;

            for (var i = 0; i < paths.Length; i++)
            {
                if (paths[i] == modelPath)
                    return true;
            }

            return false;
        }

        static OVRRuntimeController EnsureRuntime(Transform controller, OVRInput.Controller kind)
        {
            var existing = controller.Find(RuntimeName);
            if (existing != null)
                return existing.GetComponent<OVRRuntimeController>();

            var host = new GameObject(RuntimeName);
            host.transform.SetParent(controller, false);
            var runtime = host.AddComponent<OVRRuntimeController>();
            runtime.m_controller = kind;
            runtime.m_controllerModelShader = ModelShader();
            runtime.m_supportAnimation = true;
            return runtime;
        }

        static Shader ModelShader()
        {
            if (_modelShader != null)
                return _modelShader;

            var template = Resources.Load<Material>("MetaControllerLit");
            _modelShader = template != null ? template.shader : Shader.Find("Meta/Lit");
            return _modelShader;
        }

        static void KeepModelShown(OVRRuntimeController runtime)
        {
            if (runtime == null)
                return;

            var root = runtime.transform;
            for (var i = 0; i < root.childCount; i++)
            {
                var child = root.GetChild(i).gameObject;
                if (!child.activeSelf)
                    child.SetActive(true);
            }
        }

        static bool ModelIsVisible(OVRRuntimeController runtime)
        {
            if (runtime == null)
                return false;

            var renderers = runtime.GetComponentsInChildren<MeshRenderer>(false);
            for (var i = 0; i < renderers.Length; i++)
            {
                var material = renderers[i].sharedMaterial;
                var shader = material != null ? material.shader : null;
                if (renderers[i].enabled && shader != null && shader.name != "Hidden/InternalErrorShader")
                    return true;
            }

            return false;
        }

        static void SetStarterVisual(Transform controller, bool visible)
        {
            var visual = controller.Find(controller.name + " Visual");
            if (visual == null)
                return;

            if (visual.gameObject.activeSelf != visible)
                visual.gameObject.SetActive(visible);
        }

        static bool TintStarter(Transform controller)
        {
            var visual = controller.Find(controller.name + " Visual");
            var material = FallbackMaterial();
            if (visual == null || material == null)
                return false;

            var renderers = visual.GetComponentsInChildren<MeshRenderer>(true);
            for (var i = 0; i < renderers.Length; i++)
            {
                renderers[i].enabled = true;
                renderers[i].sharedMaterial = material;
            }

            return renderers.Length > 0;
        }

        static Material FallbackMaterial()
        {
            if (_fallbackMaterial != null)
                return _fallbackMaterial;

            var shader = Shader.Find("Universal Render Pipeline/Unlit")
                         ?? Shader.Find("Unlit/Color");
            if (shader == null)
                return null;

            _fallbackMaterial = new Material(shader);
            if (_fallbackMaterial.HasProperty("_BaseColor"))
                _fallbackMaterial.SetColor("_BaseColor", FallbackColor);
            _fallbackMaterial.color = FallbackColor;
            return _fallbackMaterial;
        }

        static Transform FindController(string name)
        {
            var origins = Object.FindObjectsByType<XROrigin>(FindObjectsInactive.Include);
            for (var i = 0; i < origins.Length; i++)
            {
                var found = FindExact(origins[i].transform, name);
                if (found != null)
                    return found;
            }

            return null;
        }

        static Transform FindExact(Transform root, string name)
        {
            var transforms = root.GetComponentsInChildren<Transform>(true);
            for (var i = 0; i < transforms.Length; i++)
            {
                if (transforms[i] != null && transforms[i].name == name)
                    return transforms[i];
            }

            return null;
        }

        sealed class Hand
        {
            public Transform Controller;
            public OVRRuntimeController Runtime;
            public bool StarterTinted;

            public void Clear()
            {
                Controller = null;
                Runtime = null;
                StarterTinted = false;
            }
        }
    }
}
