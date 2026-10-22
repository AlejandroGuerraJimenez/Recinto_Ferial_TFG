namespace Fairground.Model.Attractions.FerrisWheel
{
    /// <summary>
    /// Wraps wheel angle in degrees. No engine types, so EditMode can cover a full turn.
    /// </summary>
    public static class FerrisWheelMotion
    {
        public const float FullCircleDegrees = 360f;

        public static float Advance(float angleDegrees, float degreesPerSecond, float deltaSeconds)
        {
            if (deltaSeconds <= 0f || degreesPerSecond == 0f)
                return Wrap(angleDegrees);

            return Wrap(angleDegrees + degreesPerSecond * deltaSeconds);
        }

        public static float Wrap(float angleDegrees)
        {
            float wrapped = angleDegrees % FullCircleDegrees;
            if (wrapped < 0f)
                wrapped += FullCircleDegrees;
            return wrapped;
        }
    }
}
