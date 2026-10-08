using Fairground.View.Attractions.DuckFishing;
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
            Text score = CreateLabel(parent, "ScoreText", new Vector2(0f, 120f), "Score: 0");
            Text attempts = CreateLabel(parent, "AttemptsText", new Vector2(0f, 40f), "Hooks: 8");
            Text ducks = CreateLabel(parent, "DucksText", new Vector2(0f, -40f), "Caught: 0/4");
            Text status = CreateLabel(parent, "StatusText", new Vector2(0f, -120f), "Catch 4 ducks!");
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
            canvasGo.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            canvasGo.transform.localScale = Vector3.one * 0.0025f;
            return canvasGo;
        }

        static Text CreateLabel(Transform parent, string name, Vector2 pos, string value)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var text = go.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                        ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 48;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.text = value;

            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(700f, 70f);
            rect.anchoredPosition = pos;
            return text;
        }
    }
}
