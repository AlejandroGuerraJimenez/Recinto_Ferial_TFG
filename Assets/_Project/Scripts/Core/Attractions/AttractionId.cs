namespace Fairground.Core.Attractions
{
    /// <summary>
    /// Stable id of an attraction. Values are serialized on the fairground flow.
    /// Visible name and scene live on <see cref="AttractionDescriptor"/>.
    /// </summary>
    public enum AttractionId
    {
        None = 0,
        BalloonThrow = 1,
        DuckFishing = 2,
        PunchingBall = 3,
        FerrisWheel = 4,
        BasketballHoop = 5,
    }
}
