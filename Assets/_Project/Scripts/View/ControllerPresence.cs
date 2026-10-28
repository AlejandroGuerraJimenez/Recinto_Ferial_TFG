using System.Collections.Generic;
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
        const string LeftVisualName = LeftName + " Visual";
        const string RightVisualName = RightName + " Visual";
        const string RuntimeName = "Meta Runtime Controller";
        const string LeftModelPath = "/model_fb/controller/left";
        const string RightModelPath = "/model_fb/controller/right";
        const string ErrorShaderName = "Hidden/InternalErrorShader";
        const float ModelPathPollSeconds = 0.5f;
        const float OriginSearchSeconds = 0.5f;

        static readonly Color FallbackColor = new Color(0.92f, 0.93f, 0.95f, 1f);

        static bool _installed;
        static Shader _modelShader;
        static Shader _errorShader;
        static bool _errorShaderResolved;
        static Material _fallbackMaterial;
        static string[] _renderModelPaths;
        static float _nextModelPathPoll = -1f;
        static float _nextOriginSearch = -1f;

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
            _nextOriginSearch = -1f;
            TryFindControllers();
        }

        void LateUpdate()
        {
            PumpInput();
            TryFindControllers();
            Show(_left, LeftVisualName, LeftModelPath, OVRInput.Controller.LTouch);
            Show(_right, RightVisualName, RightModelPath, OVRInput.Controller.RTouch);
        }

        void TryFindControllers()
        {
            if (_left.Controller != null && _right.Controller != null)
                return;

            if (_nextOriginSearch >= 0f && Time.unscaledTime < _nextOriginSearch)
                return;

            _nextOriginSearch = Time.unscaledTime + OriginSearchSeconds;
            if (_left.Controller == null)
                _left.Controller = FindController(LeftName);
            if (_right.Controller == null)
                _right.Controller = FindController(RightName);
        }

        static void PumpInput()
        {
            if (!OVRPlugin.initialized)
                return;

            OVRInput.Update();
        }

        static void Show(Hand hand, string visualName, string modelPath, OVRInput.Controller kind)
        {
            if (hand.Controller == null)
                return;

            if (hand.Runtime == null && RuntimeHasModel(modelPath))
                hand.Runtime = EnsureRuntime(hand.Controller, kind);

            KeepModelShown(hand.Runtime);

            if (ModelIsVisible(hand))
            {
                SetStarterVisual(hand, visualName, false);
                return;
            }

            SetStarterVisual(hand, visualName, true);
            if (!hand.StarterTinted)
                hand.StarterTinted = TintStarter(hand, visualName);
        }

        static bool RuntimeHasModel(string modelPath)
        {
            return ContainsPath(RenderModelPaths(), modelPath);
        }

        static string[] RenderModelPaths()
        {
            if (ContainsPath(_renderModelPaths, LeftModelPath) && ContainsPath(_renderModelPaths, RightModelPath))
                return _renderModelPaths;

            if (_nextModelPathPoll >= 0f && Time.unscaledTime < _nextModelPathPoll)
                return _renderModelPaths;

            if (!OVRPlugin.initialized)
                return _renderModelPaths;

            _nextModelPathPoll = Time.unscaledTime + ModelPathPollSeconds;
            _renderModelPaths = OVRPlugin.GetRenderModelPaths();
            return _renderModelPaths;
        }

        static bool ContainsPath(string[] paths, string modelPath)
        {
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

        static bool ModelIsVisible(Hand hand)
        {
            if (hand.Runtime == null)
                return false;

            if (!hand.ModelVisible)
                hand.Runtime.GetComponentsInChildren(false, hand.Renderers);

            hand.ModelVisible = AnyDrawn(hand.Renderers);
            return hand.ModelVisible;
        }

        static bool AnyDrawn(List<MeshRenderer> renderers)
        {
            for (var i = 0; i < renderers.Count; i++)
            {
                if (IsDrawn(renderers[i]))
                    return true;
            }

            return false;
        }

        static bool IsDrawn(MeshRenderer renderer)
        {
            if (renderer == null || !renderer.enabled)
                return false;

            var material = renderer.sharedMaterial;
            var shader = material != null ? material.shader : null;
            if (shader == null)
                return false;

            var error = ErrorShader();
            if (error != null)
                return shader != error;

            return shader.name != ErrorShaderName;
        }

        static Shader ErrorShader()
        {
            if (_errorShaderResolved)
                return _errorShader;

            _errorShaderResolved = true;
            _errorShader = Shader.Find(ErrorShaderName);
            return _errorShader;
        }

        static void SetStarterVisual(Hand hand, string visualName, bool visible)
        {
            var visual = StarterVisual(hand, visualName);
            if (visual == null)
                return;

            if (visual.gameObject.activeSelf != visible)
                visual.gameObject.SetActive(visible);
        }

        static Transform StarterVisual(Hand hand, string visualName)
        {
            if (hand.Visual != null)
                return hand.Visual;

            if (hand.Controller == null)
                return null;

            var found = hand.Controller.Find(visualName);
            if (found != null)
                hand.Visual = found;
            return found;
        }

        static bool TintStarter(Hand hand, string visualName)
        {
            var visual = StarterVisual(hand, visualName);
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
            public Transform Visual;
            public bool ModelVisible;
            public readonly List<MeshRenderer> Renderers = new List<MeshRenderer>();

            public void Clear()
            {
                Controller = null;
                Runtime = null;
                StarterTinted = false;
                Visual = null;
                ModelVisible = false;
                Renderers.Clear();
            }
        }
    }
}
