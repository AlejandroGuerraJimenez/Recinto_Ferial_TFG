using Fairground.View.Attractions.PunchingBall;
using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions.PunchingBall.Factories
{
    /// <summary>
    /// Factory: builds the punch pad / bag target.
    /// </summary>
    public sealed class PunchPadFactory
    {
        public PunchPadView Create()
        {
            var root = new GameObject("PunchMachine");
            root.transform.position = new Vector3(0f, 0f, 2.4f);

            CreatePost(root.transform);
            CreateBag(root.transform);
            return CreatePad(root.transform);
        }

        static void CreatePost(Transform parent)
        {
            var post = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            post.name = "Post";
            post.transform.SetParent(parent, false);
            post.transform.localPosition = new Vector3(0f, 0.7f, 0.35f);
            post.transform.localScale = new Vector3(0.18f, 0.7f, 0.18f);
            PrimitiveMaterialApplier.Apply(post, new Color(0.25f, 0.25f, 0.28f, 1f));
        }

        static void CreateBag(Transform parent)
        {
            var bag = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            bag.name = "Bag";
            bag.transform.SetParent(parent, false);
            bag.transform.localPosition = new Vector3(0f, 1.15f, 0f);
            bag.transform.localScale = new Vector3(0.45f, 0.55f, 0.45f);
            PrimitiveMaterialApplier.Apply(bag, new Color(0.55f, 0.12f, 0.12f, 1f));

            var collider = bag.GetComponent<Collider>();
            if (collider != null)
                Object.Destroy(collider);
        }

        static PunchPadView CreatePad(Transform parent)
        {
            var pad = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pad.name = "PunchPad";
            pad.transform.SetParent(parent, false);
            pad.transform.localPosition = new Vector3(0f, 1.25f, -0.28f);
            pad.transform.localScale = new Vector3(0.55f, 0.55f, 0.18f);
            PrimitiveMaterialApplier.Apply(pad, new Color(0.8f, 0.15f, 0.12f, 1f));

            var collider = pad.GetComponent<Collider>();
            if (collider != null)
                collider.isTrigger = true;

            var body = pad.AddComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;

            return pad.AddComponent<PunchPadView>();
        }
    }
}

