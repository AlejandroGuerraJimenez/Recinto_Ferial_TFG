using Fairground.View.Attractions.PunchingBall;
using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions.PunchingBall.Factories
{
    /// <summary>
    /// Factory: builds the classic strength tower with rising puck.
    /// </summary>
    public sealed class PunchPowerMeterFactory
    {
        const float MinY = 0.2f;
        const float MaxY = 2.55f;

        public PunchPowerMeterView Create()
        {
            var root = new GameObject("PowerMeter");
            root.transform.position = new Vector3(1.1f, 0f, 3.2f);

            CreateBase(root.transform);
            CreateTower(root.transform);
            Transform puck = CreatePuck(root.transform);
            CreateBell(root.transform);

            var meter = root.AddComponent<PunchPowerMeterView>();
            meter.Configure(puck, MinY, MaxY);
            return meter;
        }

        static void CreateBase(Transform parent)
        {
            var baseGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            baseGo.name = "MeterBase";
            baseGo.transform.SetParent(parent, false);
            baseGo.transform.localPosition = new Vector3(0f, 0.08f, 0f);
            baseGo.transform.localScale = new Vector3(0.55f, 0.16f, 0.55f);
            PrimitiveMaterialApplier.Apply(baseGo, new Color(0.2f, 0.2f, 0.22f, 1f));
            DestroyCollider(baseGo);
        }

        static void CreateTower(Transform parent)
        {
            var tower = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tower.name = "Tower";
            tower.transform.SetParent(parent, false);
            tower.transform.localPosition = new Vector3(0f, 1.4f, 0f);
            tower.transform.localScale = new Vector3(0.18f, 2.6f, 0.18f);
            PrimitiveMaterialApplier.Apply(tower, new Color(0.85f, 0.75f, 0.2f, 1f));
            DestroyCollider(tower);
        }

        static Transform CreatePuck(Transform parent)
        {
            var puck = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            puck.name = "Puck";
            puck.transform.SetParent(parent, false);
            puck.transform.localPosition = new Vector3(0f, MinY, 0f);
            puck.transform.localScale = new Vector3(0.32f, 0.06f, 0.32f);
            PrimitiveMaterialApplier.Apply(puck, new Color(0.95f, 0.2f, 0.15f, 1f));
            DestroyCollider(puck);
            return puck.transform;
        }

        static void CreateBell(Transform parent)
        {
            var bell = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bell.name = "Bell";
            bell.transform.SetParent(parent, false);
            bell.transform.localPosition = new Vector3(0f, MaxY + 0.15f, 0f);
            bell.transform.localScale = Vector3.one * 0.22f;
            PrimitiveMaterialApplier.Apply(bell, new Color(0.9f, 0.85f, 0.25f, 1f));
            DestroyCollider(bell);
        }

        static void DestroyCollider(GameObject target)
        {
            var collider = target.GetComponent<Collider>();
            if (collider != null)
                Object.Destroy(collider);
        }
    }
}
