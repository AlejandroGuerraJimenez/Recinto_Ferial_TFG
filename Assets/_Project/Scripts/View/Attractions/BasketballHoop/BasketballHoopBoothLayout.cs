using System.Collections.Generic;
using Fairground.ViewModel.Attractions.BasketballHoop;
using UnityEngine;

namespace Fairground.View.Attractions.BasketballHoop
{
    /// <summary>
    /// Carnival cabinet around the lane. The playfield and ground stay the out-of-play surfaces.
    /// </summary>
    public static class BasketballHoopBoothLayout
    {
        public const string LogTag = "[BasketballHoop]";

        public static readonly Color Sky = new Color(0.45f, 0.66f, 0.88f, 1f);
        static readonly Color Wood = new Color(0.52f, 0.36f, 0.22f, 1f);
        static readonly Color Court = new Color(0.86f, 0.45f, 0.16f, 1f);
        static readonly Color StallRed = new Color(0.78f, 0.12f, 0.14f, 1f);
        static readonly Color Cabinet = new Color(0.62f, 0.1f, 0.12f, 1f);
        static readonly Color Sign = new Color(0.96f, 0.78f, 0.18f, 1f);

        public static BoothLayout Props()
        {
            var pieces = new List<BoothPiece>
            {
                BoothPiece.Plane("Ground", new Vector3(2f, 1f, 2f), Wood, MarkOutOfPlay),
                BoothPiece.Cube(
                    "Playfield",
                    BasketballHoopLayout.PlayfieldPosition,
                    BasketballHoopLayout.PlayfieldScale,
                    Court,
                    configure: MarkOutOfPlay),
                BoothPiece.Cube(
                    "LeftRail",
                    new Vector3(-0.82f, 0.95f, 2.35f),
                    new Vector3(0.08f, 1.05f, 2.7f),
                    StallRed),
                BoothPiece.Cube(
                    "RightRail",
                    new Vector3(0.82f, 0.95f, 2.35f),
                    new Vector3(0.08f, 1.05f, 2.7f),
                    StallRed),
                BoothPiece.Cube(
                    "BackStop",
                    new Vector3(0f, 1.05f, 3.9f),
                    new Vector3(1.55f, 1.2f, 0.1f),
                    Cabinet),
                BoothPiece.Cube(
                    "Marquee",
                    new Vector3(0f, 2.72f, 3.82f),
                    new Vector3(1.7f, 0.28f, 0.12f),
                    Sign,
                    removeCollider: true),
                Trim("FrontPostLeft", new Vector3(-0.9f, 1.4f, 1.02f), new Vector3(0.14f, 2f, 0.14f), StallRed),
                Trim("FrontPostRight", new Vector3(0.9f, 1.4f, 1.02f), new Vector3(0.14f, 2f, 0.14f), StallRed),
                Trim("FrontBar", new Vector3(0f, 2.32f, 1.02f), new Vector3(1.94f, 0.1f, 0.1f), StallRed),
                Trim("BackPostLeft", new Vector3(-0.78f, 1.95f, 3.9f), new Vector3(0.14f, 1.85f, 0.14f), StallRed),
                Trim("BackPostRight", new Vector3(0.78f, 1.95f, 3.9f), new Vector3(0.14f, 1.85f, 0.14f), StallRed),
                Trim("LaneStripe", new Vector3(0f, 0.468f, 2.35f), new Vector3(0.07f, 0.012f, 2.4f), Sign),
            };

            AddBulbs(pieces);
            return new BoothLayout
            {
                LogTag = LogTag,
                Sky = Sky,
                HudPosition = BasketballHoopLayout.HudPosition,
                HudLines = BasketballHoopHud.Opening(),
                Lighting = new BoothLighting
                {
                    Sky = new Color(0.62f, 0.68f, 0.74f, 1f),
                    Equator = new Color(0.48f, 0.32f, 0.22f, 1f),
                    Ground = new Color(0.24f, 0.14f, 0.1f, 1f),
                },
                Pieces = pieces.ToArray(),
            };
        }

        static void AddBulbs(List<BoothPiece> pieces)
        {
            const int count = 7;
            for (int i = 0; i < count; i++)
            {
                float x = Mathf.Lerp(-0.6f, 0.6f, i / (float)(count - 1));
                var bulb = BoothPiece.Cube(
                    "Bulb_" + i,
                    new Vector3(x, 2.9f, 3.72f),
                    Vector3.one * 0.09f,
                    Sign,
                    removeCollider: true);
                bulb.Primitive = PrimitiveType.Sphere;
                pieces.Add(bulb);
            }
        }

        static BoothPiece Trim(string name, Vector3 position, Vector3 scale, Color color)
        {
            return BoothPiece.Cube(name, position, scale, color, removeCollider: true);
        }

        static void MarkOutOfPlay(GameObject target)
        {
            target.AddComponent<BallOutOfPlayZone>();
        }
    }
}
