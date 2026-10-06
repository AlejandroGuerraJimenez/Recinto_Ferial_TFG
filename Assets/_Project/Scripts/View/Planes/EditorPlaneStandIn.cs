#if UNITY_EDITOR
using Fairground.Model;
using UnityEngine;

namespace Fairground.View
{
    /// <summary>
    /// Two horizontal stand-ins used when the editor has no plane subsystem.
    /// Device builds do not include this type.
    /// </summary>
    static class EditorPlaneStandIn
    {
        public static PlaneSurfaceVisual Spawn(Camera camera, Material fill, Material border, bool adjustCamera)
        {
            if (camera == null)
                return null;

            if (adjustCamera)
                AimCamera(camera);
            return BuildPair(camera, fill, border);
        }

        static void AimCamera(Camera camera)
        {
            var position = Raised(camera.transform.position);
            var yaw = camera.transform.eulerAngles.y;
            camera.transform.SetPositionAndRotation(position, Quaternion.Euler(35f, yaw, 0f));
        }

        static Vector3 Raised(Vector3 position)
        {
            if (position.y < 0.8f)
                position.y = 1.45f;
            return position;
        }

        static PlaneSurfaceVisual BuildPair(Camera camera, Material fill, Material border)
        {
            var root = Root(camera);
            var table = Create(root, "editor-stand-in-table", Vector3.zero, 0.6f, 0.4f, PlaneSemanticClassification.Table, fill, border);
            Create(root, "editor-stand-in-open", camera.transform.right * 1.05f, 0.45f, 0.35f, PlaneSemanticClassification.Unknown, fill, border);
            return table;
        }

        static Transform Root(Camera camera)
        {
            var center = camera.transform.position + camera.transform.forward * 1.4f;
            var root = new GameObject("Editor Plane Stand-Ins");
            root.transform.SetPositionAndRotation(center, Quaternion.identity);
            return root.transform;
        }

        static PlaneSurfaceVisual Create(Transform parent, string id, Vector3 offset, float halfWidth, float halfDepth, PlaneSemanticClassification classification, Material fill, Material border)
        {
            var planeObject = CreateObject(parent, id, offset);
            return Bind(planeObject, id, Rectangle(halfWidth, halfDepth), classification, fill, border);
        }

        static GameObject CreateObject(Transform parent, string id, Vector3 offset)
        {
            var planeObject = new GameObject(id);
            planeObject.transform.SetParent(parent, false);
            planeObject.transform.SetLocalPositionAndRotation(offset, Quaternion.identity);
            AddParts(planeObject);
            return planeObject;
        }

        static void AddParts(GameObject planeObject)
        {
            planeObject.AddComponent<MeshFilter>();
            planeObject.AddComponent<MeshRenderer>();
            planeObject.AddComponent<MeshCollider>();
            planeObject.AddComponent<LineRenderer>();
        }

        static PlaneSurfaceVisual Bind(GameObject planeObject, string id, Vector2[] boundary, PlaneSemanticClassification classification, Material fill, Material border)
        {
            var visual = planeObject.AddComponent<PlaneSurfaceVisual>();
            visual.SetMaterialTemplates(fill, border);
            visual.Bind(Snapshot(planeObject, id, boundary, classification), true);
            visual.BuildStandaloneMesh(boundary);
            visual.ApplyStyle(PlaneVisualStyle.Idle);
            return visual;
        }

        static DetectedPlane Snapshot(GameObject planeObject, string id, Vector2[] boundary, PlaneSemanticClassification classification)
        {
            var pose = new Pose(planeObject.transform.position, planeObject.transform.rotation);
            return new DetectedPlane(id, pose, boundary, classification, PlaneAlignmentKind.HorizontalUp);
        }

        static Vector2[] Rectangle(float halfWidth, float halfDepth)
        {
            return new[]
            {
                new Vector2(-halfWidth, -halfDepth),
                new Vector2(-halfWidth, halfDepth),
                new Vector2(halfWidth, halfDepth),
                new Vector2(halfWidth, -halfDepth),
            };
        }
    }
}
#endif
