using Fairground.View.Attractions.DuckFishing;
using Fairground.View.Attractions.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

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

            Transform lineOrigin = CreatePole(rodRoot.transform);
            Transform hookTransform = CreateHookVisual(rodRoot.transform);
            LineRenderer line = CreateLine(rodRoot.transform);
            var hookView = CreateHookLogic(hookTransform);
            hookView.AssignTip(hookTransform.Find("Tip"));

            var rod = rodRoot.AddComponent<FishingRodView>();
            rod.Configure(hookTransform, hookView, lineOrigin, line);
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

        static Transform CreatePole(Transform parent)
        {
            var pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.name = "Pole";
            pole.transform.SetParent(parent, false);
            pole.transform.localPosition = new Vector3(0f, 0f, 0.3f);
            pole.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            pole.transform.localScale = new Vector3(0.03f, 0.35f, 0.03f);
            PrimitiveMaterialApplier.Apply(pole, new Color(0.45f, 0.28f, 0.12f, 1f));
            DestroyCollider(pole);

            var origin = new GameObject("LineOrigin");
            origin.transform.SetParent(pole.transform, false);
            origin.transform.localPosition = new Vector3(0f, 1f, 0f);
            return origin.transform;
        }

        static LineRenderer CreateLine(Transform parent)
        {
            var lineObject = new GameObject("Line");
            lineObject.transform.SetParent(parent, false);
            var line = lineObject.AddComponent<LineRenderer>();
            line.positionCount = 2;
            line.useWorldSpace = true;
            line.loop = false;
            line.widthMultiplier = 0.008f;
            line.numCapVertices = 4;
            line.shadowCastingMode = ShadowCastingMode.Off;
            line.receiveShadows = false;
            var cord = new Color(0.95f, 0.93f, 0.86f, 1f);
            line.startColor = cord;
            line.endColor = cord;
            var shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Sprites/Default");
            if (shader != null)
            {
                var material = new Material(shader);
                if (material.HasProperty("_BaseColor"))
                    material.SetColor("_BaseColor", cord);
                material.color = cord;
                line.material = material;
            }

            return line;
        }

        static Transform CreateHookVisual(Transform parent)
        {
            var hook = new GameObject("Hook");
            hook.transform.SetParent(parent, false);
            hook.transform.localPosition = new Vector3(0f, -0.5f, 1f);

            var metal = new Color(0.78f, 0.8f, 0.84f, 1f);
            var shank = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            shank.name = "Shank";
            shank.transform.SetParent(hook.transform, false);
            shank.transform.localPosition = new Vector3(0f, -0.05f, 0f);
            shank.transform.localScale = new Vector3(0.01f, 0.05f, 0.01f);
            PrimitiveMaterialApplier.Apply(shank, metal);
            DestroyCollider(shank);

            Vector3[] bend =
            {
                new Vector3(0f, -0.1f, 0f),
                new Vector3(0f, -0.118f, 0.012f),
                new Vector3(0f, -0.124f, 0.032f),
                new Vector3(0f, -0.108f, 0.046f),
                new Vector3(0f, -0.088f, 0.04f),
            };

            for (int i = 0; i < bend.Length; i++)
            {
                var bead = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                bead.name = "Bend";
                bead.transform.SetParent(hook.transform, false);
                bead.transform.localPosition = bend[i];
                bead.transform.localScale = Vector3.one * 0.018f;
                PrimitiveMaterialApplier.Apply(bead, metal);
                DestroyCollider(bead);
            }

            var tip = new GameObject("Tip");
            tip.transform.SetParent(hook.transform, false);
            tip.transform.localPosition = bend[bend.Length - 1];

            var sphere = hook.AddComponent<SphereCollider>();
            sphere.isTrigger = true;
            sphere.center = bend[2];
            sphere.radius = 0.045f;

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
