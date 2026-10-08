using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.XR.Management;
#endif

namespace Fairground.View.Attractions.DuckFishing
{
    /// <summary>
    /// Desktop Strategy for editor play without an active XR headset display.
    /// </summary>
    public sealed class DesktopPlaySupport : MonoBehaviour
    {
        [SerializeField] Transform cameraTransform;
        [SerializeField] Camera targetCamera;
        [SerializeField] FishingRodView rod;
        [SerializeField] float lookSensitivity = 0.12f;

        float _yaw;
        float _pitch;
        bool _desktopMode;
        DuckFishingInput _input;

        public void Configure(Transform camera, FishingRodView fishingRod, DuckFishingInput input)
        {
            cameraTransform = camera;
            targetCamera = camera != null ? camera.GetComponent<Camera>() : null;
            rod = fishingRod;
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

            if (IsHeadsetDisplayActive())
            {
                enabled = false;
                yield break;
            }

            if (cameraTransform == null || _input == null)
            {
                enabled = false;
                yield break;
            }

            StopXrForEditorDesktop();
            yield return null;

            _desktopMode = true;
            PrepareDesktopCamera();
            PlaceCameraForBooth();
            ReparentRodToCamera();
            CaptureInitialLook();
            LockCursor(true);
        }

        static void StopXrForEditorDesktop()
        {
            var manager = XRGeneralSettings.Instance != null
                ? XRGeneralSettings.Instance.Manager
                : null;
            if (manager == null)
                return;

            if (manager.isInitializationComplete)
            {
                manager.StopSubsystems();
                manager.DeinitializeLoader();
            }
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
            HandleCursorUnlock();
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
                color = new Color(0.4f, 0.65f, 0.85f, 1f);
            targetCamera.backgroundColor = color;
        }

        void PlaceCameraForBooth()
        {
            cameraTransform.SetPositionAndRotation(
                new Vector3(0f, 1.45f, 0.35f),
                Quaternion.identity);
        }

        void ReparentRodToCamera()
        {
            if (rod == null || cameraTransform == null)
                return;

            rod.transform.SetParent(cameraTransform, false);
            rod.transform.localPosition = new Vector3(0.2f, -0.15f, 0.35f);
            rod.transform.localRotation = Quaternion.identity;
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

        void HandleCursorUnlock()
        {
            if (UnityEngine.InputSystem.Keyboard.current != null
                && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
                LockCursor(false);
        }

        static void LockCursor(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }

        static bool IsHeadsetDisplayActive()
        {
            return UnityEngine.XR.XRSettings.isDeviceActive;
        }
    }
}
