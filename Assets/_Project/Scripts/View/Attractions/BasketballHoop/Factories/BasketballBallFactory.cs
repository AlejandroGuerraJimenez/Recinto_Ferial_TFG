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

        static void ConfigureGrab(XRGrabInteractable grab)
        {
            grab.throwOnDetach = true;
            grab.useDynamicAttach = true;
            grab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        }
    }
}
