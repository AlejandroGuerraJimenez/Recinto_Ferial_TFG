using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions.BasketballHoop.Factories
{
    /// <summary>
    /// Factory: booth environment props (ground, arcade cabinet, light).
    /// </summary>
    public sealed class BoothPropFactory
    {
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

        public void CreateGround()
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(2f, 1f, 2f);
            ground.AddComponent<BallOutOfPlayZone>();
            PrimitiveMaterialApplier.Apply(ground, new Color(0.22f, 0.24f, 0.22f, 1f));
        }

        public void CreateArcadeMachine()
        {
            CreatePlayfield();
            CreateRail(-1f);
            CreateRail(1f);
            CreateBackStop();
            CreateMarquee();
        }

        static void CreatePlayfield()
        {
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Playfield";
            floor.transform.position = BasketballHoopLayout.PlayfieldPosition;
            floor.transform.localScale = BasketballHoopLayout.PlayfieldScale;
            floor.AddComponent<BallOutOfPlayZone>();
            PrimitiveMaterialApplier.Apply(floor, new Color(0.16f, 0.2f, 0.32f, 1f));
        }

        static void CreateRail(float side)
        {
            var rail = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rail.name = side < 0f ? "LeftRail" : "RightRail";
            rail.transform.position = new Vector3(side * 0.82f, 0.95f, 2.35f);
            rail.transform.localScale = new Vector3(0.08f, 1.05f, 2.7f);
            PrimitiveMaterialApplier.Apply(rail, new Color(0.75f, 0.12f, 0.14f, 1f));
        }

        static void CreateBackStop()
        {
            var stop = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stop.name = "BackStop";
            stop.transform.position = new Vector3(0f, 1.05f, 3.9f);
            stop.transform.localScale = new Vector3(1.55f, 1.2f, 0.1f);
            PrimitiveMaterialApplier.Apply(stop, new Color(0.08f, 0.16f, 0.42f, 1f));
        }

        static void CreateMarquee()
        {
            var marquee = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marquee.name = "Marquee";
            marquee.transform.position = new Vector3(0f, 2.72f, 3.82f);
            marquee.transform.localScale = new Vector3(1.7f, 0.28f, 0.12f);
            DestroyCollider(marquee);
            PrimitiveMaterialApplier.Apply(marquee, new Color(0.95f, 0.72f, 0.12f, 1f));
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
