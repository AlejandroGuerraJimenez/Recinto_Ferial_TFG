namespace Fairground.Model
{
    public sealed class TableAcceptanceRule : IPlaneAcceptanceRule
    {
        public const int Priority = 2;

        public bool TryAccept(in DetectedPlane plane, out int priority)
        {
            var matches = plane.Classification == PlaneSemanticClassification.Table;
            priority = matches ? Priority : 0;
            return matches;
        }
    }
}
