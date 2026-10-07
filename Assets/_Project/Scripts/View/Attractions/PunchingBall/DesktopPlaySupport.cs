using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.XR.Management;
#endif

namespace Fairground.View.Attractions.PunchingBall
{
    /// <summary>
    /// Desktop Strategy for editor play without an active XR headset display.
    /// Hold punch to charge, release to hit the pad.
    /// </summary>
    public sealed class DesktopPlaySupport : MonoBehaviour
    {
        [SerializeField] Transform cameraTransform;
        [SerializeField] Camera targetCamera;
        [SerializeField] PunchPadView punchPad;
        [SerializeField] float lookSensitivity = 0.12f;
        [SerializeField] float chargePerSecond = 1.4f;
        [SerializeField] float maxImpactSpeed = 8f;

        float _yaw;
        float _pitch;
        float _charge;
        bool _charging;
        bool _desktopMode;
        PunchingBallInput _input;

        public void Configure(Transform camera, PunchPadView pad, PunchingBallInput input)
        {
            cameraTransform = camera;
            targetCamera = camera != null ? camera.GetComponent<Camera>() : null;
            punchPad = pad;
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
            HandlePunchCharge();
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
                color = new Color(0.28f, 0.32f, 0.45f, 1f);
            targetCamera.backgroundColor = color;
        }

        void PlaceCameraForBooth()
        {
            cameraTransform.SetPositionAndRotation(
                new Vector3(0f, 1.5f, 0.4f),
                Quaternion.identity);
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

        void HandlePunchCharge()
        {
            if (_input.PunchHeld)
            {
                _charging = true;
                _charge = Mathf.Clamp01(_charge + chargePerSecond * Time.deltaTime);
                return;
            }

            if (!_charging)
                return;

            float impact = Mathf.Lerp(1.5f, maxImpactSpeed, _charge);
            punchPad?.TryApplyImpact(impact);
            _charge = 0f;
            _charging = false;
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
