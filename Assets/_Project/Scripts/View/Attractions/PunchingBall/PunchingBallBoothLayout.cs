using Fairground.ViewModel.Attractions.PunchingBall;
using UnityEngine;

namespace Fairground.View.Attractions.PunchingBall
{
    /// <summary>
    /// Punch booth props, sky, and HUD lines. The shared shell builds everything else.
    /// </summary>
    public static class PunchingBallBoothLayout
    {
        public const string LogTag = "[PunchingBall]";

        public static readonly Color Sky = new Color(0.28f, 0.32f, 0.45f, 1f);
        public static readonly Vector3 HudPosition = new Vector3(-1.6f, 2.1f, 3.2f);

        public static BoothLayout Props()
        {
            return new BoothLayout
            {
                LogTag = LogTag,
                Sky = Sky,
                HudPosition = HudPosition,
                HudLines = PunchingBallHud.Opening(),
                Pieces = new[]
                {
                    BoothPiece.Plane("Ground", new Vector3(2f, 1f, 2f), new Color(0.22f, 0.22f, 0.26f, 1f)),
                    BoothPiece.Cube(
                        "BoothBack",
                        new Vector3(0f, 1.4f, 4.2f),
                        new Vector3(4.5f, 2.8f, 0.15f),
                        new Color(0.35f, 0.18f, 0.14f, 1f),
                        removeCollider: true),
                },
            };
        }
    }
}
