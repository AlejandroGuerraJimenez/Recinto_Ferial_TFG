using Fairground.View.Attractions.Rendering;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Fairground.View.Attractions.BalloonThrow.Factories
{
    /// <summary>
    /// Factory: builds inactive throwable ball templates.
    /// </summary>
    public sealed class ThrowableBallFactory
    {
        static readonly Color BallColor = new Color(0.95f, 0.55f, 0.15f, 1f);

        public ThrowableBallView CreateTemplate()
        {
            var root = CreateSphere();
            ConfigureBody(root.AddComponent<Rigidbody>());
            ConfigureGrab(root.AddComponent<XRGrabInteractable>());
            return root.AddComponent<ThrowableBallView>();
        }

        static GameObject CreateSphere()
        {
            var root = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            root.name = "ThrowBallTemplate";
            root.transform.localScale = Vector3.one * 0.12f;
            root.SetActive(false);
            PrimitiveMaterialApplier.Apply(root, BallColor);
            return root;
        }

        static void ConfigureBody(Rigidbody body)
        {
            body.mass = 0.2f;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        static void ConfigureGrab(XRGrabInteractable grab)
        {
            grab.throwOnDetach = true;
            grab.useDynamicAttach = true;
            grab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        }
    }
}
