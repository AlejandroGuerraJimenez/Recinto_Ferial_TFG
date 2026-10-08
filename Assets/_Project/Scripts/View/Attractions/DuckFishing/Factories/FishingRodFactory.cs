using Fairground.View.Attractions.DuckFishing;
using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions.DuckFishing.Factories
{
    /// <summary>
    /// Factory: fishing rod + hook, parented to XR controller or a desktop stand-in.
    /// </summary>
    public sealed class FishingRodFactory
    {
        public FishingRodView Create(GameObject xrOrigin)
        {
            Transform parent = ResolveRodParent(xrOrigin);
            var rodRoot = new GameObject("FishingRod");
            rodRoot.transform.SetParent(parent, false);
            rodRoot.transform.localPosition = Vector3.zero;
            rodRoot.transform.localRotation = Quaternion.identity;

            CreatePole(rodRoot.transform);
            Transform hookTransform = CreateHookVisual(rodRoot.transform);
            var hookView = CreateHookLogic(hookTransform);

            var rod = rodRoot.AddComponent<FishingRodView>();
            rod.Configure(hookTransform, hookView);
            return rod;
        }

        static Transform ResolveRodParent(GameObject xrOrigin)
        {
            if (xrOrigin != null)
            {
                foreach (var t in xrOrigin.GetComponentsInChildren<Transform>(true))
                {
                    if (t == null)
                        continue;

                    string n = t.name;
                    if (n.Contains("Right Controller") || n.Contains("RightHand") || n.Contains("Right Hand"))
                        return t;
                }
            }

            var standIn = new GameObject("DesktopRodAnchor");
            standIn.transform.position = new Vector3(0.25f, 1.2f, 0.6f);
            return standIn.transform;
        }

        static void CreatePole(Transform parent)
        {
            var pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.name = "Pole";
            pole.transform.SetParent(parent, false);
            pole.transform.localPosition = new Vector3(0f, 0f, 0.3f);
            pole.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            pole.transform.localScale = new Vector3(0.03f, 0.35f, 0.03f);
            PrimitiveMaterialApplier.Apply(pole, new Color(0.45f, 0.28f, 0.12f, 1f));
            DestroyCollider(pole);
        }

        static Transform CreateHookVisual(Transform parent)
        {
            var hook = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hook.name = "Hook";
            hook.transform.SetParent(parent, false);
            hook.transform.localPosition = new Vector3(0f, 0f, 0.55f);
            hook.transform.localScale = Vector3.one * 0.08f;
            PrimitiveMaterialApplier.Apply(hook, new Color(0.75f, 0.75f, 0.78f, 1f));

            var collider = hook.GetComponent<Collider>();
            if (collider != null)
            {
                collider.isTrigger = true;
                if (collider is SphereCollider sphere)
                    sphere.radius = 0.75f;
            }

            var body = hook.AddComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;
            return hook.transform;
        }

        static FishingHookView CreateHookLogic(Transform hookTransform)
        {
            return hookTransform.gameObject.AddComponent<FishingHookView>();
        }

        static void DestroyCollider(GameObject target)
        {
            var collider = target.GetComponent<Collider>();
            if (collider != null)
                Object.Destroy(collider);
        }
    }
}
