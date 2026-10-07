using System;
using Fairground.Model;
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

        static PlanePose WorldPose(ARPlane plane)
        {
            var position = plane.transform.position;
            var rotation = plane.transform.rotation;
            return new PlanePose(position.x, position.y, position.z, rotation.x, rotation.y, rotation.z, rotation.w);
        }

        static PlanePoint[] CopyBoundary(ARPlane plane)
        {
            var source = plane.boundary;
            if (!source.IsCreated || source.Length == 0)
                return System.Array.Empty<PlanePoint>();

            return Filled(plane);
        }

        static PlanePoint[] Filled(ARPlane plane)
        {
            var source = plane.boundary;
            var boundary = new PlanePoint[source.Length];
            for (var i = 0; i < source.Length; i++)
                boundary[i] = new PlanePoint(source[i].x, source[i].y);
            return boundary;
        }
    }
}
