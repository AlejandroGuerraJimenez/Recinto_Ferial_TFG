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

            if ((classifications & PlaneClassifications.Floor) != 0)
                return PlaneSemanticClassification.Floor;

            if (IsWall(classifications))
                return PlaneSemanticClassification.Wall;

            if ((classifications & PlaneClassifications.Ceiling) != 0)
                return PlaneSemanticClassification.Ceiling;

            if ((classifications & PlaneClassifications.DoorFrame) != 0)
                return PlaneSemanticClassification.Door;

            if ((classifications & PlaneClassifications.WindowFrame) != 0)
                return PlaneSemanticClassification.Window;

            return classifications == PlaneClassifications.None
                ? PlaneSemanticClassification.Unknown
                : PlaneSemanticClassification.Other;
        }

        public static PlaneAlignmentKind MapAlignment(PlanePose pose)
        {
            return pose.FacesUp() ? PlaneAlignmentKind.HorizontalUp : PlaneAlignmentKind.Other;
        }

        static bool IsWall(PlaneClassifications classifications)
        {
            const PlaneClassifications walls = PlaneClassifications.WallFace
                | PlaneClassifications.InvisibleWallFace
                | PlaneClassifications.InnerWallFace
                | PlaneClassifications.WallArt;
            return (classifications & walls) != 0;
        }

        public static bool Matches(ARPlane plane, DetectedPlane snapshot)
        {
            if (snapshot.Classification != MapClassification(plane.classifications))
                return false;

            var pose = WorldPose(plane);
            if (snapshot.Alignment != MapAlignment(pose) || !Close(snapshot.Pose, pose))
                return false;

            var source = plane.boundary;
            if (!source.IsCreated || snapshot.Boundary == null || snapshot.Boundary.Count != source.Length)
                return false;

            for (var i = 0; i < source.Length; i++)
            {
                if (snapshot.Boundary[i].X != source[i].x || snapshot.Boundary[i].Y != source[i].y)
                    return false;
            }

            return true;
        }

        static bool Close(PlanePose current, PlanePose next)
        {
            var dx = current.PositionX - next.PositionX;
            var dy = current.PositionY - next.PositionY;
            var dz = current.PositionZ - next.PositionZ;
            if (dx * dx + dy * dy + dz * dz > 0.000001f)
                return false;

            var dot = current.RotationX * next.RotationX
                + current.RotationY * next.RotationY
                + current.RotationZ * next.RotationZ
                + current.RotationW * next.RotationW;
            return dot > 0.9999f || dot < -0.9999f;
        }

        static DetectedPlane Snapshot(ARPlane plane)
        {
            var pose = WorldPose(plane);
            return new DetectedPlane(plane.trackableId.ToString(), pose, CopyBoundary(plane), MapClassification(plane.classifications), MapAlignment(pose));
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
            if (source.IsCreated && source.Length >= PlaneCandidateFilter.MinimumBoundaryVertices)
                return Filled(plane);

            return Rectangle(plane);
        }

        static PlanePoint[] Rectangle(ARPlane plane)
        {
            var extents = plane.extents;
            if (extents.x < 0.05f || extents.y < 0.05f)
                return System.Array.Empty<PlanePoint>();

            var center = plane.centerInPlaneSpace;
            return new[]
            {
                new PlanePoint(center.x - extents.x, center.y - extents.y),
                new PlanePoint(center.x - extents.x, center.y + extents.y),
                new PlanePoint(center.x + extents.x, center.y + extents.y),
                new PlanePoint(center.x + extents.x, center.y - extents.y),
            };
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
