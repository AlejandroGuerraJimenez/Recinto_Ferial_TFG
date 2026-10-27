using Fairground.View.Attractions.DuckFishing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fairground.View.Attractions.DuckFishing.Factories
{
    /// <summary>
    /// Factory: world-space HUD canvas and labels.
    /// </summary>
    public sealed class DuckFishingHudFactory
    {
        public DuckFishingHudView Create()
        {
            var canvasGo = CreateCanvas();
            var hud = canvasGo.AddComponent<DuckFishingHudView>();
            AssignDefaultTexts(hud, canvasGo.transform);
            return hud;
        }

        static void AssignDefaultTexts(DuckFishingHudView hud, Transform parent)
        {
            TextMeshProUGUI score = CreateLabel(parent, "ScoreText", new Vector2(0f, 120f), "Score: 0");
            TextMeshProUGUI attempts = CreateLabel(parent, "AttemptsText", new Vector2(0f, 40f), "Hooks: 8");
            TextMeshProUGUI ducks = CreateLabel(parent, "DucksText", new Vector2(0f, -40f), "Caught: 0/4");
            TextMeshProUGUI status = CreateLabel(parent, "StatusText", new Vector2(0f, -120f), "Catch 4 ducks!");
            hud.AssignTexts(score, attempts, ducks, status);
        }

        static GameObject CreateCanvas()
        {
            var canvasGo = new GameObject("HUD");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();

            var rect = canvasGo.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(800f, 400f);
            canvasGo.transform.position = new Vector3(0f, 2.2f, 3.6f);
            canvasGo.transform.rotation = Quaternion.identity;
            canvasGo.transform.localScale = Vector3.one * 0.0025f;
            return canvasGo;
        }

        static TextMeshProUGUI CreateLabel(Transform parent, string name, Vector2 pos, string value)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var text = go.AddComponent<TextMeshProUGUI>();
            text.font = ResolveFont();
            text.fontSize = 48;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            text.raycastTarget = false;
            text.text = value;

            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(700f, 70f);
            rect.anchoredPosition = pos;
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
