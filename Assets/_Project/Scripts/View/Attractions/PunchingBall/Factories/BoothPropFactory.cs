using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions.PunchingBall.Factories
{
    /// <summary>
    /// Factory: booth environment props (ground, wall, light).
    /// Prefer baking via Editor SceneBuilder; runtime uses prefab/scene instance.
    /// </summary>
    public sealed class BoothPropFactory
    {
        public const string EnvironmentRootName = "BoothEnvironment";

        public void EnsureLighting()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.55f, 0.6f, 0.7f, 1f);
            RenderSettings.ambientEquatorColor = new Color(0.35f, 0.35f, 0.35f, 1f);
            RenderSettings.ambientGroundColor = new Color(0.15f, 0.15f, 0.12f, 1f);
            RenderSettings.ambientIntensity = 1f;

            if (Object.FindFirstObjectByType<Light>() != null)
                return;

            CreateDirectionalLight();
        }

        /// <summary>
        /// Builds ground + back wall under a root. Used by Editor bake and runtime fallback.
        /// </summary>
        public GameObject CreateEnvironment()
        {
            var root = new GameObject(EnvironmentRootName);
            CreateGround(root.transform);
            CreateBoothBack(root.transform);
            return root;
        }

        static void CreateGround(Transform parent)
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.SetParent(parent, false);
            ground.transform.localScale = new Vector3(2f, 1f, 2f);
            PrimitiveMaterialApplier.Apply(ground, new Color(0.22f, 0.22f, 0.26f, 1f));
        }

        static void CreateBoothBack(Transform parent)
        {
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "BoothBack";
            wall.transform.SetParent(parent, false);
            wall.transform.localPosition = new Vector3(0f, 1.4f, 4.2f);
            wall.transform.localScale = new Vector3(4.5f, 2.8f, 0.15f);
            PrimitiveMaterialApplier.Apply(wall, new Color(0.35f, 0.18f, 0.14f, 1f));
            DestroyCollider(wall);
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
