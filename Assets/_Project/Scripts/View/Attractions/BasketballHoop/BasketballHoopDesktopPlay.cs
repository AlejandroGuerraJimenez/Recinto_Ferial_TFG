using UnityEngine;

namespace Fairground.View.Attractions.BasketballHoop
{
    /// <summary>
    /// Editor shot: reset the ball in front of the camera and launch it with an upward arc.
    /// </summary>
    public static class BasketballHoopDesktopPlay
    {
        const float ThrowSpeed = 9f;
        const float ArcBoost = 2.4f;

        public static DesktopPlayProfile Profile(BallSpawnerView spawner)
        {
            return new DesktopPlayProfile
            {
                FallbackSky = BasketballHoopBoothLayout.Sky,
                EyePosition = new Vector3(0f, 1.5f, 0f),
                OnFrame = (camera, input) => Throw(camera, input, spawner),
            };
        }

        static void Throw(Transform camera, AttractionInput input, BallSpawnerView spawner)
        {
            if (camera == null || input == null || !input.PlayPressed)
                return;

            BasketballBallView ball = spawner != null ? spawner.CurrentBall as BasketballBallView : null;
            if (ball != null)
            {
                Vector3 origin = camera.position + camera.forward * 0.45f;
                var body = ball.GetComponent<Rigidbody>();
                if (body != null)
                {
                    body.position = origin;
                    body.linearVelocity = Vector3.zero;
                    body.angularVelocity = Vector3.zero;
                }

                ball.transform.position = origin;
            }

            Vector3 velocity = camera.forward * ThrowSpeed + Vector3.up * ArcBoost;
            spawner?.LaunchCurrentBall(velocity);
        }
    }
}
