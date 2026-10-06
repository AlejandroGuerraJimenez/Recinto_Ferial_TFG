using System;
using Fairground.Model;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Fairground.View
{
    /// <summary>
    /// Copies an <see cref="ARPlane"/> into a <see cref="DetectedPlane"/> snapshot.
    /// </summary>
    public static class DetectedPlaneMapper
    {
        public static DetectedPlane FromPlane(ARPlane plane)
        {
            if (plane == null)
                throw new ArgumentNullException(nameof(plane));

            return Snapshot(plane);
        }

        public static PlaneSemanticClassification MapClassification(PlaneClassifications classifications)
        {
            if ((classifications & PlaneClassifications.Table) != 0)
                return PlaneSemanticClassification.Table;

            return classifications == PlaneClassifications.None
                ? PlaneSemanticClassification.Unknown
                : PlaneSemanticClassification.Other;
        }

        public static PlaneAlignmentKind MapAlignment(PlaneAlignment alignment)
        {
            return alignment == PlaneAlignment.HorizontalUp
                ? PlaneAlignmentKind.HorizontalUp
                : PlaneAlignmentKind.Other;
        }

        static DetectedPlane Snapshot(ARPlane plane)
        {
            return new DetectedPlane(plane.trackableId.ToString(), WorldPose(plane), CopyBoundary(plane), MapClassification(plane.classifications), MapAlignment(plane.alignment));
        }

        static Pose WorldPose(ARPlane plane) => new Pose(plane.transform.position, plane.transform.rotation);

        static Vector2[] CopyBoundary(ARPlane plane)
        {
            var source = plane.boundary;
            if (!source.IsCreated || source.Length == 0)
                return Array.Empty<Vector2>();

            return Filled(plane);
        }

        static Vector2[] Filled(ARPlane plane)
        {
            var source = plane.boundary;
            var boundary = new Vector2[source.Length];
            for (var i = 0; i < source.Length; i++)
                boundary[i] = source[i];
            return boundary;
        }
    }
}
