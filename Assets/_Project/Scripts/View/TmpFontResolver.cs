using TMPro;
using UnityEngine;

namespace Fairground.View
{
    /// <summary>
    /// One font for world-space text. The project default wins; LiberationSans is loaded once if it is missing.
    /// </summary>
    public static class TmpFontResolver
    {
        const string FallbackResource = "Fonts & Materials/LiberationSans SDF";

        static TMP_FontAsset _fallback;
        static bool _fallbackSought;

        public static TMP_FontAsset Font()
        {
            if (TMP_Settings.defaultFontAsset != null)
                return TMP_Settings.defaultFontAsset;

            if (_fallbackSought)
                return _fallback;

            _fallbackSought = true;
            _fallback = Resources.Load<TMP_FontAsset>(FallbackResource);
            return _fallback;
        }
    }
}
