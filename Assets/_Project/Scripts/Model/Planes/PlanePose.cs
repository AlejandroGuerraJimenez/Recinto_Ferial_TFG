namespace Fairground.Model
{
    /// <summary>
    /// World pose stored without engine types. Rotation is a quaternion.
    /// </summary>
    public readonly struct PlanePose
    {
        public PlanePose(float positionX, float positionY, float positionZ, float rotationX, float rotationY, float rotationZ, float rotationW)
        {
            PositionX = positionX;
            PositionY = positionY;
            PositionZ = positionZ;
            RotationX = rotationX;
            RotationY = rotationY;
            RotationZ = rotationZ;
            RotationW = rotationW;
        }

        public static PlanePose Identity { get; } = new PlanePose(0f, 0f, 0f, 0f, 0f, 0f, 1f);

        public float PositionX { get; }

        public float PositionY { get; }

        public float PositionZ { get; }

        public float RotationX { get; }

        public float RotationY { get; }

        public float RotationZ { get; }

        public float RotationW { get; }
    }
}
