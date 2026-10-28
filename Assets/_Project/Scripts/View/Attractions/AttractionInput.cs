using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Shared attraction input: restart, return to the fairground, one play button, and mouse look.
    /// Booths enable play. Duck fishing also binds the headset triggers. The wheel omits restart and play.
    /// </summary>
    public sealed class AttractionInput : IDisposable
    {
        readonly InputAction _restart;
        readonly InputAction _returnToFairground;
        readonly InputAction _play;
        readonly InputAction _look;

        public AttractionInput(bool restart = true, bool play = true, bool headsetTriggers = false)
        {
            if (restart)
            {
                _restart = Button("RestartAttraction");
                _restart.AddBinding("<Keyboard>/r");
            }

            _returnToFairground = Button("ReturnToFairground");
            _returnToFairground.AddBinding("<Keyboard>/b");
            _returnToFairground.AddBinding("<XRController>{RightHand}/secondaryButton");

            if (play)
            {
                _play = Button("DesktopPlay");
                _play.AddBinding("<Keyboard>/space");
                _play.AddBinding("<Mouse>/leftButton");
                if (headsetTriggers)
                {
                    _play.AddBinding("<XRController>{RightHand}/triggerPressed");
                    _play.AddBinding("<XRController>{LeftHand}/triggerPressed");
                }
            }

            _look = new InputAction("DesktopLook", InputActionType.Value, "<Mouse>/delta");
        }

        public bool RestartPressed => _restart != null && _restart.WasPressedThisFrame();

        public bool ReturnToFairgroundPressed => _returnToFairground.WasPressedThisFrame();

        public bool PlayPressed => _play != null && _play.WasPressedThisFrame();

        public bool PlayHeld => _play != null && _play.IsPressed();

        public Vector2 LookDelta => _look.ReadValue<Vector2>();

        public void Enable()
        {
            _restart?.Enable();
            _returnToFairground.Enable();
            _play?.Enable();
            _look.Enable();
        }

        public void Disable()
        {
            _restart?.Disable();
            _returnToFairground.Disable();
            _play?.Disable();
            _look.Disable();
        }

        public void Dispose()
        {
            _restart?.Dispose();
            _returnToFairground.Dispose();
            _play?.Dispose();
            _look.Dispose();
        }

        static InputAction Button(string name) => new InputAction(name, InputActionType.Button);
    }
}
