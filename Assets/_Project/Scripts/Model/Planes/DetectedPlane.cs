using System.Collections.Generic;
using UnityEngine;

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
            Pose pose,
            IReadOnlyList<Vector2> boundary,
            PlaneSemanticClassification classification,
            PlaneAlignmentKind alignment)
        {
            Id = id ?? string.Empty;
            Pose = pose;
            Boundary = boundary ?? System.Array.Empty<Vector2>();
            Classification = classification;
            Alignment = alignment;
        }

        public string Id { get; }

        /// <summary>World pose of the plane origin.</summary>
        public Pose Pose { get; }

        public IReadOnlyList<Vector2> Boundary { get; }

        public PlaneSemanticClassification Classification { get; }

        public PlaneAlignmentKind Alignment { get; }
    }
}
