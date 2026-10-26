using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Fairground.View
{
    /// <summary>
    /// Closes the Quest build. The system Close button only pauses a focus-aware app,
    /// so Y on the left controller finishes the Android activity.
    /// </summary>
    public sealed class HeadsetQuit : MonoBehaviour
    {
        static bool _installed;

        InputAction _quit;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Install()
        {
            if (_installed)
                return;

            _installed = true;
            var root = new GameObject(nameof(HeadsetQuit));
            DontDestroyOnLoad(root);
            root.AddComponent<HeadsetQuit>();
        }

        void Awake()
        {
            _quit = new InputAction("QuitApplication", InputActionType.Button);
            _quit.AddBinding("<XRController>{LeftHand}/secondaryButton");
            _quit.Enable();
        }

        void Update()
        {
            if (_quit != null && _quit.WasPressedThisFrame())
                Quit();
        }

        void OnDestroy()
        {
            _quit?.Dispose();
        }

        public static void Quit()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    if (activity != null)
                        activity.Call("finishAndRemoveTask");
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Could not finish the Android activity: " + exception.Message);
            }

            Application.Quit();
#endif
        }
    }
}
