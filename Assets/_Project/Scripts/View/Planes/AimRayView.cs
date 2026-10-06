using UnityEngine;
using UnityEngine.Rendering;

namespace Fairground.View
{
    public sealed class AimRayView
    {
        static readonly Color Hit = new Color(0.5f, 1f, 0.7f, 0.9f);
        static readonly Color Idle = new Color(1f, 1f, 1f, 0.45f);

        readonly LineRenderer _line;
        Material _material;

        AimRayView(LineRenderer line) => _line = line;

        public static AimRayView Create(Transform parent, Material border)
        {
            var view = new AimRayView(CreateRenderer(parent));
            view.Use(PlaneMaterialFactory.Create(border, Color.white));
            return view;
        }

        public void Draw(bool show, Vector3 origin, Vector3 end, bool hovering)
        {
            if (_line == null)
                return;

            _line.enabled = show;
            if (show)
                Paint(origin, end, hovering);
        }

        public void Release() => PlaneMaterialFactory.Release(_material);

        void Use(Material material)
        {
            _material = material;
            if (_material != null)
                _line.material = _material;
        }

        void Paint(Vector3 origin, Vector3 end, bool hovering)
        {
            _line.SetPosition(0, origin);
            _line.SetPosition(1, end);
            var color = hovering ? Hit : Idle;
            _line.startColor = color;
            _line.endColor = color;
        }

        static LineRenderer CreateRenderer(Transform parent)
        {
            var line = NewLine(parent);
            SetShape(line);
            SetLook(line);
            return line;
        }

        static LineRenderer NewLine(Transform parent)
        {
            var rayObject = new GameObject("Aim Ray");
            rayObject.transform.SetParent(parent, false);
            return rayObject.AddComponent<LineRenderer>();
        }

        static void SetShape(LineRenderer line)
        {
            line.positionCount = 2;
            line.useWorldSpace = true;
            line.loop = false;
        }

        static void SetLook(LineRenderer line)
        {
            line.widthMultiplier = 0.004f;
            line.numCapVertices = 4;
            line.shadowCastingMode = ShadowCastingMode.Off;
            line.receiveShadows = false;
        }
    }
}
