using Fairground.Core.Planes;
using UnityEngine;

namespace Fairground.View
{
    /// <summary>
    /// Rebuilds the floor the user already confirmed, so returning from an attraction
    /// shows the parcel menu without scanning the room again.
    /// </summary>
    static class SavedPlaneSurface
    {
        public static PlaneSurfaceVisual Spawn(DetectedPlane plane, Transform parent, Material fill, Material border)
        {
            var planeObject = new GameObject("Saved Plane");
            planeObject.transform.SetParent(parent, false);
            planeObject.transform.SetPositionAndRotation(Position(plane.Pose), Rotation(plane.Pose));
            planeObject.AddComponent<MeshFilter>();
            planeObject.AddComponent<MeshRenderer>();
            planeObject.AddComponent<MeshCollider>();
            planeObject.AddComponent<LineRenderer>();

            var visual = planeObject.AddComponent<PlaneSurfaceVisual>();
            visual.SetMaterialTemplates(fill, border);
            visual.Bind(plane, true);
            visual.BuildStandaloneMesh(Boundary(plane));
            visual.ApplyStyle(PlaneVisualStyle.Selected);
            return visual;
        }

        static Vector3 Position(PlanePose pose) => new Vector3(pose.PositionX, pose.PositionY, pose.PositionZ);

        static Quaternion Rotation(PlanePose pose) => new Quaternion(pose.RotationX, pose.RotationY, pose.RotationZ, pose.RotationW);

        static Vector2[] Boundary(DetectedPlane plane)
        {
            var source = plane.Boundary;
            if (source == null || source.Count == 0)
                return System.Array.Empty<Vector2>();

            var points = new Vector2[source.Count];
            for (var i = 0; i < source.Count; i++)
                points[i] = new Vector2(source[i].X, source[i].Y);
            return points;
        }
    }
}
