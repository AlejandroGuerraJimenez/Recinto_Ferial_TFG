using System;
using UnityEngine;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// What one attraction adds on top of editor mouse-look: where the camera starts, and what the play button does.
    /// </summary>
    public sealed class DesktopPlayProfile
    {
        public Color FallbackSky = new Color(0.35f, 0.55f, 0.75f, 1f);
        public Vector3 EyePosition = new Vector3(0f, 1.5f, 0f);
        public Quaternion EyeRotation = Quaternion.identity;
        public bool UseCustomPlacement;
        public Action<Transform> Place;
        public Func<Transform, bool> AllowMove;
        public Action<Transform> OnReady;
        public Action<Transform, AttractionInput> OnFrame;
    }
}
