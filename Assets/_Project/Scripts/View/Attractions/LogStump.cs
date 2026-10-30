using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// The downloaded log, stood on end. The prop spawns on the centre of the upper cut.
    /// The mesh lies along Z, so it is turned onto Y and scaled to the grab height.
    /// </summary>
    public static class LogStump
    {
        const string ResourcePath = "Models/Log/Log";
        const float TopBand = 0.08f;

        // The cut is off to one side of the mesh bounds. This is that face, after standing the log up.
        static readonly Vector3 TopFace = new Vector3(-0.159f, 0.874f, 0.002f);
        static readonly Quaternion Upright = Quaternion.Euler(-90f, 0f, 0f);

        public static ThrowStation Create(
            Vector3 position,
            Vector3 restOffset,
            Color tint,
            string logTag,
            float fallbackRadius,
            Vector3 fallbackSpawn)
        {
            GameObject source = Resources.Load<GameObject>(ResourcePath);
            MeshFilter sourceMesh = source != null ? source.GetComponentInChildren<MeshFilter>() : null;
            if (sourceMesh == null || sourceMesh.sharedMesh == null)
            {
                Debug.LogWarning(logTag + " Log mesh missing. Using the plain pedestal.");
                return ThrowStation.Create(position, fallbackRadius, fallbackSpawn);
            }

            var root = new GameObject("BallSpawner");
            root.transform.position = position;

            GameObject stump = Object.Instantiate(source, root.transform, false);
            stump.name = "Stump";
            FlattenImport(stump.transform);
            StandOnGround(stump.transform, position.y);
            Tint(stump, tint);
            AddCollider(stump);

            var spawnPoint = new GameObject("SpawnPoint");
            spawnPoint.transform.SetParent(root.transform, false);
            spawnPoint.transform.localPosition = RestPoint(stump.transform, sourceMesh.sharedMesh, restOffset);
            return new ThrowStation(root.transform, spawnPoint.transform);
        }

        static void FlattenImport(Transform stump)
        {
            Transform[] parts = stump.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i].localPosition = Vector3.zero;
                parts[i].localRotation = Quaternion.identity;
                parts[i].localScale = Vector3.one;
            }
        }

        static void StandOnGround(Transform stump, float height)
        {
            Mesh mesh = stump.GetComponentInChildren<MeshFilter>().sharedMesh;
            MeasureUpright(mesh.bounds, out float minX, out float maxX, out float maxY, out float minY, out float minZ, out float maxZ);

            float rawHeight = Mathf.Max(0.001f, maxY - minY);
            float scale = Mathf.Max(0.2f, height) / rawHeight;
            stump.localRotation = Upright;
            stump.localScale = new Vector3(scale, scale, scale);
            stump.localPosition = new Vector3(
                -((minX + maxX) * 0.5f) * scale,
                -maxY * scale,
                -((minZ + maxZ) * 0.5f) * scale);
        }

        static void MeasureUpright(Bounds bounds, out float minX, out float maxX, out float maxY, out float minY, out float minZ, out float maxZ)
        {
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;
            minX = float.PositiveInfinity;
            maxX = float.NegativeInfinity;
            minY = float.PositiveInfinity;
            maxY = float.NegativeInfinity;
            minZ = float.PositiveInfinity;
            maxZ = float.NegativeInfinity;
            for (int i = 0; i < 8; i++)
            {
                var corner = new Vector3(
                    (i & 1) == 0 ? center.x - extents.x : center.x + extents.x,
                    (i & 2) == 0 ? center.y - extents.y : center.y + extents.y,
                    (i & 4) == 0 ? center.z - extents.z : center.z + extents.z);
                Vector3 turned = Upright * corner;
                if (turned.x < minX) minX = turned.x;
                if (turned.x > maxX) maxX = turned.x;
                if (turned.y < minY) minY = turned.y;
                if (turned.y > maxY) maxY = turned.y;
                if (turned.z < minZ) minZ = turned.z;
                if (turned.z > maxZ) maxZ = turned.z;
            }
        }

        static Vector3 RestPoint(Transform stump, Mesh mesh, Vector3 restOffset)
        {
            Vector3 face = TopOf(mesh);
            float scale = stump.localScale.x;
            return stump.localPosition + face * scale + restOffset;
        }

        static Vector3 TopOf(Mesh mesh)
        {
            if (mesh == null || !mesh.isReadable)
                return TopFace;

            Vector3[] vertices = mesh.vertices;
            if (vertices.Length == 0)
                return TopFace;

            float minY = float.PositiveInfinity;
            float maxY = float.NegativeInfinity;
            var turned = new Vector3[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                turned[i] = Upright * vertices[i];
                if (turned[i].y < minY)
                    minY = turned[i].y;
                if (turned[i].y > maxY)
                    maxY = turned[i].y;
            }

            float limit = maxY - (maxY - minY) * TopBand;
            Vector3 sum = Vector3.zero;
            int count = 0;
            for (int i = 0; i < turned.Length; i++)
            {
                if (turned[i].y < limit)
                    continue;

                sum += turned[i];
                count++;
            }

            return count > 0 ? sum / count : TopFace;
        }

        static void Tint(GameObject stump, Color tint)
        {
            Renderer[] renderers = stump.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                PrimitiveMaterialApplier.Apply(renderers[i].gameObject, tint);
                Material material = renderers[i].sharedMaterial;
                Material[] slots = renderers[i].sharedMaterials;
                for (int slot = 0; slot < slots.Length; slot++)
                    slots[slot] = material;

                renderers[i].sharedMaterials = slots;
            }
        }

        static void AddCollider(GameObject stump)
        {
            MeshFilter filter = stump.GetComponentInChildren<MeshFilter>();
            Collider existing = filter.GetComponent<Collider>();
            if (existing != null)
            {
                if (Application.isPlaying)
                    Object.Destroy(existing);
                else
                    Object.DestroyImmediate(existing);
            }

            var collider = filter.gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = filter.sharedMesh;
        }
    }
}
