using UnityEngine;

namespace Fairground.View.Attractions.BasketballHoop
{
    /// <summary>
    /// Shared booth dimensions so the hoop, lane, and spawner line up.
    /// </summary>
    public static class BasketballHoopLayout
    {
        public static readonly Vector3 RimPosition = new Vector3(0f, 1.78f, 3.44f);
        public static readonly Vector3 BackboardPosition = new Vector3(0f, 2.0f, 3.82f);
        public static readonly Vector3 SpawnerPosition = new Vector3(0.42f, 1.05f, 0.65f);
        public static readonly Vector3 PlayfieldPosition = new Vector3(0f, 0.42f, 2.35f);
        public static readonly Vector3 PlayfieldScale = new Vector3(1.55f, 0.08f, 2.7f);
        public static readonly Vector3 HudPosition = new Vector3(0f, 2.62f, 3.35f);
    }
}
