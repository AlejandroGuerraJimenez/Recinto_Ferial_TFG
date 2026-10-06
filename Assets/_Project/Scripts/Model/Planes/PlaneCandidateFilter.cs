namespace Fairground.Model
{
    /// <summary>
    /// Chain of acceptance rules. The highest matching priority wins.
    /// </summary>
    public static class PlaneCandidateFilter
    {
        public const int TablePriority = TableAcceptanceRule.Priority;
        public const int UnlabeledHorizontalPriority = UnlabeledHorizontalAcceptanceRule.Priority;
        public const int MinimumBoundaryVertices = 3;

        static readonly IPlaneAcceptanceRule[] Rules =
        {
            new TableAcceptanceRule(),
            new UnlabeledHorizontalAcceptanceRule(),
        };

        public static bool TryGetPriority(in DetectedPlane plane, out int priority)
        {
            priority = 0;
            return IsComplete(plane) && BestPriority(plane, ref priority);
        }

        static bool IsComplete(in DetectedPlane plane)
        {
            return !string.IsNullOrEmpty(plane.Id) && HasBoundary(plane);
        }

        static bool HasBoundary(in DetectedPlane plane)
        {
            return plane.Boundary != null && plane.Boundary.Count >= MinimumBoundaryVertices;
        }

        static bool BestPriority(in DetectedPlane plane, ref int priority)
        {
            var accepted = false;
            for (var i = 0; i < Rules.Length; i++)
                accepted |= Accept(Rules[i], plane, ref priority);
            return accepted;
        }

        static bool Accept(IPlaneAcceptanceRule rule, in DetectedPlane plane, ref int best)
        {
            if (!rule.TryAccept(plane, out var priority) || priority <= best)
                return false;

            best = priority;
            return true;
        }
    }
}
