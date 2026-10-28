using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Builds booth ground, walls and lights from a <see cref="BoothLayout"/>.
    /// Editor scene builders bake the result; runtime uses that prefab and falls back here.
    /// </summary>
    public sealed class BoothPropFactory
    {
        public const string EnvironmentRootName = "BoothEnvironment";

        readonly BoothLayout _layout;

        public BoothPropFactory(BoothLayout layout)
        {
            _layout = layout ?? new BoothLayout();
        }

        public void EnsureLighting()
        {
            BoothLighting lighting = _layout.Lighting ?? new BoothLighting();
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = lighting.Sky;
            RenderSettings.ambientEquatorColor = lighting.Equator;
            RenderSettings.ambientGroundColor = lighting.Ground;
            RenderSettings.ambientIntensity = lighting.Intensity;

            if (Object.FindAnyObjectByType<Light>() != null)
                return;

            CreateDirectionalLight();
        }

        public GameObject CreateEnvironment()
        {
            var root = new GameObject(EnvironmentRootName);
            BoothPiece[] pieces = _layout.Pieces ?? System.Array.Empty<BoothPiece>();
            for (int i = 0; i < pieces.Length; i++)
            {
                if (pieces[i] != null)
                    CreatePiece(root.transform, pieces[i]);
            }

            return root;
        }

        static void CreatePiece(Transform root, BoothPiece piece)
        {
            Transform parent = ResolveParent(root, piece.ParentPath);
            GameObject target = piece.Empty
                ? new GameObject(piece.Name)
                : GameObject.CreatePrimitive(piece.Primitive);

            target.name = piece.Name;
            target.transform.SetParent(parent, false);
            target.transform.localPosition = piece.LocalPosition;
            target.transform.localScale = piece.LocalScale;

            if (!piece.Empty && piece.Tint)
                PrimitiveMaterialApplier.Apply(target, piece.Color);

            if (piece.RemoveCollider)
                DestroyCollider(target);

            piece.Configure?.Invoke(target);
        }

        static Transform ResolveParent(Transform root, string parentPath)
        {
            if (string.IsNullOrEmpty(parentPath))
                return root;

            Transform found = root.Find(parentPath);
            if (found != null)
                return found;

            var group = new GameObject(parentPath);
            group.transform.SetParent(root, false);
            return group.transform;
        }

        static void CreateDirectionalLight()
        {
            var lightGo = new GameObject("Directional Light");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            light.shadows = LightShadows.None;
            lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        static void DestroyCollider(GameObject target)
        {
            var collider = target.GetComponent<Collider>();
            if (collider == null)
                return;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Object.DestroyImmediate(collider);
                return;
            }
#endif
            Object.Destroy(collider);
        }
    }
}
