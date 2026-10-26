namespace Fairground.Model
{
    /// <summary>
    /// Semantic label used to choose a surface for the fairground.
    /// Unknown means the device reported no label.
    /// Wall, ceiling, door and window stay selectable. Floor and table still rank above them.
    /// </summary>
    public enum PlaneSemanticClassification
    {
        Unknown = 0,
        Table = 1,
        Other = 2,
        Floor = 3,
        Wall = 4,
        Ceiling = 5,
        Door = 6,
        Window = 7,
    }
}
