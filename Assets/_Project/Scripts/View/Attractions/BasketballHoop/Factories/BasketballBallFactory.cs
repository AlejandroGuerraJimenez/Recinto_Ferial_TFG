using Fairground.View.Attractions;
using Fairground.View.Attractions.Rendering;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Fairground.View.Attractions.BasketballHoop.Factories
{
    /// <summary>
    /// Factory: builds inactive throwable basketball templates.
    /// </summary>
    public sealed class BasketballBallFactory
    {
        static readonly Color BallColor = new Color(0.92f, 0.42f, 0.08f, 1f);

        public BasketballBallView CreateTemplate()
        {
            var root = CreateSphere();
            ConfigureCollider(root.GetComponent<SphereCollider>());
            ConfigureBody(root.AddComponent<Rigidbody>());
            ConfigureGrab(root.AddComponent<XRGrabInteractable>());
            // Rim is ~3 m ahead and above the hand. Scale keeps a hard shot faster
            // than a soft one; the arc fills in a flat swing so it can reach the rim.
            root.AddComponent<HeldThrowRelease>().Configure(3f, 12f, 0.38f);
            return root.AddComponent<BasketballBallView>();
        }

        static GameObject CreateSphere()
        {
            var root = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            root.name = "BasketballTemplate";
            root.transform.localScale = Vector3.one * 0.22f;
            root.SetActive(false);
            PrimitiveMaterialApplier.Apply(root, BallColor);
            return root;
        }

        static void ConfigureCollider(SphereCollider collider)
        {
            var material = new PhysicsMaterial("Basketball")
            {
                bounciness = 0.55f,
                dynamicFriction = 0.45f,
                staticFriction = 0.45f,
                bounceCombine = PhysicsMaterialCombine.Maximum,
                frictionCombine = PhysicsMaterialCombine.Average,
            };
            collider.material = material;
        }

        static void ConfigureBody(Rigidbody body)
        {
            body.mass = 0.62f;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        static void ConfigureGrab(XRGrabInteractable grab) => HeldThrowGrab.Apply(grab);
    }
}
