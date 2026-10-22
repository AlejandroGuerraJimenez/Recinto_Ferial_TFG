using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.XR.Management;
#endif

namespace Fairground.View.Attractions.FerrisWheel
{
    /// <summary>
    /// Editor play without a headset: park the camera in front of the wheel and allow mouse look.
    /// </summary>
    public sealed class DesktopPlaySupport : MonoBehaviour
    {
        [SerializeField] Transform cameraTransform;
        [SerializeField] Camera targetCamera;
        [SerializeField] float lookSensitivity = 0.12f;

        float _yaw;
        float _pitch;
        bool _desktopMode;
        FerrisWheelInput _input;

        public void Configure(Transform camera, FerrisWheelInput input)
        {
            cameraTransform = camera;
            targetCamera = camera != null ? camera.GetComponent<Camera>() : null;
            _input = input;
        }

        void Awake()
        {
#if !UNITY_EDITOR
            enabled = false;
#endif
        }

        void Start()
        {
#if UNITY_EDITOR
            StartCoroutine(InitializeWhenReady());
#else
            enabled = false;
#endif
        }

#if UNITY_EDITOR
        IEnumerator InitializeWhenReady()
        {
            yield return null;
            yield return null;

            if (IsHeadsetDisplayActive() || cameraTransform == null || _input == null)
            {
                enabled = false;
                yield break;
            }

            StopXrForEditorDesktop();
            yield return null;

            _desktopMode = true;
            PrepareDesktopCamera();
            PlaceCamera();
            CaptureInitialLook();
            LockCursor(true);
        }

        static void StopXrForEditorDesktop()
        {
            var manager = XRGeneralSettings.Instance != null
                ? XRGeneralSettings.Instance.Manager
                : null;
            if (manager == null || !manager.isInitializationComplete)
                return;

            manager.StopSubsystems();
            manager.DeinitializeLoader();
        }

        static bool IsHeadsetDisplayActive()
        {
            return UnityEngine.XR.XRSettings.isDeviceActive;
        }
#endif

        void OnDisable()
        {
            if (_desktopMode)
                LockCursor(false);
        }

        void Update()
        {
            if (!_desktopMode || cameraTransform == null || _input == null)
                return;

            ApplyLook();
            if (KeyboardEscapePressed())
                LockCursor(false);
        }

        void PrepareDesktopCamera()
        {
            if (targetCamera == null)
                targetCamera = cameraTransform.GetComponent<Camera>();

            if (targetCamera == null)
                return;

            targetCamera.targetDisplay = 0;
            targetCamera.enabled = true;
            targetCamera.clearFlags = CameraClearFlags.SolidColor;
            var color = targetCamera.backgroundColor;
            color.a = 1f;
            if (color.r + color.g + color.b < 0.05f)
                color = new Color(0.45f, 0.68f, 0.9f, 1f);
            targetCamera.backgroundColor = color;
        }

        void PlaceCamera()
        {
            Vector3 eye = FerrisWheelLayout.DesktopEye;
            Vector3 hub = FerrisWheelLayout.WheelCenter;
            cameraTransform.SetPositionAndRotation(eye, Quaternion.LookRotation(hub - eye, Vector3.up));
        }

        void CaptureInitialLook()
        {
            Vector3 euler = cameraTransform.rotation.eulerAngles;
            _yaw = euler.y;
            _pitch = euler.x > 180f ? euler.x - 360f : euler.x;
        }

        void ApplyLook()
        {
            Vector2 delta = _input.LookDelta;
            _yaw += delta.x * lookSensitivity;
            _pitch = Mathf.Clamp(_pitch - delta.y * lookSensitivity, -80f, 80f);
            cameraTransform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }

        static bool KeyboardEscapePressed()
        {
            return UnityEngine.InputSystem.Keyboard.current != null
                   && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame;
        }

        static void LockCursor(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}
