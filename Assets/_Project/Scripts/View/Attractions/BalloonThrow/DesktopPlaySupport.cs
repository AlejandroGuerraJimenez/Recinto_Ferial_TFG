using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow
{
    /// <summary>
    /// Desktop Strategy for editor play without XR headset.
    /// </summary>
    public sealed class DesktopPlaySupport : MonoBehaviour
    {
        [SerializeField] Transform cameraTransform;
        [SerializeField] BallSpawnerView ballSpawner;
        [SerializeField] float lookSensitivity = 2.2f;
        [SerializeField] float throwSpeed = 7.5f;
        [SerializeField] KeyCode throwKey = KeyCode.Space;

        float _yaw;
        float _pitch;
        bool _desktopMode;

        public void Configure(Transform camera, BallSpawnerView spawner)
        {
            cameraTransform = camera;
            ballSpawner = spawner;
        }

        void Start()
        {
            _desktopMode = !IsXrSessionActive();
            if (!_desktopMode || cameraTransform == null)
            {
                enabled = false;
                return;
            }

            CaptureInitialLook();
            LockCursor(true);
        }

        void OnDisable()
        {
            if (_desktopMode)
                LockCursor(false);
        }

        void Update()
        {
            if (!_desktopMode || cameraTransform == null)
                return;

            ApplyLook();
            HandleThrowInput();
            HandleCursorUnlock();
        }

        void CaptureInitialLook()
        {
            Vector3 euler = cameraTransform.rotation.eulerAngles;
            _yaw = euler.y;
            _pitch = euler.x > 180f ? euler.x - 360f : euler.x;
        }

        void ApplyLook()
        {
            _yaw += Input.GetAxis("Mouse X") * lookSensitivity;
            _pitch = Mathf.Clamp(_pitch - Input.GetAxis("Mouse Y") * lookSensitivity, -80f, 80f);
            cameraTransform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }

        void HandleThrowInput()
        {
            if (!Input.GetKeyDown(throwKey) && !Input.GetMouseButtonDown(0))
                return;

            PlaceBallAtCamera();
            ballSpawner?.LaunchCurrentBall(cameraTransform.forward * throwSpeed);
        }

        void PlaceBallAtCamera()
        {
            if (ballSpawner?.CurrentBall == null)
                return;

            Vector3 origin = cameraTransform.position + cameraTransform.forward * 0.4f;
            ballSpawner.CurrentBall.transform.position = origin;
        }

        void HandleCursorUnlock()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                LockCursor(false);
        }

        static void LockCursor(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }

        static bool IsXrSessionActive()
        {
            string display = UnityEngine.XR.XRSettings.loadedDeviceName;
            return !string.IsNullOrEmpty(display)
                   && !string.Equals(display, "None", System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
