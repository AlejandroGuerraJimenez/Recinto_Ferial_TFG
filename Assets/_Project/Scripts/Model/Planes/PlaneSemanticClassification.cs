namespace Fairground.Model
{
    /// <summary>
    /// Semantic label used to choose a surface for the fairground.
    /// Unknown means the device reported no label.
    /// </summary>
    public enum PlaneSemanticClassification
    {
        Unknown = 0,
        Table = 1,
        Other = 2,
    }
}
