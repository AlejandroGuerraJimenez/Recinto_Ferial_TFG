using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Fairground.View.Attractions.BasketballHoop
{
    public sealed class BasketballHoopInput : IDisposable
    {
        readonly InputAction _restart;
        readonly InputAction _returnToFairground;
        readonly InputAction _throw;
        readonly InputAction _look;

        public BasketballHoopInput()
        {
            _restart = Button("RestartBasketballHoop");
            _restart.AddBinding("<Keyboard>/r");

            _returnToFairground = Button("ReturnToFairground");
            _returnToFairground.AddBinding("<Keyboard>/b");
            _returnToFairground.AddBinding("<XRController>{RightHand}/menuButton");

            _throw = Button("DesktopThrow");
            _throw.AddBinding("<Keyboard>/space");
            _throw.AddBinding("<Mouse>/leftButton");

            _look = new InputAction("DesktopLook", InputActionType.Value, "<Mouse>/delta");
        }

        public void Enable()
        {
            _restart.Enable();
            _returnToFairground.Enable();
            _throw.Enable();
            _look.Enable();
        }

        public void Disable()
        {
            _restart.Disable();
            _returnToFairground.Disable();
            _throw.Disable();
            _look.Disable();
        }

        public void Dispose()
        {
            _restart.Dispose();
            _returnToFairground.Dispose();
            _throw.Dispose();
            _look.Dispose();
        }

        public bool RestartPressed => _restart.WasPressedThisFrame();

        public bool ReturnToFairgroundPressed => _returnToFairground.WasPressedThisFrame();

        public bool ThrowPressed => _throw.WasPressedThisFrame();

        public Vector2 LookDelta => _look.ReadValue<Vector2>();

        static InputAction Button(string name) => new InputAction(name, InputActionType.Button);
    }
}
