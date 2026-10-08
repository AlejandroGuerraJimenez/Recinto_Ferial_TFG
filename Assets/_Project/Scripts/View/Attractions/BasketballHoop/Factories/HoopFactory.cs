using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions.BasketballHoop.Factories
{
    /// <summary>
    /// Factory: backboard, rim, and the scoring trigger inside the hoop.
    /// </summary>
    public sealed class HoopFactory
    {
        const float RimRadius = 0.26f;
        const float RimThickness = 0.028f;
        static readonly Color RimColor = new Color(0.9f, 0.32f, 0.08f, 1f);

        public BasketDetectorView Create()
        {
            CreateBackboard();
            Transform rim = CreateRim();
            CreateSupport(rim);
            return CreateDetector(rim);
        }

        static void CreateBackboard()
        {
            var board = GameObject.CreatePrimitive(PrimitiveType.Cube);
            board.name = "Backboard";
            board.transform.position = BasketballHoopLayout.BackboardPosition;
            board.transform.localScale = new Vector3(1.2f, 1.15f, 0.06f);
            PrimitiveMaterialApplier.Apply(board, new Color(0.93f, 0.94f, 0.96f, 1f));
            CreateTargetSquare();
        }

        static void CreateTargetSquare()
        {
            var square = GameObject.CreatePrimitive(PrimitiveType.Cube);
            square.name = "TargetSquare";
            square.transform.position = BasketballHoopLayout.BackboardPosition + new Vector3(0f, 0.08f, -0.05f);
            square.transform.localScale = new Vector3(0.36f, 0.32f, 0.02f);
            DestroyCollider(square);
            PrimitiveMaterialApplier.Apply(square, new Color(0.85f, 0.15f, 0.12f, 1f));
        }

        static Transform CreateRim()
        {
            var rim = new GameObject("Rim");
            rim.transform.position = BasketballHoopLayout.RimPosition;
            var tube = new GameObject("RimTube");
            tube.transform.SetParent(rim.transform, false);

            Mesh mesh = CreateTorus(RimRadius, RimThickness, 48, 12);
            tube.AddComponent<MeshFilter>().sharedMesh = mesh;
            tube.AddComponent<MeshRenderer>();
            PrimitiveMaterialApplier.Apply(tube, RimColor);
            tube.AddComponent<MeshCollider>().sharedMesh = mesh;
            return rim.transform;
        }

        static Mesh CreateTorus(float majorRadius, float minorRadius, int majorSegments, int minorSegments)
        {
            int stride = minorSegments + 1;
            var vertices = new Vector3[(majorSegments + 1) * stride];
            var uvs = new Vector2[vertices.Length];
            var triangles = new int[majorSegments * minorSegments * 6];

            WriteRings(vertices, uvs, majorRadius, minorRadius, majorSegments, minorSegments);
            WriteTriangles(triangles, majorSegments, minorSegments, stride);

            var mesh = new Mesh { name = "RimTorus" };
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        static void WriteRings(Vector3[] vertices, Vector2[] uvs, float majorRadius, float minorRadius, int majorSegments, int minorSegments)
        {
            int stride = minorSegments + 1;
            for (int i = 0; i <= majorSegments; i++)
            {
                float around = i / (float)majorSegments * Mathf.PI * 2f;
                var center = new Vector3(Mathf.Cos(around), 0f, Mathf.Sin(around));
                for (int j = 0; j <= minorSegments; j++)
                {
                    float tube = j / (float)minorSegments * Mathf.PI * 2f;
                    Vector3 offset = center * Mathf.Cos(tube) + Vector3.up * Mathf.Sin(tube);
                    int index = i * stride + j;
                    vertices[index] = center * majorRadius + offset * minorRadius;
                    uvs[index] = new Vector2(i / (float)majorSegments, j / (float)minorSegments);
                }
            }
        }

        static void WriteTriangles(int[] triangles, int majorSegments, int minorSegments, int stride)
        {
            int t = 0;
            for (int i = 0; i < majorSegments; i++)
            {
                for (int j = 0; j < minorSegments; j++)
                {
                    int current = i * stride + j;
                    int next = current + stride;
                    triangles[t++] = current;
                    triangles[t++] = next;
                    triangles[t++] = current + 1;
                    triangles[t++] = current + 1;
                    triangles[t++] = next;
                    triangles[t++] = next + 1;
                }
            }
        }

        static void CreateSupport(Transform rim)
        {
            // Short bracket on the back of the ring, clear of the opening.
            float boardFrontZ = BasketballHoopLayout.BackboardPosition.z - 0.02f;
            float rearRingZ = rim.position.z + RimRadius;
            float length = Mathf.Max(0.08f, boardFrontZ - rearRingZ + RimThickness);
            var arm = GameObject.CreatePrimitive(PrimitiveType.Cube);
            arm.name = "RimSupport";
            arm.transform.position = new Vector3(rim.position.x, rim.position.y, rearRingZ + length * 0.5f - RimThickness);
            arm.transform.localScale = new Vector3(0.08f, 0.035f, length);
            PrimitiveMaterialApplier.Apply(arm, new Color(0.75f, 0.75f, 0.78f, 1f));
        }

        static BasketDetectorView CreateDetector(Transform rim)
        {
            var trigger = new GameObject("BasketTrigger");
            trigger.transform.SetParent(rim, false);
            var box = trigger.AddComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = new Vector3(0.32f, 0.4f, 0.32f);
            var body = trigger.AddComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;
            return trigger.AddComponent<BasketDetectorView>();
        }

        static void DestroyCollider(GameObject target)
        {
            var collider = target.GetComponent<Collider>();
            if (collider != null)
                Object.Destroy(collider);
        }
    }
}
