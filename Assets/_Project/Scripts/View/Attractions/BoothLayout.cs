using System;
using UnityEngine;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Ambient fill for a booth. Attractions that omit it get the shared fairground default.
    /// </summary>
    public sealed class BoothLighting
    {
        public Color Sky = new Color(0.55f, 0.6f, 0.7f, 1f);
        public Color Equator = new Color(0.35f, 0.35f, 0.35f, 1f);
        public Color Ground = new Color(0.15f, 0.15f, 0.12f, 1f);
        public float Intensity = 1f;
    }

    /// <summary>
    /// One primitive (or empty group) inside the booth environment.
    /// </summary>
    public sealed class BoothPiece
    {
        public string Name;
        public string ParentPath;
        public bool Empty;
        public PrimitiveType Primitive;
        public Vector3 LocalPosition;
        public Vector3 LocalScale = Vector3.one;
        public Color Color = Color.white;
        public bool Tint = true;
        public bool RemoveCollider;
        public Action<GameObject> Configure;

        public static BoothPiece Plane(string name, Vector3 scale, Color color, Action<GameObject> configure = null)
        {
            return new BoothPiece
            {
                Name = name,
                Primitive = PrimitiveType.Plane,
                LocalScale = scale,
                Color = color,
                Configure = configure,
            };
        }

        public static BoothPiece Cube(
            string name,
            Vector3 position,
            Vector3 scale,
            Color color,
            bool removeCollider = false,
            Action<GameObject> configure = null)
        {
            return new BoothPiece
            {
                Name = name,
                Primitive = PrimitiveType.Cube,
                LocalPosition = position,
                LocalScale = scale,
                Color = color,
                RemoveCollider = removeCollider,
                Configure = configure,
            };
        }

        public static BoothPiece Cylinder(
            string name,
            string parentPath,
            Vector3 position,
            Vector3 scale,
            Color color)
        {
            return new BoothPiece
            {
                Name = name,
                ParentPath = parentPath,
                Primitive = PrimitiveType.Cylinder,
                LocalPosition = position,
                LocalScale = scale,
                Color = color,
                RemoveCollider = true,
            };
        }

        public static BoothPiece Group(string name, Vector3 position)
        {
            return new BoothPiece
            {
                Name = name,
                Empty = true,
                Tint = false,
                LocalPosition = position,
            };
        }
    }

    /// <summary>
    /// Data for the shared booth prop factory: lighting plus the pieces that are unique to one attraction.
    /// </summary>
    public sealed class BoothLayout
    {
        public BoothLighting Lighting = new BoothLighting();
        public BoothPiece[] Pieces = Array.Empty<BoothPiece>();
    }
}
