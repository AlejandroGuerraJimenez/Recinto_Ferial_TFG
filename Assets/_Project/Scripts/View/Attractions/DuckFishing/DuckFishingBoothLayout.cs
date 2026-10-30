using System.Collections.Generic;
using Fairground.ViewModel.Attractions.DuckFishing;
using UnityEngine;
using UnityEngine.Rendering;

namespace Fairground.View.Attractions.DuckFishing
{
    /// <summary>
    /// Pond booth props, sky, and HUD lines. The shared shell builds everything else.
    /// The water is a plane clipped to a disc: a cylinder cap reads as a plastic puck.
    /// </summary>
    public static class DuckFishingBoothLayout
    {
        public const string LogTag = "[DuckFishing]";
        public const string PondName = "Pond";
        public const int RimCount = 20;

        public const float WaterSurfaceY = 0.58f;
        public const float DuckFloatY = 0.62f;

        public static readonly Color Sky = new Color(0.45f, 0.68f, 0.9f, 1f);
        public static readonly Vector3 HudPosition = new Vector3(0f, 2.2f, 3.6f);

        static readonly Color Wood = new Color(0.52f, 0.36f, 0.22f, 1f);
        static readonly Color StallRed = new Color(0.7f, 0.13f, 0.14f, 1f);
        static readonly Color TubRed = new Color(0.62f, 0.1f, 0.12f, 1f);
        static readonly Color SignYellow = new Color(0.96f, 0.78f, 0.18f, 1f);
        static readonly Color PoolBed = new Color(0.03f, 0.18f, 0.22f, 1f);
        static readonly Color RimRed = new Color(0.78f, 0.16f, 0.14f, 1f);
        static readonly Color RimCream = new Color(0.95f, 0.86f, 0.62f, 1f);

        public static BoothLayout Props()
        {
            var pieces = new List<BoothPiece>
            {
                BoothPiece.Plane("Ground", new Vector3(2f, 1f, 2f), Wood),
                BoothPiece.Cube(
                    "BoothBack",
                    new Vector3(0f, 1.55f, 4.55f),
                    new Vector3(4.8f, 3.1f, 0.12f),
                    StallRed,
                    removeCollider: true),
                BoothPiece.Cube(
                    "Sign",
                    new Vector3(0f, 2.45f, 4.4f),
                    new Vector3(3.2f, 0.85f, 0.06f),
                    SignYellow,
                    removeCollider: true),
                BoothPiece.Cube(
                    "PostLeft",
                    new Vector3(-2.2f, 1.55f, 4.4f),
                    new Vector3(0.16f, 3.1f, 0.16f),
                    StallRed,
                    removeCollider: true),
                BoothPiece.Cube(
                    "PostRight",
                    new Vector3(2.2f, 1.55f, 4.4f),
                    new Vector3(0.16f, 3.1f, 0.16f),
                    StallRed,
                    removeCollider: true),
                Pitched(
                    "Awning",
                    new Vector3(0f, 2.95f, 4.05f),
                    new Vector3(3.6f, 0.06f, 1.05f),
                    StallRed,
                    -16f),
                BoothPiece.Group(PondName, new Vector3(0f, 0f, 2.6f)),
                BoothPiece.Cylinder(
                    "Basin",
                    PondName,
                    new Vector3(0f, 0.2f, 0f),
                    new Vector3(3.36f, 0.2f, 3.36f),
                    TubRed),
                BoothPiece.Cylinder(
                    "PoolBed",
                    PondName,
                    new Vector3(0f, 0.46f, 0f),
                    new Vector3(3.1f, 0.06f, 3.1f),
                    PoolBed),
            };

            var water = BoothPiece.Plane(
                "Water",
                new Vector3(0.284f, 1f, 0.284f),
                new Color(0.2f, 0.62f, 0.68f, 0.7f),
                FitWater);
            water.ParentPath = PondName;
            water.LocalPosition = new Vector3(0f, WaterSurfaceY, 0f);
            water.RemoveCollider = true;
            pieces.Add(water);
            AddRim(pieces);

            return new BoothLayout
            {
                LogTag = LogTag,
                Sky = Sky,
                HudPosition = HudPosition,
                HudLines = DuckFishingHud.Opening(),
                Lighting = new BoothLighting
                {
                    Sky = new Color(0.62f, 0.68f, 0.74f, 1f),
                    Equator = new Color(0.45f, 0.38f, 0.32f, 1f),
                    Ground = new Color(0.22f, 0.16f, 0.12f, 1f),
                },
                Pieces = pieces.ToArray(),
            };
        }

        static void AddRim(List<BoothPiece> pieces)
        {
            const float rimRadius = 1.58f;
            for (int i = 0; i < RimCount; i++)
            {
                float turns = (Mathf.PI * 2f * i) / RimCount;
                var slat = BoothPiece.Cube(
                    "Rim_" + i,
                    new Vector3(Mathf.Sin(turns) * rimRadius, 0.68f, Mathf.Cos(turns) * rimRadius),
                    new Vector3(0.52f, 0.18f, 0.16f),
                    i % 2 == 0 ? RimRed : RimCream,
                    removeCollider: true);
                slat.ParentPath = PondName;
                slat.LocalEulerAngles = new Vector3(0f, turns * Mathf.Rad2Deg, 0f);
                pieces.Add(slat);
            }
        }

        static BoothPiece Pitched(string name, Vector3 position, Vector3 scale, Color color, float pitch)
        {
            var piece = BoothPiece.Cube(name, position, scale, color, removeCollider: true);
            piece.LocalEulerAngles = new Vector3(pitch, 0f, 0f);
            return piece;
        }

        static void FitWater(GameObject water)
        {
            var renderer = water.GetComponent<Renderer>();
            if (renderer != null)
                renderer.shadowCastingMode = ShadowCastingMode.Off;
        }
    }
}
