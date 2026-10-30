using Fairground.View.Attractions.Rendering;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Fairground.View.Attractions.BalloonThrow.Factories
{
    /// <summary>
    /// Factory: builds inactive dart templates. The point faces local +Z.
    /// </summary>
    public sealed class ThrowableBallFactory
    {
        static readonly Color BarrelColor = new Color(0.28f, 0.62f, 0.95f, 1f);
        static readonly Color GripColor = new Color(0.12f, 0.34f, 0.72f, 1f);
        static readonly Color TipColor = new Color(0.96f, 0.8f, 0.18f, 1f);
        static readonly Color FlightColor = new Color(0.9f, 0.14f, 0.18f, 1f);
        static readonly Color FlightStripe = new Color(0.96f, 0.96f, 0.96f, 1f);

        public ThrowableBallView CreateTemplate()
        {
            var root = new GameObject("DartTemplate");
            AddVisuals(root.transform);
            ConfigureCollider(root.AddComponent<CapsuleCollider>());
            ConfigureBody(root.AddComponent<Rigidbody>());
            ConfigureGrab(root.AddComponent<XRGrabInteractable>());
            // Booth targets sit about 3.3 m ahead. A wrist flick is only a few m/s,
            // so the release scale brings a normal throw up to that distance.
            root.AddComponent<HeldThrowRelease>().Configure(3.4f, 14f, 0f);
            root.SetActive(false);
            return root.AddComponent<ThrowableBallView>();
        }

        static void AddVisuals(Transform root)
        {
            AddPart(root, PrimitiveType.Cylinder, "Barrel", new Vector3(0f, 0f, 0.09f), new Vector3(90f, 0f, 0f), new Vector3(0.03f, 0.09f, 0.03f), BarrelColor);
            AddPart(root, PrimitiveType.Cylinder, "Grip", new Vector3(0f, 0f, 0.05f), new Vector3(90f, 0f, 0f), new Vector3(0.04f, 0.028f, 0.04f), GripColor);
            AddPart(root, PrimitiveType.Cylinder, "Tip", new Vector3(0f, 0f, 0.22f), new Vector3(90f, 0f, 0f), new Vector3(0.016f, 0.04f, 0.016f), TipColor);
            AddPart(root, PrimitiveType.Sphere, "Point", new Vector3(0f, 0f, 0.265f), Vector3.zero, new Vector3(0.02f, 0.02f, 0.03f), TipColor);
            AddPart(root, PrimitiveType.Cube, "FlightUp", new Vector3(0f, 0f, -0.02f), Vector3.zero, new Vector3(0.012f, 0.09f, 0.07f), FlightColor);
            AddPart(root, PrimitiveType.Cube, "FlightSide", new Vector3(0f, 0f, -0.02f), Vector3.zero, new Vector3(0.09f, 0.012f, 0.07f), FlightColor);
            AddPart(root, PrimitiveType.Cube, "FlightStripe", new Vector3(0f, 0f, -0.01f), Vector3.zero, new Vector3(0.014f, 0.055f, 0.03f), FlightStripe);
        }

        static void AddPart(
            Transform parent,
            PrimitiveType primitive,
            string partName,
            Vector3 localPosition,
            Vector3 localEuler,
            Vector3 localScale,
            Color color)
        {
            var part = GameObject.CreatePrimitive(primitive);
            part.name = partName;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localRotation = Quaternion.Euler(localEuler);
            part.transform.localScale = localScale;
            PrimitiveMaterialApplier.Apply(part, color);
            var collider = part.GetComponent<Collider>();
            if (collider != null)
                Object.DestroyImmediate(collider);
        }

        static void ConfigureCollider(CapsuleCollider capsule)
        {
            capsule.direction = 2;
            capsule.center = new Vector3(0f, 0f, 0.1f);
            capsule.height = 0.36f;
            capsule.radius = 0.045f;
        }

        static void ConfigureBody(Rigidbody body)
        {
            body.mass = 0.08f;
            body.isKinematic = true;
            body.useGravity = false;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        static void ConfigureGrab(XRGrabInteractable grab) => HeldThrowGrab.Apply(grab);
    }
}
