namespace Fairground.Model
{
    /// <summary>
    /// Boundary vertex in plane space. X is local X and Y is local Z.
    /// </summary>
    public readonly struct PlanePoint
    {
        public PlanePoint(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X { get; }

        public float Y { get; }
    }
}
