namespace Fairground.Model
{
    public sealed class UnlabeledHorizontalAcceptanceRule : IPlaneAcceptanceRule
    {
        public const int Priority = 1;

        public bool TryAccept(in DetectedPlane plane, out int priority)
        {
            var matches = IsMatch(plane);
            priority = matches ? Priority : 0;
            return matches;
        }

        static bool IsMatch(in DetectedPlane plane)
        {
            return plane.Classification == PlaneSemanticClassification.Unknown
                && plane.Alignment == PlaneAlignmentKind.HorizontalUp;
        }
    }
}
