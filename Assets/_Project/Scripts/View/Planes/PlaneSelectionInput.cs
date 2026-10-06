using System;
using UnityEngine.InputSystem;

namespace Fairground.View
{
    public sealed class PlaneSelectionInput
    {
        readonly InputAction _confirm;
        readonly InputAction _restart;
        readonly Action _onConfirm;
        readonly Action _onRestart;
        readonly Func<EditorInputContext> _editorContext;

        public PlaneSelectionInput(Action onConfirm, Action onRestart, Func<EditorInputContext> editorContext)
        {
            _onConfirm = onConfirm;
            _onRestart = onRestart;
            _editorContext = editorContext;
            _confirm = ConfirmAction();
            _restart = RestartAction();
        }

        public void Enable()
        {
            _confirm.Enable();
            _restart.Enable();
        }

        public void Disable()
        {
            _confirm.Disable();
            _restart.Disable();
        }

        public void Dispose()
        {
            _confirm.Dispose();
            _restart.Dispose();
        }

        public void Read()
        {
            ReadEditor();
            if (_confirm.WasPressedThisFrame())
                _onConfirm();
            if (_restart.WasPressedThisFrame())
                _onRestart();
        }

        void ReadEditor()
        {
#if UNITY_EDITOR
            EditorSelectionInput.Read(_editorContext());
#endif
        }

        static InputAction ConfirmAction()
        {
            var action = Button("ConfirmPlane");
            action.AddBinding("<XRController>{RightHand}/triggerPressed");
            action.AddBinding("<XRController>{RightHand}/trigger").WithInteraction("Press");
            return action;
        }

        static InputAction RestartAction()
        {
            var action = Button("RestartPlaneSelection");
            action.AddBinding("<XRController>{RightHand}/secondaryButton");
            return action;
        }

        static InputAction Button(string name) => new InputAction(name, InputActionType.Button);
    }
}
