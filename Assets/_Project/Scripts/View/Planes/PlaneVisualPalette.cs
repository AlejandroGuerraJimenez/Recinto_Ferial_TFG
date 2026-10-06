using Fairground.Model;
using UnityEngine;

namespace Fairground.View
{
    public static class PlaneVisualPalette
    {
        public static readonly Color IdleFill = new Color(0.2f, 0.78f, 0.95f, 0.24f);
        public static readonly Color IdleBorder = new Color(0.75f, 0.95f, 1f, 0.9f);

        public static Color Fill(PlaneVisualStyle style, bool table)
        {
            if (IsEmphasis(style))
                return new Color(0.45f, 1f, 0.72f, 0.5f);
            if (style == PlaneVisualStyle.Dimmed)
                return new Color(0.2f, 0.75f, 0.9f, 0.07f);
            return table ? new Color(0.2f, 0.9f, 0.55f, 0.32f) : IdleFill;
        }

        public static Color Border(PlaneVisualStyle style, bool table)
        {
            if (IsEmphasis(style))
                return new Color(0.75f, 1f, 0.85f, 1f);
            if (style == PlaneVisualStyle.Dimmed)
                return new Color(0.7f, 0.9f, 1f, 0.28f);
            return table ? new Color(0.6f, 1f, 0.75f, 0.95f) : IdleBorder;
        }

        public static float Width(PlaneVisualStyle style, bool table)
        {
            if (IsEmphasis(style))
                return 0.018f;
            if (style == PlaneVisualStyle.Dimmed)
                return 0.006f;
            return table ? 0.012f : 0.008f;
        }

        static bool IsEmphasis(PlaneVisualStyle style)
        {
            return style == PlaneVisualStyle.Highlighted || style == PlaneVisualStyle.Selected;
        }
    }
}
