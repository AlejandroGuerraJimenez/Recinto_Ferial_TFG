#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.InputSystem;

namespace Fairground.View
{
    public static class EditorSelectionInput
    {
        public static void Read(EditorInputContext context)
        {
            ReadRestart(context);
            if (context.IsAwaiting())
                ReadConfirm(context);
        }

        static void ReadRestart(EditorInputContext context)
        {
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
                context.Restart();
        }

        static void ReadConfirm(EditorInputContext context)
        {
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
                context.Confirm();

            ReadClick(context);
        }

        static void ReadClick(EditorInputContext context)
        {
            if (!LeftClick() || Camera.main == null)
                return;

            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (HitsCandidate(ray, context, out var visual))
                Confirm(context, visual);
        }

        static void Confirm(EditorInputContext context, PlaneSurfaceVisual visual)
        {
            context.Aim.Hover(visual);
            context.Confirm();
        }

        static bool LeftClick()
        {
            return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        }

        static bool HitsCandidate(Ray ray, EditorInputContext context, out PlaneSurfaceVisual visual)
        {
            visual = null;
            if (!Physics.Raycast(ray, out var hit, context.Distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                return false;

            visual = hit.collider.GetComponentInParent<PlaneSurfaceVisual>();
            return visual != null && visual.IsCandidate;
        }
    }
}
#endif
