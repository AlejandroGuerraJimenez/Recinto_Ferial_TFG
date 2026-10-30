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
            var root = new GameObject("HoopAssembly");
            CreateBackboard(root.transform);
            Transform rim = CreateRim(root.transform);
            CreateSupport(root.transform);
            return CreateDetector(rim);
        }

        static void CreateBackboard(Transform parent)
        {
            var board = GameObject.CreatePrimitive(PrimitiveType.Cube);
            board.name = "Backboard";
            board.transform.SetParent(parent, false);
            board.transform.localPosition = BasketballHoopLayout.BackboardPosition;
            board.transform.localScale = new Vector3(1.2f, 1.15f, 0.06f);
            PrimitiveMaterialApplier.Apply(board, new Color(0.93f, 0.94f, 0.96f, 1f));
            CreateTargetSquare(parent);
        }

        static void CreateTargetSquare(Transform parent)
        {
            var square = GameObject.CreatePrimitive(PrimitiveType.Cube);
            square.name = "TargetSquare";
            square.transform.SetParent(parent, false);
            square.transform.localPosition = BasketballHoopLayout.BackboardPosition + new Vector3(0f, 0.08f, -0.05f);
            square.transform.localScale = new Vector3(0.36f, 0.32f, 0.02f);
            DestroyCollider(square);
            PrimitiveMaterialApplier.Apply(square, new Color(0.85f, 0.15f, 0.12f, 1f));
            CreateBoardFrame(parent);
        }

        static void CreateBoardFrame(Transform parent)
        {
            Vector3 board = BasketballHoopLayout.BackboardPosition;
            var ink = new Color(0.1f, 0.1f, 0.12f, 1f);
            Frame(parent, "FrameTop", board + new Vector3(0f, 0.55f, -0.04f), new Vector3(1.24f, 0.045f, 0.02f), ink);
            Frame(parent, "FrameBottom", board + new Vector3(0f, -0.55f, -0.04f), new Vector3(1.24f, 0.045f, 0.02f), ink);
            Frame(parent, "FrameLeft", board + new Vector3(-0.6f, 0f, -0.04f), new Vector3(0.045f, 1.1f, 0.02f), ink);
            Frame(parent, "FrameRight", board + new Vector3(0.6f, 0f, -0.04f), new Vector3(0.045f, 1.1f, 0.02f), ink);
        }

        static void Frame(Transform parent, string name, Vector3 position, Vector3 scale, Color color)
        {
            var bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bar.name = name;
            bar.transform.SetParent(parent, false);
            bar.transform.localPosition = position;
            bar.transform.localScale = scale;
            DestroyCollider(bar);
            PrimitiveMaterialApplier.Apply(bar, color);
        }

        static void CreateNet(Transform rim)
        {
            var cord = new Color(0.95f, 0.95f, 0.92f, 1f);
            const int strands = 12;
            for (int i = 0; i < strands; i++)
            {
                float turns = i / (float)strands * Mathf.PI * 2f;
                float x = Mathf.Cos(turns);
                float z = Mathf.Sin(turns);
                var strand = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                strand.name = "NetStrand";
                strand.transform.SetParent(rim, false);
                strand.transform.localPosition = new Vector3(x * 0.16f, -0.12f, z * 0.16f);
                strand.transform.localRotation = Quaternion.Euler(14f * z, 0f, -14f * x);
                strand.transform.localScale = new Vector3(0.008f, 0.13f, 0.008f);
                DestroyCollider(strand);
                PrimitiveMaterialApplier.Apply(strand, cord);
            }
        }

        static Transform CreateRim(Transform parent)
        {
            var rim = new GameObject("Rim");
            rim.transform.SetParent(parent, false);
            rim.transform.localPosition = BasketballHoopLayout.RimPosition;
            if (!TryAttachModel(rim.transform))
            {
                CreateFallbackTube(rim.transform);
                CreateNet(rim.transform);
            }

            return rim.transform;
        }

        static bool TryAttachModel(Transform rim)
        {
            GameObject ringSource = Resources.Load<GameObject>("Models/Basketball/Ring");
            MeshFilter ringMesh = ringSource != null ? ringSource.GetComponentInChildren<MeshFilter>() : null;
            if (ringMesh == null || ringMesh.sharedMesh == null)
                return false;

            GameObject ring = Object.Instantiate(ringSource, rim, false);
            ring.name = "RingModel";
            ring.transform.localPosition = Vector3.zero;
            ring.transform.localRotation = Quaternion.identity;
            ring.transform.localScale = Vector3.one;
            Tint(ring, new Color(0.8f, 0.1f, 0.02f, 1f), 0.55f);
            AddMeshCollider(ring);

            GameObject netSource = Resources.Load<GameObject>("Models/Basketball/Net");
            if (netSource != null)
            {
                GameObject net = Object.Instantiate(netSource, rim, false);
                net.name = "NetModel";
                net.transform.localPosition = Vector3.zero;
                net.transform.localRotation = Quaternion.identity;
                net.transform.localScale = Vector3.one;
                Tint(net, new Color(0.96f, 0.96f, 0.94f, 1f), 0.12f);
                StripColliders(net);
            }

            return true;
        }

        static void CreateFallbackTube(Transform rim)
        {
            var tube = new GameObject("RimTube");
            tube.transform.SetParent(rim, false);
            Mesh mesh = CreateTorus(RimRadius, RimThickness, 48, 12);
            tube.AddComponent<MeshFilter>().sharedMesh = mesh;
            tube.AddComponent<MeshRenderer>();
            PrimitiveMaterialApplier.Apply(tube, RimColor);
            tube.AddComponent<MeshCollider>().sharedMesh = mesh;
        }

        static void Tint(GameObject target, Color color, float smoothness)
        {
            Material shared = PrimitiveMaterialApplier.Shared(color);
            Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                if (shared == null)
                    continue;

                var material = new Material(shared);
                if (material.HasProperty("_Smoothness"))
                    material.SetFloat("_Smoothness", smoothness);

                renderers[i].sharedMaterial = material;
            }
        }

        static void AddMeshCollider(GameObject target)
        {
            MeshFilter[] filters = target.GetComponentsInChildren<MeshFilter>();
            for (int i = 0; i < filters.Length; i++)
            {
                StripColliders(filters[i].gameObject);
                var collider = filters[i].gameObject.AddComponent<MeshCollider>();
                collider.sharedMesh = filters[i].sharedMesh;
            }
        }

        static void StripColliders(GameObject target)
        {
            Collider[] colliders = target.GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                if (Application.isPlaying)
                    Object.Destroy(colliders[i]);
                else
                    Object.DestroyImmediate(colliders[i]);
            }
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

        static void CreateSupport(Transform parent)
        {
            // Short bracket on the back of the ring, clear of the opening.
            float boardFrontZ = BasketballHoopLayout.BackboardPosition.z - 0.02f;
            float rearRingZ = BasketballHoopLayout.RimPosition.z + RimRadius;
            float length = Mathf.Max(0.08f, boardFrontZ - rearRingZ + RimThickness);
            var arm = GameObject.CreatePrimitive(PrimitiveType.Cube);
            arm.name = "RimSupport";
            arm.transform.SetParent(parent, false);
            arm.transform.localPosition = new Vector3(
                BasketballHoopLayout.RimPosition.x,
                BasketballHoopLayout.RimPosition.y,
                rearRingZ + length * 0.5f - RimThickness);
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
