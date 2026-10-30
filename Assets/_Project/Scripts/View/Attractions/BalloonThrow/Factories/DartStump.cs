using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow.Factories
{
    /// <summary>
    /// Darts rest on the shared log. The pivot sits behind the barrel, so the
    /// offset puts the middle of the dart on the cut.
    /// </summary>
    public static class DartStump
    {
        static readonly Vector3 DartOnFace = new Vector3(0f, 0.04f, -0.1f);

        public static ThrowStation Create(Vector3 position, Vector3 spawnLocalPosition)
        {
            return LogStump.Create(
                position,
                DartOnFace,
                BalloonThrowBoothLayout.Wood,
                BalloonThrowBoothLayout.LogTag,
                BalloonThrowBoothLayout.SpawnerPedestalRadius,
                spawnLocalPosition);
        }
    }
}
