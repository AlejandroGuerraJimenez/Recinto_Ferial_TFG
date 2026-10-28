using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// World-space HUD canvas. Line count and world position come from the attraction.
    /// </summary>
    public sealed class BoothHudFactory
    {
        const float LineStep = 80f;
        const float LabelWidth = 700f;
        const float LabelHeight = 70f;
        const float CanvasWidth = 800f;
        const float WorldScale = 0.0025f;
        const float FontSize = 48f;

        public BoothHudView Create(Vector3 worldPosition, string[] initialLines)
        {
            string[] lines = initialLines ?? System.Array.Empty<string>();
            var canvasGo = CreateCanvas(worldPosition, lines.Length);
            var labels = new TextMeshProUGUI[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                float y = ((lines.Length - 1) * 0.5f - i) * LineStep;
                labels[i] = CreateLabel(canvasGo.transform, "Line" + i, new Vector2(0f, y), lines[i]);
            }

            var hud = canvasGo.AddComponent<BoothHudView>();
            hud.AssignLines(labels);
            return hud;
        }

        static GameObject CreateCanvas(Vector3 worldPosition, int lineCount)
        {
            var canvasGo = new GameObject("HUD");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();

            var rect = canvasGo.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(CanvasWidth, LineStep * (lineCount + 1));
            canvasGo.transform.position = worldPosition;
            canvasGo.transform.rotation = Quaternion.identity;
            canvasGo.transform.localScale = Vector3.one * WorldScale;
            return canvasGo;
        }

        static TextMeshProUGUI CreateLabel(Transform parent, string name, Vector2 position, string value)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var text = go.AddComponent<TextMeshProUGUI>();
            text.font = ResolveFont();
            text.fontSize = FontSize;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            text.text = value;
            text.raycastTarget = false;

            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(LabelWidth, LabelHeight);
            rect.anchoredPosition = position;
            return text;
        }

        static TMP_FontAsset ResolveFont()
        {
            if (TMP_Settings.defaultFontAsset != null)
                return TMP_Settings.defaultFontAsset;

            return Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        }
    }
}
