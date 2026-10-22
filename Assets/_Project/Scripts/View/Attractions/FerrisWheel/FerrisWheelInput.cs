using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Fairground.View.Attractions.FerrisWheel
{
    public sealed class FerrisWheelInput : IDisposable
    {
        readonly InputAction _returnToFairground;
        readonly InputAction _look;

        public FerrisWheelInput()
        {
            _returnToFairground = Button("ReturnToFairground");
            _returnToFairground.AddBinding("<Keyboard>/b");
            _returnToFairground.AddBinding("<XRController>{RightHand}/menuButton");

            _look = new InputAction("DesktopLook", InputActionType.Value, "<Mouse>/delta");
        }

        public void Enable()
        {
            _returnToFairground.Enable();
            _look.Enable();
        }

        public void Disable()
        {
            _returnToFairground.Disable();
            _look.Disable();
        }

        public void Dispose()
        {
            _returnToFairground.Dispose();
            _look.Dispose();
        }

        public bool ReturnToFairgroundPressed => _returnToFairground.WasPressedThisFrame();

        public Vector2 LookDelta => _look.ReadValue<Vector2>();

        static InputAction Button(string name) => new InputAction(name, InputActionType.Button);
    }
}
