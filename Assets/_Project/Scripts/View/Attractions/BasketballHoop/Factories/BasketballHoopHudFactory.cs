using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fairground.View.Attractions.BasketballHoop.Factories
{
    /// <summary>
    /// Factory: world-space HUD canvas and labels.
    /// </summary>
    public sealed class BasketballHoopHudFactory
    {
        public BasketballHoopHudView Create()
        {
            var canvasGo = CreateCanvas();
            var hud = canvasGo.AddComponent<BasketballHoopHudView>();
            AssignDefaultTexts(hud, canvasGo.transform);
            return hud;
        }

        static void AssignDefaultTexts(BasketballHoopHudView hud, Transform parent)
        {
            TextMeshProUGUI score = CreateLabel(parent, "ScoreText", new Vector2(0f, 80f), "Score: 0");
            TextMeshProUGUI balls = CreateLabel(parent, "BallsText", new Vector2(0f, 0f), "Balls: 10");
            TextMeshProUGUI status = CreateLabel(parent, "StatusText", new Vector2(0f, -80f), "Shoot the hoop!");
            hud.AssignTexts(score, balls, status);
        }

        static GameObject CreateCanvas()
        {
            var canvasGo = new GameObject("HUD");
            ConfigureCanvasComponents(canvasGo);
            PlaceCanvas(canvasGo);
            return canvasGo;
        }

        static void ConfigureCanvasComponents(GameObject canvasGo)
        {
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();
        }

        static void PlaceCanvas(GameObject canvasGo)
        {
            var rect = canvasGo.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(800f, 320f);
            canvasGo.transform.position = BasketballHoopLayout.HudPosition;
            canvasGo.transform.rotation = Quaternion.identity;
            canvasGo.transform.localScale = Vector3.one * 0.0025f;
        }

        static TextMeshProUGUI CreateLabel(Transform parent, string name, Vector2 pos, string value)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var text = go.AddComponent<TextMeshProUGUI>();
            StyleLabel(text, value);
            PlaceLabel(go.GetComponent<RectTransform>(), pos);
            return text;
        }

        static void StyleLabel(TextMeshProUGUI text, string value)
        {
            text.font = ResolveFont();
            text.fontSize = 48;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            text.text = value;
            text.raycastTarget = false;
        }

        static void PlaceLabel(RectTransform rect, Vector2 pos)
        {
            rect.sizeDelta = new Vector2(700f, 70f);
            rect.anchoredPosition = pos;
        }

        static TMP_FontAsset ResolveFont()
        {
            if (TMP_Settings.defaultFontAsset != null)
                return TMP_Settings.defaultFontAsset;

            return Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        }
    }
}
