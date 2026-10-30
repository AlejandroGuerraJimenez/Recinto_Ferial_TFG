using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow.Factories
{
    /// <summary>
    /// Quaternius bomb. The skull is modeled on local +Z, so the mesh is turned
    /// to face the player. The fuse stays on top.
    /// </summary>
    public sealed class BombTargetFactory
    {
        const float Radius = 0.19f;
        const string ResourcePath = "Models/Bomb/Bomb";
        static readonly Quaternion FacePlayer = Quaternion.Euler(0f, 180f, 0f);

        static readonly Color Shell = new Color(0.02f, 0.02f, 0.025f, 1f);
        static readonly Color Metal = new Color(0.32f, 0.33f, 0.36f, 1f);
        static readonly Color Skull = new Color(0.94f, 0.95f, 0.9f, 1f);

        public BombTargetView[] CreateAll()
        {
            var parent = new GameObject("Bombs");
            Vector3[] slots = BalloonThrowBoothLayout.BombSlots;
            var bombs = new BombTargetView[slots.Length];
            for (int i = 0; i < slots.Length; i++)
                bombs[i] = CreateAt(parent.transform, slots[i], i);

            return bombs;
        }

        static BombTargetView CreateAt(Transform parent, Vector3 position, int index)
        {
            var root = new GameObject($"Bomb_{index + 1}");
            root.transform.SetParent(parent, false);
            root.transform.position = position;

            var shell = root.AddComponent<SphereCollider>();
            shell.radius = Radius + 0.02f;
            var body = root.AddComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;

            if (!TryAttachModel(root.transform))
            {
                Debug.LogWarning(BalloonThrowBoothLayout.LogTag + " Bomb mesh missing. Using a plain sphere.");
                var fallback = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                fallback.name = "Body";
                fallback.transform.SetParent(root.transform, false);
                fallback.transform.localScale = Vector3.one * (Radius * 2f);
                PrimitiveMaterialApplier.Apply(fallback, Shell);
                var collider = fallback.GetComponent<Collider>();
                if (collider != null)
                    Object.Destroy(collider);
            }

            return root.AddComponent<BombTargetView>();
        }

        static bool TryAttachModel(Transform root)
        {
            GameObject source = Resources.Load<GameObject>(ResourcePath);
            MeshFilter sourceMesh = source != null ? source.GetComponentInChildren<MeshFilter>() : null;
            if (sourceMesh == null || sourceMesh.sharedMesh == null)
                return false;

            GameObject bomb = Object.Instantiate(source, root, false);
            bomb.name = "Model";
            Mesh mesh = bomb.GetComponentInChildren<MeshFilter>().sharedMesh;
            float width = mesh.bounds.size.x;
            float scale = width > 0.001f ? (Radius * 2f) / width : 1f;
            bomb.transform.localRotation = FacePlayer;
            bomb.transform.localScale = new Vector3(scale, scale, scale);
            bomb.transform.localPosition = Vector3.zero;
            Paint(bomb);
            return true;
        }

        static void Paint(GameObject bomb)
        {
            Renderer[] renderers = bomb.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                Material[] slots = renderers[i].sharedMaterials;
                var painted = new Material[slots.Length];
                string objectName = renderers[i].gameObject.name;
                for (int slot = 0; slot < slots.Length; slot++)
                {
                    string materialName = slots[slot] != null ? slots[slot].name : string.Empty;
                    string name = materialName + " " + objectName;
                    painted[slot] = Finish(ColorFor(name, slot), SmoothnessFor(name, slot));
                }

                renderers[i].sharedMaterials = painted;
            }
        }

        static Color ColorFor(string materialName, int index)
        {
            string name = materialName.ToLowerInvariant();
            if (name.Contains("white") || name.Contains("skull"))
                return Skull;
            if (name.Contains("metal"))
                return Metal;
            if (name.Contains("black"))
                return Shell;
            if (index == 1)
                return Metal;
            if (index == 2)
                return Skull;

            return Shell;
        }

        static float SmoothnessFor(string materialName, int index)
        {
            string name = materialName.ToLowerInvariant();
            if (name.Contains("white") || name.Contains("skull") || index == 2)
                return 0.22f;
            if (name.Contains("metal") || index == 1)
                return 0.72f;

            return 0.62f;
        }

        static Material Finish(Color color, float smoothness)
        {
            Material shared = PrimitiveMaterialApplier.Shared(color);
            if (shared == null)
                return null;

            var material = new Material(shared);
            if (material.HasProperty("_Smoothness"))
                material.SetFloat("_Smoothness", smoothness);

            return material;
        }
    }
}
