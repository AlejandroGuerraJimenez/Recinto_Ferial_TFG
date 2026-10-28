using UnityEngine;

namespace Fairground.View.Attractions.DuckFishing
{
    /// <summary>
    /// Editor fishing: parent the rod to the desktop camera. The dip itself stays on the attraction input.
    /// </summary>
    public static class DuckFishingDesktopPlay
    {
        public static DesktopPlayProfile Profile(FishingRodView rod)
        {
            return new DesktopPlayProfile
            {
                FallbackSky = DuckFishingBoothLayout.Sky,
                EyePosition = new Vector3(0f, 1.45f, 0.35f),
                OnReady = camera => AttachRod(camera, rod),
            };
        }

        static void AttachRod(Transform camera, FishingRodView rod)
        {
            if (rod == null || camera == null)
                return;

            rod.transform.SetParent(camera, false);
            rod.transform.localPosition = new Vector3(0.2f, -0.15f, 0.35f);
            rod.transform.localRotation = Quaternion.identity;
        }
    }
}
