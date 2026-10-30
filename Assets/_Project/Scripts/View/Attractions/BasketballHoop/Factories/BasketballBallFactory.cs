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
            GameObject source = Resources.Load<GameObject>("Models/Basketball/Ball");
            MeshFilter sourceMesh = source != null ? source.GetComponentInChildren<MeshFilter>() : null;
            if (sourceMesh == null || sourceMesh.sharedMesh == null)
                return CreatePrimitiveBall();

            var root = new GameObject("BasketballTemplate");
            GameObject visual = Object.Instantiate(source, root.transform, false);
            visual.name = "Ball";
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localScale = Vector3.one;
            Collider[] colliders = visual.GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; i++)
                Object.Destroy(colliders[i]);

            var shell = root.AddComponent<SphereCollider>();
            shell.radius = sourceMesh.sharedMesh.bounds.extents.x;
            Paint(visual);
            root.SetActive(false);
            return root;
        }

        static void Paint(GameObject visual)
        {
            Texture2D texture = Resources.Load<Texture2D>("Models/Basketball/BallAlbedo");
            Material shared = PrimitiveMaterialApplier.Shared(texture != null ? Color.white : BallColor);
            Renderer[] renderers = visual.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                if (shared == null)
                    continue;

                var material = new Material(shared);
                if (texture != null)
                {
                    material.SetTexture("_BaseMap", texture);
                    material.SetTexture("_MainTex", texture);
                    material.color = Color.white;
                    if (material.HasProperty("_BaseColor"))
                        material.SetColor("_BaseColor", Color.white);
                }

                if (material.HasProperty("_Smoothness"))
                    material.SetFloat("_Smoothness", 0.28f);

                renderers[i].sharedMaterial = material;
            }
        }

        static GameObject CreatePrimitiveBall()
        {
            var root = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            root.name = "BasketballTemplate";
            root.transform.localScale = Vector3.one * 0.22f;
            root.SetActive(false);
            PrimitiveMaterialApplier.Apply(root, BallColor);
            AddSeam(root.transform, new Vector3(0f, 0f, 90f));
            AddSeam(root.transform, new Vector3(90f, 0f, 0f));
            AddSeam(root.transform, new Vector3(0f, 0f, 40f));
            return root;
        }

        static void AddSeam(Transform ball, Vector3 euler)
        {
            var seam = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            seam.name = "Seam";
            seam.transform.SetParent(ball, false);
            seam.transform.localRotation = Quaternion.Euler(euler);
            seam.transform.localScale = new Vector3(0.035f, 0.52f, 0.035f);
            PrimitiveMaterialApplier.Apply(seam, new Color(0.08f, 0.07f, 0.06f, 1f));
            var collider = seam.GetComponent<Collider>();
            if (collider != null)
                Object.Destroy(collider);
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
