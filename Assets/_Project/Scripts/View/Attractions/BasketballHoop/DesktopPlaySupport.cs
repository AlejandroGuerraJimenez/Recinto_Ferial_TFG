using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.XR.Management;
#endif

namespace Fairground.View.Attractions.BasketballHoop
{
    /// <summary>
    /// Desktop Strategy for editor play without an active XR headset display.
    /// </summary>
    public sealed class DesktopPlaySupport : MonoBehaviour
    {
        [SerializeField] Transform cameraTransform;
        [SerializeField] Camera targetCamera;
        [SerializeField] BallSpawnerView ballSpawner;
        [SerializeField] float lookSensitivity = 0.12f;
        [SerializeField] float throwSpeed = 9f;
        [SerializeField] float arcBoost = 2.4f;

        float _yaw;
        float _pitch;
        bool _desktopMode;
        BasketballHoopInput _input;

        public void Configure(Transform camera, BallSpawnerView spawner, BasketballHoopInput input)
        {
            cameraTransform = camera;
            targetCamera = camera != null ? camera.GetComponent<Camera>() : null;
            ballSpawner = spawner;
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

            // With URP, stereoTargetEye is invalid. Stop XR so Game View renders mono.
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
            HandleThrowInput();
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
                color = new Color(0.35f, 0.55f, 0.75f, 1f);
            targetCamera.backgroundColor = color;
        }

        void PlaceCameraForBooth()
        {
            cameraTransform.SetPositionAndRotation(
                new Vector3(0f, 1.5f, 0f),
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

        void HandleThrowInput()
        {
            if (!_input.ThrowPressed)
                return;

            PlaceBallAtCamera();
            Vector3 velocity = cameraTransform.forward * throwSpeed + Vector3.up * arcBoost;
            ballSpawner?.LaunchCurrentBall(velocity);
        }

        void PlaceBallAtCamera()
        {
            BasketballBallView ball = ballSpawner != null ? ballSpawner.CurrentBall : null;
            if (ball == null)
                return;

            Vector3 origin = cameraTransform.position + cameraTransform.forward * 0.45f;
            var body = ball.GetComponent<Rigidbody>();
            if (body != null)
            {
                body.position = origin;
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }

            ball.transform.position = origin;
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
