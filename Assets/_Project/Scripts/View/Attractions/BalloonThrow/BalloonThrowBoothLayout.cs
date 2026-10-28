using Fairground.ViewModel.Attractions.BalloonThrow;
using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow
{
    /// <summary>
    /// Balloon booth props, sky, and HUD lines. The shared shell builds everything else.
    /// </summary>
    public static class BalloonThrowBoothLayout
    {
        public const string LogTag = "[BalloonThrow]";

        public static readonly Color Sky = new Color(0.35f, 0.55f, 0.75f, 1f);
        public static readonly Vector3 HudPosition = new Vector3(0f, 2.1f, 3.2f);
        public static readonly string[] HudLines =
        {
            "Score: 0",
            "Throws: 8",
            "Balloons: 6",
            "Pop the balloons!",
        };

        public static BoothLayout Props()
        {
            return new BoothLayout
            {
                Pieces = new[]
                {
                    BoothPiece.Plane("Ground", new Vector3(2f, 1f, 2f), new Color(0.25f, 0.45f, 0.28f, 1f)),
                    BoothPiece.Cube(
                        "BoothBack",
                        new Vector3(0f, 1.4f, 4.2f),
                        new Vector3(4.5f, 2.8f, 0.15f),
                        new Color(0.45f, 0.25f, 0.18f, 1f),
                        removeCollider: true),
                },
            };
        }

        public static string[] FormatHud(BalloonThrowViewModel viewModel)
        {
            return new[]
            {
                $"Score: {viewModel.Score}",
                $"Throws: {viewModel.ThrowsRemaining}",
                $"Balloons: {viewModel.BalloonsRemaining}",
                viewModel.StatusText,
            };
        }
    }
}
