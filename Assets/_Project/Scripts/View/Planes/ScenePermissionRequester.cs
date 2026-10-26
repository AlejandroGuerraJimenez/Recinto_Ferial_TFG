using System;
using System.Collections;
using UnityEngine;
#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif

namespace Fairground.View
{
    /// <summary>
    /// Asks for com.oculus.permission.USE_SCENE before plane detection starts.
    /// Quest planes come from the Space Setup scene model, and the Meta provider
    /// returns none until this permission is granted.
    /// </summary>
    public static class ScenePermissionRequester
    {
        public const string UseScenePermission = "com.oculus.permission.USE_SCENE";

        public static void Request(Action onGranted, Action onDenied)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var host = new GameObject("ScenePermissionRequest");
            host.AddComponent<Runner>().Begin(onGranted, onDenied);
#else
            onGranted?.Invoke();
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        sealed class Runner : MonoBehaviour
        {
            Action _onGranted;
            Action _onDenied;
            bool _settled;

            public void Begin(Action onGranted, Action onDenied)
            {
                _onGranted = onGranted;
                _onDenied = onDenied;
            }

            IEnumerator Start()
            {
                // The system panel only composites once the Quest app has focus.
                // Asking on the first frame, or while Unity's own startup dialog is pending, drops it.
                while (!Application.isFocused || !ActivityReady())
                    yield return null;

                yield return null;
                if (AlreadyGranted())
                {
                    Finish(true);
                    yield break;
                }

                Ask();
            }

            void Ask()
            {
                var callbacks = new PermissionCallbacks();
                callbacks.PermissionGranted += _ => Finish(true);
                callbacks.PermissionDenied += _ => Finish(false);
                Debug.Log("[Planos] Pidiendo com.oculus.permission.USE_SCENE.");
                Permission.RequestUserPermission(UseScenePermission, callbacks);
            }

            static bool ActivityReady()
            {
                using var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
                return activity != null;
            }

            static bool AlreadyGranted()
            {
                return Permission.HasUserAuthorizedPermission(UseScenePermission);
            }

            void Finish(bool granted)
            {
                if (_settled)
                    return;

                _settled = true;
                if (granted)
                    _onGranted?.Invoke();
                else
                    _onDenied?.Invoke();

                Destroy(gameObject);
            }
        }
#endif
    }
}
