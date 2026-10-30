using System.Collections.Generic;
using Fairground.ViewModel.Attractions.BalloonThrow;
using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow
{
    /// <summary>
    /// Carnival dart gallery. Three rows step back: the front row is closest,
    /// then the middle, then the back row against the barrels.
    /// </summary>
    public static class BalloonThrowBoothLayout
    {
        public const string LogTag = "[BalloonThrow]";
        public const float FrontZ = 3.08f;
        public const float MiddleZ = 3.42f;
        public const float BackZ = 3.78f;

        public static readonly Color Sky = new Color(0.16f, 0.14f, 0.28f, 1f);
        public static readonly Vector3 HudPosition = new Vector3(0f, 3.55f, 3.05f);
        public static readonly Vector3 SpawnerPosition = new Vector3(0.35f, 1.05f, 0.55f);
        public const float SpawnerPedestalRadius = 0.25f;
        public static readonly Vector3 SpawnerLocalOffset = new Vector3(0f, 0.05f, 0f);

        public static readonly Color Stone = new Color(0.76f, 0.73f, 0.68f, 1f);
        public static readonly Color Wood = new Color(0.78f, 0.55f, 0.34f, 1f);
        public static readonly Color StallRed = new Color(0.84f, 0.1f, 0.14f, 1f);
        public static readonly Color Barrel = new Color(0.62f, 0.67f, 0.74f, 1f);
        public static readonly Color Band = new Color(0.28f, 0.31f, 0.36f, 1f);
        public static readonly Color Ink = new Color(0.08f, 0.08f, 0.09f, 1f);
        public static readonly Color Paper = new Color(0.96f, 0.95f, 0.92f, 1f);
        public static readonly Color Back = new Color(0.36f, 0.4f, 0.48f, 1f);

        public static readonly Vector3[] TargetSlots =
        {
            new Vector3(-0.78f, 0.62f, FrontZ),
            new Vector3(0f, 0.62f, FrontZ),
            new Vector3(0.78f, 0.62f, FrontZ),
            new Vector3(-0.78f, 1.28f, MiddleZ),
            new Vector3(0.78f, 1.28f, MiddleZ),
            new Vector3(0f, 1.95f, BackZ),
        };

        public static readonly Vector3[] BombSlots =
        {
            new Vector3(-0.78f, 1.95f, BackZ),
            new Vector3(0f, 1.28f, MiddleZ),
            new Vector3(0.78f, 1.95f, BackZ),
        };

        const float ArchZ = 3.22f;
        const float SpringY = 1.82f;
        const float ArchRadius = 1.32f;
        const int ArchSegments = 11;
        const int Scallops = 7;

        public static BoothLayout Props()
        {
            var pieces = new List<BoothPiece>
            {
                BoothPiece.Plane("Ground", new Vector3(2f, 1f, 2f), Stone),
                Piece("PostLeft", null, PrimitiveType.Cube, new Vector3(-ArchRadius, 1.02f, ArchZ), Vector3.zero, new Vector3(0.36f, 2.08f, 0.44f), Wood),
                Piece("PostRight", null, PrimitiveType.Cube, new Vector3(ArchRadius, 1.02f, ArchZ), Vector3.zero, new Vector3(0.36f, 2.08f, 0.44f), Wood),
                Piece("CheekLeft", null, PrimitiveType.Cube, new Vector3(-1.5f, 1.35f, 4.02f), Vector3.zero, new Vector3(0.22f, 2.75f, 1.9f), Wood),
                Piece("CheekRight", null, PrimitiveType.Cube, new Vector3(1.5f, 1.35f, 4.02f), Vector3.zero, new Vector3(0.22f, 2.75f, 1.9f), Wood),
                Piece("BackWall", null, PrimitiveType.Cube, new Vector3(0f, 1.55f, 4.9f), Vector3.zero, new Vector3(3.05f, 3.2f, 0.12f), Back),
                Piece("Sill", null, PrimitiveType.Cube, new Vector3(0f, 0.08f, 3.35f), Vector3.zero, new Vector3(2.3f, 0.12f, 0.48f), Wood),
                Piece("Valance", null, PrimitiveType.Cube, new Vector3(0f, 2.88f, 3.36f), Vector3.zero, new Vector3(1.85f, 0.38f, 0.08f), StallRed),
            };

            AddArch(pieces);
            AddScallops(pieces);
            AddBarrels(pieces);

            return new BoothLayout
            {
                LogTag = LogTag,
                Sky = Sky,
                HudPosition = HudPosition,
                HudLines = BalloonThrowHud.Opening(),
                Lighting = new BoothLighting
                {
                    Sky = new Color(0.32f, 0.28f, 0.46f, 1f),
                    Equator = new Color(0.48f, 0.32f, 0.26f, 1f),
                    Ground = new Color(0.18f, 0.14f, 0.12f, 1f),
                },
                Pieces = pieces.ToArray(),
            };
        }

        public static (string ChildPath, string MaterialFile)[] MaterialBindings()
        {
            BoothPiece[] pieces = Props().Pieces;
            var bindings = new List<(string, string)>(pieces.Length);
            for (int i = 0; i < pieces.Length; i++)
            {
                BoothPiece piece = pieces[i];
                if (piece == null || piece.Empty)
                    continue;

                string path = string.IsNullOrEmpty(piece.ParentPath)
                    ? piece.Name
                    : piece.ParentPath + "/" + piece.Name;
                bindings.Add((path, MaterialFile(piece.Color)));
            }

            return bindings.ToArray();
        }

        public static string MaterialFile(Color color)
        {
            if (color == Stone)
                return "BoothGround.mat";
            if (color == Back)
                return "BoothBack.mat";
            if (color == Wood)
                return "BoothWood.mat";
            if (color == StallRed)
                return "BoothRed.mat";
            if (color == Barrel)
                return "BoothBarrel.mat";
            if (color == Band)
                return "BoothBand.mat";
            if (color == Paper)
                return "BoothPaper.mat";

            return "BoothInk.mat";
        }

        static void AddArch(List<BoothPiece> pieces)
        {
            for (int i = 0; i < ArchSegments; i++)
            {
                float angle = Mathf.PI - (Mathf.PI * i / (ArchSegments - 1));
                var segment = Piece(
                    "Arch_" + i,
                    null,
                    PrimitiveType.Cube,
                    new Vector3(Mathf.Cos(angle) * ArchRadius, SpringY + Mathf.Sin(angle) * ArchRadius, ArchZ),
                    new Vector3(0f, 0f, TangentRoll(angle)),
                    new Vector3(0.52f, 0.3f, 0.44f),
                    Wood);
                pieces.Add(segment);
            }
        }

        static void AddScallops(List<BoothPiece> pieces)
        {
            for (int i = 0; i < Scallops; i++)
            {
                float angle = Mathf.Lerp(2.45f, 0.69f, i / (float)(Scallops - 1));
                pieces.Add(Piece(
                    "Scallop_" + i,
                    null,
                    PrimitiveType.Sphere,
                    new Vector3(Mathf.Cos(angle) * 0.92f, SpringY + Mathf.Sin(angle) * 0.92f - 0.08f, 3.32f),
                    Vector3.zero,
                    new Vector3(0.28f, 0.22f, 0.16f),
                    StallRed));
            }
        }

        static void AddBarrels(List<BoothPiece> pieces)
        {
            float[] rows = { 0.48f, 1.1f, 1.72f };
            float[] depths = { FrontZ + 0.55f, MiddleZ + 0.55f, BackZ + 0.55f };
            float[] cols = { -0.78f, 0f, 0.78f };
            var laidFlat = new Vector3(0f, 0f, 90f);
            for (int row = 0; row < rows.Length; row++)
            {
                for (int col = 0; col < cols.Length; col++)
                {
                    string id = row + "_" + col;
                    float x = cols[col];
                    float y = rows[row];
                    float z = depths[row];
                    pieces.Add(Piece(
                        "Barrel_" + id,
                        null,
                        PrimitiveType.Cylinder,
                        new Vector3(x, y, z),
                        laidFlat,
                        new Vector3(0.6f, 0.32f, 0.6f),
                        Barrel));
                    pieces.Add(Piece(
                        "Band_" + id + "_L",
                        null,
                        PrimitiveType.Cylinder,
                        new Vector3(x - 0.22f, y, z),
                        laidFlat,
                        new Vector3(0.66f, 0.035f, 0.66f),
                        Band));
                    pieces.Add(Piece(
                        "Band_" + id + "_R",
                        null,
                        PrimitiveType.Cylinder,
                        new Vector3(x + 0.22f, y, z),
                        laidFlat,
                        new Vector3(0.66f, 0.035f, 0.66f),
                        Band));
                }
            }
        }

        static BoothPiece Piece(
            string name,
            string parent,
            PrimitiveType primitive,
            Vector3 position,
            Vector3 euler,
            Vector3 scale,
            Color color)
        {
            var piece = BoothPiece.Cube(name, position, scale, color, removeCollider: true);
            piece.ParentPath = parent;
            piece.Primitive = primitive;
            piece.LocalEulerAngles = euler;
            return piece;
        }

        static float TangentRoll(float angle)
        {
            return Mathf.Atan2(Mathf.Cos(angle), -Mathf.Sin(angle)) * Mathf.Rad2Deg;
        }
    }
}
