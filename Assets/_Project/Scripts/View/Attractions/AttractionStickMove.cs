using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Thumbstick walk and turn on the rig root. The starter locomotion stack is off
    /// so room-scale head tracking is not pinned to the character controller.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AttractionStickMove : MonoBehaviour
    {
        const float MoveSpeed = 2.2f;
        const float TurnSpeed = 80f;
        const float StickThresholdSqr = 0.04f;

        Transform _head;
        InputAction _move;
        InputAction _turn;

        void Awake()
        {
            Camera camera = GetComponentInChildren<Camera>();
            _head = camera != null ? camera.transform : null;
        }

        void OnEnable()
        {
            _move = new InputAction("AttractionMove", InputActionType.Value);
            _move.AddBinding("<XRController>{LeftHand}/thumbstick");
            _move.Enable();

            _turn = new InputAction("AttractionTurn", InputActionType.Value);
            _turn.AddBinding("<XRController>{RightHand}/thumbstick");
            _turn.Enable();
        }

        void Start()
        {
            // The fairground rig disables this asset on unload. Enable it again after that.
            GetComponent<InputActionManager>()?.EnableInput();
        }

        void OnDisable()
        {
            _move?.Disable();
            _turn?.Disable();
        }

        void OnDestroy()
        {
            _move?.Dispose();
            _turn?.Dispose();
            _move = null;
            _turn = null;
        }

        void Update()
        {
#if UNITY_EDITOR
            if (!AttractionDesktopPlay.IsLiveHeadset())
                return;
#endif
            if (_head == null || _move == null || _turn == null)
                return;

            ApplyMove(_move.ReadValue<Vector2>());
            ApplyTurn(_turn.ReadValue<Vector2>().x);
        }

        void ApplyMove(Vector2 stick)
        {
            if (stick.sqrMagnitude < StickThresholdSqr)
                return;

            Vector3 planar = new Vector3(stick.x, 0f, stick.y);
            if (planar.sqrMagnitude > 1f)
                planar.Normalize();

            Vector3 world = Quaternion.Euler(0f, _head.eulerAngles.y, 0f) * planar;
            transform.position += world * MoveSpeed * Time.deltaTime;
        }

        void ApplyTurn(float x)
        {
            if (x * x < StickThresholdSqr)
                return;

            transform.RotateAround(_head.position, Vector3.up, x * TurnSpeed * Time.deltaTime);
        }
    }
}
