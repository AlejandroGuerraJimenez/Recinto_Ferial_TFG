using Fairground.ViewModel.Attractions.DuckFishing;
using UnityEngine;

namespace Fairground.View.Attractions.DuckFishing
{
    /// <summary>
    /// Pond booth props, sky, and HUD lines. The shared shell builds everything else.
    /// </summary>
    public static class DuckFishingBoothLayout
    {
        public const string LogTag = "[DuckFishing]";
        public const string PondName = "Pond";

        public static readonly Color Sky = new Color(0.4f, 0.65f, 0.85f, 1f);
        public static readonly Vector3 HudPosition = new Vector3(0f, 2.2f, 3.6f);
        public static readonly string[] HudLines =
        {
            "Score: 0",
            "Hooks: 8",
            "Caught: 0/4",
            "Catch 4 ducks!",
        };

        public static BoothLayout Props()
        {
            return new BoothLayout
            {
                LogTag = LogTag,
                Sky = Sky,
                HudPosition = HudPosition,
                HudLines = HudLines,
                Lighting = new BoothLighting
                {
                    Sky = new Color(0.55f, 0.65f, 0.75f, 1f),
                    Equator = new Color(0.35f, 0.4f, 0.4f, 1f),
                    Ground = new Color(0.12f, 0.14f, 0.12f, 1f),
                },
                Pieces = new[]
                {
                    BoothPiece.Plane("Ground", new Vector3(2f, 1f, 2f), new Color(0.3f, 0.42f, 0.28f, 1f)),
                    BoothPiece.Cube(
                        "BoothBack",
                        new Vector3(0f, 1.4f, 4.4f),
                        new Vector3(4.5f, 2.8f, 0.15f),
                        new Color(0.2f, 0.4f, 0.55f, 1f),
                        removeCollider: true),
                    BoothPiece.Group(PondName, new Vector3(0f, 0f, 2.6f)),
                    BoothPiece.Cylinder(
                        "Basin",
                        PondName,
                        new Vector3(0f, 0.35f, 0f),
                        new Vector3(3.2f, 0.35f, 3.2f),
                        new Color(0.35f, 0.28f, 0.18f, 1f)),
                    BoothPiece.Cylinder(
                        "Water",
                        PondName,
                        new Vector3(0f, 0.62f, 0f),
                        new Vector3(2.9f, 0.08f, 2.9f),
                        new Color(0.2f, 0.55f, 0.75f, 1f)),
                },
            };
        }

        public static string[] FormatHud(DuckFishingViewModel viewModel)
        {
            return viewModel.HudLines;
        }
    }
}
