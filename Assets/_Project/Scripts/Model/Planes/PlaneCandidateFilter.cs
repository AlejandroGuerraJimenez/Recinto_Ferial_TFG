namespace Fairground.Model
{
    /// <summary>
    /// Keeps upward horizontal surfaces that have a boundary. A wall, door or ceiling is dropped.
    /// Floor still outranks a table, and a table outranks the rest.
    /// </summary>
    public static class PlaneCandidateFilter
    {
        public const int FloorClassRank = 2;
        public const int TableClassRank = 1;
        public const int OpenClassRank = 0;
        public const int MinimumBoundaryVertices = 3;

        public static bool TryRank(in DetectedPlane plane, out PlaneRank rank)
        {
            rank = default;
            if (!IsComplete(plane) || !IsHorizontal(plane))
                return false;

            rank = new PlaneRank(ClassRank(plane.Classification), PlaneArea.Of(plane.Boundary));
            return true;
        }

        static bool IsComplete(in DetectedPlane plane)
        {
            return !string.IsNullOrEmpty(plane.Id) && HasBoundary(plane);
        }

        static bool HasBoundary(in DetectedPlane plane)
        {
            return plane.Boundary != null && plane.Boundary.Count >= MinimumBoundaryVertices;
        }

        static bool IsHorizontal(in DetectedPlane plane)
        {
            return plane.Alignment == PlaneAlignmentKind.HorizontalUp && plane.Pose.FacesUp();
        }

        static int ClassRank(PlaneSemanticClassification classification)
        {
            if (classification == PlaneSemanticClassification.Floor)
                return FloorClassRank;

            return classification == PlaneSemanticClassification.Table ? TableClassRank : OpenClassRank;
        }
    }
}
