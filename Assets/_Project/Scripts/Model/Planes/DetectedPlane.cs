using System.Collections.Generic;

namespace Fairground.Model
{
    /// <summary>
    /// Snapshot of a detected flat surface. Boundary vertices are in plane space:
    /// X is local X and Y is local Z.
    /// </summary>
    public readonly struct DetectedPlane
    {
        public DetectedPlane(
            string id,
            PlanePose pose,
            IReadOnlyList<PlanePoint> boundary,
            PlaneSemanticClassification classification,
            PlaneAlignmentKind alignment)
        {
            Id = id ?? string.Empty;
            Pose = pose;
            Boundary = boundary ?? System.Array.Empty<PlanePoint>();
            Classification = classification;
            Alignment = alignment;
        }

        public string Id { get; }

        /// <summary>World pose of the plane origin.</summary>
        public PlanePose Pose { get; }

        public IReadOnlyList<PlanePoint> Boundary { get; }

        public PlaneSemanticClassification Classification { get; }

        public PlaneAlignmentKind Alignment { get; }
    }
}
