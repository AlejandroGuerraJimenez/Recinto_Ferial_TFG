using System;
using UnityEngine;
#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif

namespace Fairground.View
{
    /// <summary>
    /// Asks for com.oculus.permission.USE_SCENE before plane detection starts.
    /// Quest 3 planes come from the Space Setup scene model, and the Meta OpenXR provider
    /// refuses them until this permission is granted.
    /// </summary>
    public static class ScenePermissionRequester
    {
        public const string UseScenePermission = "com.oculus.permission.USE_SCENE";

        public static void Request(Action onGranted, Action onDenied)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            Ask(onGranted, onDenied);
#else
            onGranted?.Invoke();
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        static void Ask(Action onGranted, Action onDenied)
        {
            if (Permission.HasUserAuthorizedPermission(UseScenePermission))
                Grant(onGranted);
            else
                Prompt(onGranted, onDenied);
        }

        static void Grant(Action onGranted) => onGranted?.Invoke();

        static void Prompt(Action onGranted, Action onDenied)
        {
            var callbacks = new PermissionCallbacks();
            callbacks.PermissionGranted += _ => onGranted?.Invoke();
            callbacks.PermissionDenied += _ => onDenied?.Invoke();
            Permission.RequestUserPermission(UseScenePermission, callbacks);
        }
#endif
    }
}
