using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Fairground.View.Attractions.PunchingBall
{
    public sealed class PunchingBallInput : IDisposable
    {
        readonly InputAction _restart;
        readonly InputAction _returnToFairground;
        readonly InputAction _punch;
        readonly InputAction _look;

        public PunchingBallInput()
        {
            _restart = Button("RestartPunchingBall");
            _restart.AddBinding("<Keyboard>/r");

            _returnToFairground = Button("ReturnToFairground");
            _returnToFairground.AddBinding("<Keyboard>/b");
            _returnToFairground.AddBinding("<XRController>{RightHand}/menuButton");

            _punch = Button("DesktopPunch");
            _punch.AddBinding("<Keyboard>/space");
            _punch.AddBinding("<Mouse>/leftButton");

            _look = new InputAction("DesktopLook", InputActionType.Value, "<Mouse>/delta");
        }

        public void Enable()
        {
            _restart.Enable();
            _returnToFairground.Enable();
            _punch.Enable();
            _look.Enable();
        }

        public void Disable()
        {
            _restart.Disable();
            _returnToFairground.Disable();
            _punch.Disable();
            _look.Disable();
        }

        public void Dispose()
        {
            _restart.Dispose();
            _returnToFairground.Dispose();
            _punch.Dispose();
            _look.Dispose();
        }

        public bool RestartPressed => _restart.WasPressedThisFrame();

        public bool ReturnToFairgroundPressed => _returnToFairground.WasPressedThisFrame();

        public bool PunchPressed => _punch.WasPressedThisFrame();

        public bool PunchHeld => _punch.IsPressed();

        public Vector2 LookDelta => _look.ReadValue<Vector2>();

        static InputAction Button(string name) => new InputAction(name, InputActionType.Button);
    }
}
