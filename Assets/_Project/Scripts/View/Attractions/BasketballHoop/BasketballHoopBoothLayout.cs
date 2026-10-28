using Fairground.Model.Attractions.BasketballHoop;
using UnityEngine;

namespace Fairground.View.Attractions.BasketballHoop
{
    /// <summary>
    /// Arcade cabinet props, sky, and HUD lines. The shared shell builds everything else.
    /// </summary>
    public static class BasketballHoopBoothLayout
    {
        public const string LogTag = "[BasketballHoop]";

        public static readonly Color Sky = new Color(0.35f, 0.55f, 0.75f, 1f);
        public static readonly string[] HudLines =
        {
            "Score: 0",
            "Balls: " + BasketballHoopRules.DefaultStartingBalls,
            "Shoot the hoop!",
        };

        public static BoothLayout Props()
        {
            return new BoothLayout
            {
                LogTag = LogTag,
                Sky = Sky,
                HudPosition = BasketballHoopLayout.HudPosition,
                HudLines = HudLines,
                Pieces = new[]
                {
                    BoothPiece.Plane(
                        "Ground",
                        new Vector3(2f, 1f, 2f),
                        new Color(0.22f, 0.24f, 0.22f, 1f),
                        MarkOutOfPlay),
                    BoothPiece.Cube(
                        "Playfield",
                        BasketballHoopLayout.PlayfieldPosition,
                        BasketballHoopLayout.PlayfieldScale,
                        new Color(0.16f, 0.2f, 0.32f, 1f),
                        configure: MarkOutOfPlay),
                    BoothPiece.Cube(
                        "LeftRail",
                        new Vector3(-0.82f, 0.95f, 2.35f),
                        new Vector3(0.08f, 1.05f, 2.7f),
                        new Color(0.75f, 0.12f, 0.14f, 1f)),
                    BoothPiece.Cube(
                        "RightRail",
                        new Vector3(0.82f, 0.95f, 2.35f),
                        new Vector3(0.08f, 1.05f, 2.7f),
                        new Color(0.75f, 0.12f, 0.14f, 1f)),
                    BoothPiece.Cube(
                        "BackStop",
                        new Vector3(0f, 1.05f, 3.9f),
                        new Vector3(1.55f, 1.2f, 0.1f),
                        new Color(0.08f, 0.16f, 0.42f, 1f)),
                    BoothPiece.Cube(
                        "Marquee",
                        new Vector3(0f, 2.72f, 3.82f),
                        new Vector3(1.7f, 0.28f, 0.12f),
                        new Color(0.95f, 0.72f, 0.12f, 1f),
                        removeCollider: true),
                },
            };
        }

        static void MarkOutOfPlay(GameObject target)
        {
            target.AddComponent<BallOutOfPlayZone>();
        }
    }
}
