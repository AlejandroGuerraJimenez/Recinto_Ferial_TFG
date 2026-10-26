using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fairground.View
{
    /// <summary>
    /// World-space line in front of the headset so plane detection has a visible state.
    /// TextMeshPro faces the user. Legacy Text renders mirrored on Quest.
    /// </summary>
    public sealed class PlaneDetectionStatusView : MonoBehaviour
    {
        const float Distance = 1.15f;
        const float CanvasScale = 0.0012f;

        TextMeshProUGUI _text;
        Canvas _canvas;
        Transform _camera;

        public static PlaneDetectionStatusView Create()
        {
            var root = new GameObject("PlaneDetectionStatus");
            var view = root.AddComponent<PlaneDetectionStatusView>();
            view.BuildLabel();
            return view;
        }

        public void Present(string message)
        {
            FollowCamera();
            if (_text != null)
                _text.text = message ?? string.Empty;
        }

        void BuildLabel()
        {
            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.WorldSpace;
            gameObject.AddComponent<CanvasScaler>();

            var rect = gameObject.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(1400f, 520f);
            transform.localScale = Vector3.one * CanvasScale;

            var label = new GameObject("Label");
            label.transform.SetParent(transform, false);
            _text = label.AddComponent<TextMeshProUGUI>();
            _text.font = Font();
            _text.fontSize = 36;
            _text.alignment = TextAlignmentOptions.Center;
            _text.color = Color.white;
            _text.textWrappingMode = TextWrappingModes.Normal;
            _text.overflowMode = TextOverflowModes.Overflow;
            _text.raycastTarget = false;

            var labelRect = label.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
        }

        void FollowCamera()
        {
            if (_camera == null)
                _camera = ResolveCamera();
            if (_camera == null)
                return;

            var forward = _camera.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.0001f)
                forward = _camera.forward;
            forward.Normalize();

            if (_canvas != null && _canvas.worldCamera == null)
                _canvas.worldCamera = _camera.GetComponent<Camera>();

            transform.position = _camera.position + forward * Distance + Vector3.down * 0.28f;
            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }

        static TMP_FontAsset Font()
        {
            if (TMP_Settings.defaultFontAsset != null)
                return TMP_Settings.defaultFontAsset;

            return Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        }

        static Transform ResolveCamera()
        {
            var camera = Camera.main;
            return camera != null ? camera.transform : null;
        }
    }
}
