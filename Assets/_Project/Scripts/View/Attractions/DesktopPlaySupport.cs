using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEngine.XR.Management;
#endif

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Editor play without a headset: opaque camera, mouse look, and the attraction's own play action.
    /// </summary>
    public sealed class DesktopPlaySupport : MonoBehaviour
    {
        const float LookSensitivity = 0.12f;

        Transform _cameraTransform;
        Camera _targetCamera;
        AttractionInput _input;
        DesktopPlayProfile _profile = new DesktopPlayProfile();
        float _yaw;
        float _pitch;
        bool _desktopMode;

        public void Configure(Transform camera, AttractionInput input, DesktopPlayProfile profile)
        {
            _cameraTransform = camera;
            _targetCamera = camera != null ? camera.GetComponent<Camera>() : null;
            _input = input;
            if (profile != null)
                _profile = profile;
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

            if (AttractionDesktopPlay.IsLiveHeadset() || _cameraTransform == null || _input == null)
            {
                enabled = false;
                yield break;
            }

            StopXrForEditorDesktop();
            yield return null;

            _desktopMode = true;
            PrepareDesktopCamera();
            PlaceCamera();
            _profile.OnReady?.Invoke(_cameraTransform);
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
#endif

        void OnDisable()
        {
            if (_desktopMode)
                LockCursor(false);
        }

        void Update()
        {
            if (!_desktopMode || _cameraTransform == null || _input == null)
                return;

            ApplyLook();
            if (_profile.AllowMove == null || _profile.AllowMove(_cameraTransform))
                AttractionDesktopPlay.MoveWithKeyboard(_cameraTransform, AttractionDesktopPlay.MoveSpeed);

            _profile.OnFrame?.Invoke(_cameraTransform, _input);

            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                LockCursor(false);
        }

        void PrepareDesktopCamera()
        {
            if (_targetCamera == null)
                _targetCamera = _cameraTransform.GetComponent<Camera>();

            if (_targetCamera == null)
                return;

            _targetCamera.targetDisplay = 0;
            _targetCamera.enabled = true;
            _targetCamera.clearFlags = CameraClearFlags.SolidColor;
            var color = _targetCamera.backgroundColor;
            color.a = 1f;
            if (color.r + color.g + color.b < 0.05f)
                color = _profile.FallbackSky;
            _targetCamera.backgroundColor = color;
        }

        void PlaceCamera()
        {
            if (_profile.UseCustomPlacement)
            {
                _profile.Place?.Invoke(_cameraTransform);
                return;
            }

            _cameraTransform.SetPositionAndRotation(_profile.EyePosition, _profile.EyeRotation);
        }

        void CaptureInitialLook()
        {
            Vector3 euler = _cameraTransform.rotation.eulerAngles;
            _yaw = euler.y;
            _pitch = euler.x > 180f ? euler.x - 360f : euler.x;
        }

        void ApplyLook()
        {
            Vector2 delta = _input.LookDelta;
            _yaw += delta.x * LookSensitivity;
            _pitch = Mathf.Clamp(_pitch - delta.y * LookSensitivity, -80f, 80f);
            _cameraTransform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }

        static void LockCursor(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}
