using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions.BalloonThrow.Factories
{
    /// <summary>
    /// Factory: booth environment props (ground, wall, light).
    /// </summary>
    public sealed class BoothPropFactory
    {
        public void EnsureLighting()
        {
            if (Object.FindFirstObjectByType<Light>() != null)
                return;

            CreateDirectionalLight();
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        }

        public void CreateGround()
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(2f, 1f, 2f);
            PrimitiveMaterialApplier.Apply(ground, new Color(0.25f, 0.45f, 0.28f, 1f));
        }

        public void CreateBoothBack()
        {
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "BoothBack";
            wall.transform.position = new Vector3(0f, 1.4f, 4.2f);
            wall.transform.localScale = new Vector3(4.5f, 2.8f, 0.15f);
            PrimitiveMaterialApplier.Apply(wall, new Color(0.45f, 0.25f, 0.18f, 1f));
            DestroyCollider(wall);
        }

        static void CreateDirectionalLight()
        {
            var lightGo = new GameObject("Directional Light");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            light.shadows = LightShadows.Soft;
            lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        static void DestroyCollider(GameObject target)
        {
            var collider = target.GetComponent<Collider>();
            if (collider != null)
                Object.Destroy(collider);
        }
    }
}
