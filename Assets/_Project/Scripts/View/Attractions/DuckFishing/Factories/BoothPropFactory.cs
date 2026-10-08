using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions.DuckFishing.Factories
{
    /// <summary>
    /// Factory: booth environment props (ground, trough, light).
    /// </summary>
    public sealed class BoothPropFactory
    {
        public void EnsureLighting()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.55f, 0.65f, 0.75f, 1f);
            RenderSettings.ambientEquatorColor = new Color(0.35f, 0.4f, 0.4f, 1f);
            RenderSettings.ambientGroundColor = new Color(0.12f, 0.14f, 0.12f, 1f);
            RenderSettings.ambientIntensity = 1f;

            if (Object.FindFirstObjectByType<Light>() != null)
                return;

            CreateDirectionalLight();
        }

        public void CreateGround()
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(2f, 1f, 2f);
            PrimitiveMaterialApplier.Apply(ground, new Color(0.3f, 0.42f, 0.28f, 1f));
        }

        public void CreateBoothBack()
        {
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "BoothBack";
            wall.transform.position = new Vector3(0f, 1.4f, 4.4f);
            wall.transform.localScale = new Vector3(4.5f, 2.8f, 0.15f);
            PrimitiveMaterialApplier.Apply(wall, new Color(0.2f, 0.4f, 0.55f, 1f));
            DestroyCollider(wall);
        }

        public Transform CreatePond()
        {
            var pond = new GameObject("Pond");
            pond.transform.position = new Vector3(0f, 0f, 2.6f);

            var basin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            basin.name = "Basin";
            basin.transform.SetParent(pond.transform, false);
            basin.transform.localPosition = new Vector3(0f, 0.35f, 0f);
            basin.transform.localScale = new Vector3(3.2f, 0.35f, 3.2f);
            PrimitiveMaterialApplier.Apply(basin, new Color(0.35f, 0.28f, 0.18f, 1f));
            DestroyCollider(basin);

            var water = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            water.name = "Water";
            water.transform.SetParent(pond.transform, false);
            water.transform.localPosition = new Vector3(0f, 0.62f, 0f);
            water.transform.localScale = new Vector3(2.9f, 0.08f, 2.9f);
            PrimitiveMaterialApplier.Apply(water, new Color(0.2f, 0.55f, 0.75f, 1f));
            DestroyCollider(water);

            return pond.transform;
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
            if (collider != null)
                Object.Destroy(collider);
        }
    }
}
