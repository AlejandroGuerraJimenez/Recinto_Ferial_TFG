using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow
{
    /// <summary>
    /// Editor throw: place the ball in front of the camera and launch it along the look direction.
    /// The dart itself stays on the stump until the player grabs it.
    /// </summary>
    public static class BalloonThrowDesktopPlay
    {
        const float ThrowSpeed = 7.5f;

        public static DesktopPlayProfile Profile(BallSpawnerView spawner)
        {
            return new DesktopPlayProfile
            {
                FallbackSky = BalloonThrowBoothLayout.Sky,
                EyePosition = new Vector3(0f, 1.5f, 0f),
                OnFrame = (camera, input) => Throw(camera, input, spawner),
            };
        }

        static void Throw(Transform camera, AttractionInput input, BallSpawnerView spawner)
        {
            if (camera == null || input == null || !input.PlayPressed)
                return;

            if (spawner?.CurrentBall != null)
            {
                Vector3 origin = camera.position + camera.forward * 0.4f;
                spawner.CurrentBall.transform.position = origin;
            }

            spawner?.LaunchCurrentBall(camera.forward * ThrowSpeed);
        }
    }
}
