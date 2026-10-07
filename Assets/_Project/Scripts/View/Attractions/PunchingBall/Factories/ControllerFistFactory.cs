using Fairground.View.Attractions.PunchingBall;
using UnityEngine;

namespace Fairground.View.Attractions.PunchingBall.Factories
{
    /// <summary>
    /// Factory: adds punch colliders to XR controller transforms.
    /// </summary>
    public sealed class ControllerFistFactory
    {
        public void AttachFists(GameObject xrOrigin)
        {
            if (xrOrigin == null)
                return;

            foreach (var controller in FindControllers(xrOrigin.transform))
                EnsureFist(controller);
        }

        static Transform[] FindControllers(Transform root)
        {
            var matches = new System.Collections.Generic.List<Transform>();
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
            {
                if (t == null)
                    continue;

                string n = t.name;
                if (n.Contains("Left Controller") || n.Contains("Right Controller")
                    || n.Contains("LeftHand") || n.Contains("RightHand")
                    || n.Contains("Left Hand") || n.Contains("Right Hand"))
                    matches.Add(t);
            }

            return matches.ToArray();
        }

        static void EnsureFist(Transform controller)
        {
            if (controller.Find("PunchFist") != null)
                return;

            var fist = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            fist.name = "PunchFist";
            fist.transform.SetParent(controller, false);
            fist.transform.localPosition = new Vector3(0f, 0f, 0.08f);
            fist.transform.localScale = Vector3.one * 0.12f;

            var renderer = fist.GetComponent<Renderer>();
            if (renderer != null)
                renderer.enabled = false;

            var body = fist.AddComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            fist.AddComponent<FistVelocityTracker>();
        }
    }
}

