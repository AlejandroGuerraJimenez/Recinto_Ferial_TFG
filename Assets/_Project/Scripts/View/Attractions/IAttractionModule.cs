using Fairground.Core.Attractions;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// One attraction's scene assembly. The registry is the only list a new attraction joins.
    /// </summary>
    public interface IAttractionModule
    {
        AttractionId Id { get; }

        void Assemble(AttractionBuildContext context);
    }
}
