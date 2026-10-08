using Fairground.View.Attractions.Rendering;
using UnityEngine;

namespace Fairground.View.Attractions.BasketballHoop.Factories
{
    /// <summary>
    /// Factory: backboard, rim, and the scoring trigger inside the hoop.
    /// </summary>
    public sealed class HoopFactory
    {
        const int RimSegments = 12;
        const float RimRadius = 0.26f;

        public BasketDetectorView Create()
        {
            CreateBackboard();
            Transform rim = CreateRim();
            CreateSupport(rim);
            return CreateDetector(rim);
        }

        static void CreateBackboard()
        {
            var board = GameObject.CreatePrimitive(PrimitiveType.Cube);
            board.name = "Backboard";
            board.transform.position = BasketballHoopLayout.BackboardPosition;
            board.transform.localScale = new Vector3(1.2f, 1.15f, 0.06f);
            PrimitiveMaterialApplier.Apply(board, new Color(0.93f, 0.94f, 0.96f, 1f));
            CreateTargetSquare();
        }

        static void CreateTargetSquare()
        {
            var square = GameObject.CreatePrimitive(PrimitiveType.Cube);
            square.name = "TargetSquare";
            square.transform.position = BasketballHoopLayout.BackboardPosition + new Vector3(0f, 0.08f, -0.05f);
            square.transform.localScale = new Vector3(0.36f, 0.32f, 0.02f);
            DestroyCollider(square);
            PrimitiveMaterialApplier.Apply(square, new Color(0.85f, 0.15f, 0.12f, 1f));
        }

        static Transform CreateRim()
        {
            var rim = new GameObject("Rim");
            rim.transform.position = BasketballHoopLayout.RimPosition;

            for (int i = 0; i < RimSegments; i++)
                CreateRimSegment(rim.transform, i);

            return rim.transform;
        }

        static void CreateRimSegment(Transform rim, int index)
        {
            float angle = index * Mathf.PI * 2f / RimSegments;
            var segment = GameObject.CreatePrimitive(PrimitiveType.Cube);
            segment.name = $"RimSegment_{index + 1}";
            segment.transform.SetParent(rim, false);
            segment.transform.localPosition = new Vector3(Mathf.Cos(angle) * RimRadius, 0f, Mathf.Sin(angle) * RimRadius);
            segment.transform.localRotation = Quaternion.Euler(0f, -angle * Mathf.Rad2Deg, 0f);
            segment.transform.localScale = new Vector3(0.15f, 0.045f, 0.05f);
            PrimitiveMaterialApplier.Apply(segment, new Color(0.9f, 0.32f, 0.08f, 1f));
        }

        static void CreateSupport(Transform rim)
        {
            // Bracket sits above the opening so it never blocks a downward entry.
            var arm = GameObject.CreatePrimitive(PrimitiveType.Cube);
            arm.name = "RimSupport";
            arm.transform.position = rim.position + new Vector3(0f, 0.32f, 0.14f);
            arm.transform.localScale = new Vector3(0.06f, 0.05f, 0.24f);
            PrimitiveMaterialApplier.Apply(arm, new Color(0.75f, 0.75f, 0.78f, 1f));
        }

        static BasketDetectorView CreateDetector(Transform rim)
        {
            var trigger = new GameObject("BasketTrigger");
            trigger.transform.SetParent(rim, false);
            var box = trigger.AddComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = new Vector3(0.32f, 0.4f, 0.32f);
            var body = trigger.AddComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;
            return trigger.AddComponent<BasketDetectorView>();
        }

        static void DestroyCollider(GameObject target)
        {
            var collider = target.GetComponent<Collider>();
            if (collider != null)
                Object.Destroy(collider);
        }
    }
}
