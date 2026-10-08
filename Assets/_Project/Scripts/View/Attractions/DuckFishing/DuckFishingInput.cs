using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Fairground.View.Attractions.DuckFishing
{
    public sealed class DuckFishingInput : IDisposable
    {
        readonly InputAction _restart;
        readonly InputAction _returnToFairground;
        readonly InputAction _dip;
        readonly InputAction _look;

        public DuckFishingInput()
        {
            _restart = Button("RestartDuckFishing");
            _restart.AddBinding("<Keyboard>/r");

            _returnToFairground = Button("ReturnToFairground");
            _returnToFairground.AddBinding("<Keyboard>/b");
            _returnToFairground.AddBinding("<XRController>{RightHand}/menuButton");

            _dip = Button("DesktopDip");
            _dip.AddBinding("<Keyboard>/space");
            _dip.AddBinding("<Mouse>/leftButton");
            _dip.AddBinding("<XRController>{RightHand}/triggerPressed");
            _dip.AddBinding("<XRController>{LeftHand}/triggerPressed");

            _look = new InputAction("DesktopLook", InputActionType.Value, "<Mouse>/delta");
        }

        public void Enable()
        {
            _restart.Enable();
            _returnToFairground.Enable();
            _dip.Enable();
            _look.Enable();
        }

        public void Disable()
        {
            _restart.Disable();
            _returnToFairground.Disable();
            _dip.Disable();
            _look.Disable();
        }

        public void Dispose()
        {
            _restart.Dispose();
            _returnToFairground.Dispose();
            _dip.Dispose();
            _look.Dispose();
        }

        public bool RestartPressed => _restart.WasPressedThisFrame();

        public bool ReturnToFairgroundPressed => _returnToFairground.WasPressedThisFrame();

        public bool DipPressed => _dip.WasPressedThisFrame();

        public Vector2 LookDelta => _look.ReadValue<Vector2>();

        static InputAction Button(string name) => new InputAction(name, InputActionType.Button);
    }
}
