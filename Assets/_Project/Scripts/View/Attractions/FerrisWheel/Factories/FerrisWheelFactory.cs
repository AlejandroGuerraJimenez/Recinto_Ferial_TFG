using Fairground.View.Attractions.Rendering;
using TMPro;
using UnityEngine;

namespace Fairground.View.Attractions.FerrisWheel.Factories
{
    /// <summary>
    /// Builds a primitive ferris wheel: rim, spokes, supports and hanging cabins.
    /// Moving parts have no colliders so the spin does not fight physics.
    /// </summary>
    public sealed class FerrisWheelFactory
    {
        public const string RootName = "FerrisWheel";

        static readonly Color GroundColor = new Color(0.34f, 0.46f, 0.28f, 1f);
        static readonly Color PlatformColor = new Color(0.55f, 0.16f, 0.18f, 1f);
        static readonly Color SupportColor = new Color(0.16f, 0.15f, 0.18f, 1f);
        static readonly Color RimColor = new Color(0.82f, 0.12f, 0.16f, 1f);
        static readonly Color SpokeColor = new Color(0.93f, 0.9f, 0.84f, 1f);
        static readonly Color HubColor = new Color(0.95f, 0.78f, 0.18f, 1f);
        static readonly Color BulbColor = new Color(1f, 0.86f, 0.35f, 1f);
        static readonly Color SignColor = new Color(0.12f, 0.22f, 0.48f, 1f);
        static readonly Color SignTextColor = new Color(1f, 0.92f, 0.45f, 1f);

        static readonly Color[] CabinColors =
        {
            new Color(0.86f, 0.18f, 0.22f, 1f),
            new Color(0.95f, 0.78f, 0.18f, 1f),
            new Color(0.20f, 0.45f, 0.82f, 1f),
            new Color(0.18f, 0.62f, 0.38f, 1f),
            new Color(0.93f, 0.48f, 0.16f, 1f),
            new Color(0.58f, 0.28f, 0.72f, 1f),
            new Color(0.92f, 0.92f, 0.94f, 1f),
            new Color(0.16f, 0.66f, 0.70f, 1f),
        };

        public FerrisWheelRigView Create(int cabinCount)
        {
            if (cabinCount < 1)
                cabinCount = 1;

            var root = new GameObject(RootName);
            CreateGround(root.transform);
            CreatePlatform(root.transform);
            CreateSupports(root.transform);
            CreateSign(root.transform);
            Transform wheel = CreateWheel(root.transform);
            Transform[] cabins = CreateCabins(wheel, cabinCount);

            var rig = root.AddComponent<FerrisWheelRigView>();
            rig.Configure(wheel, cabins);
            return rig;
        }

        public void EnsureLighting()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.62f, 0.72f, 0.86f, 1f);
            RenderSettings.ambientEquatorColor = new Color(0.42f, 0.42f, 0.4f, 1f);
            RenderSettings.ambientGroundColor = new Color(0.18f, 0.16f, 0.12f, 1f);
            RenderSettings.ambientIntensity = 1f;

            if (Object.FindAnyObjectByType<Light>() != null)
                return;

            var lightGo = new GameObject("Sun");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.96f, 0.88f, 1f);
            light.intensity = 1.15f;
            light.shadows = LightShadows.Soft;
            lightGo.transform.rotation = Quaternion.Euler(48f, -28f, 0f);
        }

        static void CreateGround(Transform parent)
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.SetParent(parent, false);
            ground.transform.localScale = new Vector3(2.2f, 1f, 2.2f);
            PrimitiveMaterialApplier.Apply(ground, GroundColor);
        }

        static void CreatePlatform(Transform parent)
        {
            Vector3 center = FerrisWheelLayout.WheelCenter;
            var platform = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            platform.name = "Platform";
            platform.transform.SetParent(parent, false);
            platform.transform.localPosition = new Vector3(center.x, 0.04f, center.z);
            platform.transform.localScale = new Vector3(5.2f, 0.04f, 5.2f);
            PrimitiveMaterialApplier.Apply(platform, PlatformColor);
        }

        static void CreateSupports(Transform parent)
        {
            Vector3 center = FerrisWheelLayout.WheelCenter;
            const float axleHalf = 0.62f;
            const float footX = 1.75f;
            const float footZ = 1.15f;

            CreateAFrame(parent, center, axleHalf, footX, footZ, 1f, 0.08f);
            CreateAFrame(parent, center, axleHalf, footX, footZ, -1f, 0.08f);

            var axle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            axle.name = "Axle";
            axle.transform.SetParent(parent, false);
            axle.transform.localPosition = center;
            axle.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            axle.transform.localScale = new Vector3(0.16f, axleHalf, 0.16f);
            PrimitiveMaterialApplier.Apply(axle, HubColor);
        }

        /// <summary>
        /// Rebuilds the A-frame so its feet sit at <paramref name="footY"/> in the wheel root's local space.
        /// </summary>
        public void PlaceFeet(Transform root, float footY)
        {
            RemoveNamed(root, "Leg");
            RemoveNamed(root, "Sill");
            Vector3 center = FerrisWheelLayout.WheelCenter;
            const float axleHalf = 0.62f;
            const float footX = 1.75f;
            const float footZ = 1.15f;
            CreateAFrame(root, center, axleHalf, footX, footZ, 1f, footY);
            CreateAFrame(root, center, axleHalf, footX, footZ, -1f, footY);
        }

        static void RemoveNamed(Transform parent, string childName)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Transform child = parent.GetChild(i);
                if (child.name != childName)
                    continue;

                if (Application.isPlaying)
                    Object.Destroy(child.gameObject);
                else
                    Object.DestroyImmediate(child.gameObject);
            }
        }

        static void CreateAFrame(
            Transform parent,
            Vector3 center,
            float axleHalf,
            float footX,
            float footZ,
            float side,
            float footY)
        {
            float zHub = center.z + side * axleHalf;
            float zFoot = center.z + side * footZ;
            var hub = new Vector3(0f, center.y, zHub);
            var left = new Vector3(-footX, footY, zFoot);
            var right = new Vector3(footX, footY, zFoot);
            CreateBeam(parent, "Leg", left, hub, 0.1f, SupportColor);
            CreateBeam(parent, "Leg", right, hub, 0.1f, SupportColor);
            CreateBeam(parent, "Sill", left, right, 0.1f, SupportColor);
        }

        static void CreateSign(Transform parent)
        {
            Vector3 center = FerrisWheelLayout.WheelCenter;
            var board = GameObject.CreatePrimitive(PrimitiveType.Cube);
            board.name = "Sign";
            board.transform.SetParent(parent, false);
            board.transform.localPosition = new Vector3(0f, 1.15f, center.z - 1.85f);
            board.transform.localScale = new Vector3(1.7f, 0.42f, 0.08f);
            PrimitiveMaterialApplier.Apply(board, SignColor);

            var title = new GameObject("Title");
            title.transform.SetParent(parent, false);
            title.transform.localPosition = new Vector3(0f, 1.15f, center.z - 1.91f);
            var text = title.AddComponent<TextMeshPro>();
            text.font = TmpFontResolver.Font();
            text.text = "NORIA";
            text.color = SignTextColor;
            text.alignment = TextAlignmentOptions.Center;
            text.textWrappingMode = TextWrappingModes.NoWrap;
            text.overflowMode = TextOverflowModes.Overflow;
            text.rectTransform.sizeDelta = new Vector2(1.55f, 0.34f);
            text.enableAutoSizing = true;
            text.fontSizeMin = 0.4f;
            text.fontSizeMax = 6f;
        }

        static Transform CreateWheel(Transform parent)
        {
            var wheel = new GameObject("Wheel");
            wheel.transform.SetParent(parent, false);
            wheel.transform.localPosition = FerrisWheelLayout.WheelCenter;

            CreateHub(wheel.transform);
            CreateRim(wheel.transform);
            CreateSpokes(wheel.transform);
            CreateBulbs(wheel.transform);
            return wheel.transform;
        }

        static void CreateHub(Transform wheel)
        {
            var hub = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            hub.name = "Hub";
            hub.transform.SetParent(wheel, false);
            hub.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            hub.transform.localScale = new Vector3(0.72f, 0.1f, 0.72f);
            StripCollider(hub);
            PrimitiveMaterialApplier.Apply(hub, HubColor);

            var cap = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cap.name = "HubCap";
            cap.transform.SetParent(wheel, false);
            cap.transform.localPosition = new Vector3(0f, 0f, -0.12f);
            cap.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            cap.transform.localScale = new Vector3(0.28f, 0.04f, 0.28f);
            StripCollider(cap);
            PrimitiveMaterialApplier.Apply(cap, RimColor);
        }

        static void CreateRim(Transform wheel)
        {
            int segments = FerrisWheelLayout.RimSegments;
            float arc = (Mathf.PI * 2f * FerrisWheelLayout.Radius / segments) * 1.08f;
            for (int i = 0; i < segments; i++)
            {
                float angle = (Mathf.PI * 2f * i) / segments;
                Vector3 radial = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
                Vector3 tangent = new Vector3(-radial.y, radial.x, 0f);

                var segment = GameObject.CreatePrimitive(PrimitiveType.Cube);
                segment.name = "Rim";
                segment.transform.SetParent(wheel, false);
                segment.transform.localPosition = radial * FerrisWheelLayout.Radius;
                segment.transform.localRotation = Quaternion.FromToRotation(Vector3.up, tangent);
                segment.transform.localScale = new Vector3(0.1f, arc, 0.1f);
                StripCollider(segment);
                PrimitiveMaterialApplier.Apply(segment, RimColor);
            }
        }

        static void CreateSpokes(Transform wheel)
        {
            const int spokeCount = 8;
            float step = Mathf.PI * 2f / spokeCount;
            for (int i = 0; i < spokeCount; i++)
            {
                float angle = step * i + step * 0.5f;
                Vector3 radial = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
                var spoke = GameObject.CreatePrimitive(PrimitiveType.Cube);
                spoke.name = "Spoke";
                spoke.transform.SetParent(wheel, false);
                spoke.transform.localPosition = radial * (FerrisWheelLayout.Radius * 0.5f);
                spoke.transform.localRotation = Quaternion.FromToRotation(Vector3.up, radial);
                spoke.transform.localScale = new Vector3(0.06f, FerrisWheelLayout.Radius, 0.06f);
                StripCollider(spoke);
                PrimitiveMaterialApplier.Apply(spoke, SpokeColor);
            }
        }

        static void CreateBulbs(Transform wheel)
        {
            const int bulbCount = 10;
            float step = Mathf.PI * 2f / bulbCount;
            for (int i = 0; i < bulbCount; i++)
            {
                float angle = step * i;
                Vector3 radial = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
                var bulb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                bulb.name = "Bulb";
                bulb.transform.SetParent(wheel, false);
                bulb.transform.localPosition = radial * (FerrisWheelLayout.Radius + 0.16f);
                bulb.transform.localScale = Vector3.one * 0.14f;
                StripCollider(bulb);
                PrimitiveMaterialApplier.Apply(bulb, BulbColor);
            }
        }

        static Transform[] CreateCabins(Transform wheel, int count)
        {
            var cabins = new Transform[count];
            float step = Mathf.PI * 2f / count;
            for (int i = 0; i < count; i++)
            {
                float angle = step * i;
                var pivot = new GameObject("Cabin" + i);
                pivot.transform.SetParent(wheel, false);
                pivot.transform.localPosition =
                    new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * FerrisWheelLayout.Radius;
                CreateCabinBody(pivot.transform, CabinColors[i % CabinColors.Length]);
                cabins[i] = pivot.transform;
            }

            return cabins;
        }

        static void CreateCabinBody(Transform pivot, Color color)
        {
            var hanger = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hanger.name = "Hanger";
            hanger.transform.SetParent(pivot, false);
            hanger.transform.localPosition = new Vector3(0f, -0.12f, 0f);
            hanger.transform.localScale = new Vector3(0.05f, 0.24f, 0.05f);
            StripCollider(hanger);
            PrimitiveMaterialApplier.Apply(hanger, SupportColor);

            var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Body";
            body.transform.SetParent(pivot, false);
            body.transform.localPosition = new Vector3(0f, -FerrisWheelLayout.CabinHang, 0f);
            body.transform.localScale = new Vector3(0.62f, 0.7f, 0.55f);
            StripCollider(body);
            PrimitiveMaterialApplier.Apply(body, color);

            Color roofColor = color * 0.72f;
            roofColor.a = 1f;
            var roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roof.name = "Roof";
            roof.transform.SetParent(pivot, false);
            roof.transform.localPosition = new Vector3(0f, -FerrisWheelLayout.CabinHang + 0.4f, 0f);
            roof.transform.localScale = new Vector3(0.74f, 0.08f, 0.64f);
            StripCollider(roof);
            PrimitiveMaterialApplier.Apply(roof, roofColor);

            var window = GameObject.CreatePrimitive(PrimitiveType.Cube);
            window.name = "Window";
            window.transform.SetParent(body.transform, false);
            window.transform.localPosition = new Vector3(0f, 0.08f, -0.52f);
            window.transform.localScale = new Vector3(0.55f, 0.38f, 0.06f);
            StripCollider(window);
            PrimitiveMaterialApplier.Apply(window, new Color(0.75f, 0.88f, 0.95f, 1f));
        }

        static void CreateBeam(
            Transform parent,
            string name,
            Vector3 from,
            Vector3 to,
            float thickness,
            Color color)
        {
            Vector3 delta = to - from;
            float length = delta.magnitude;
            if (length < 0.001f)
                return;

            var beam = GameObject.CreatePrimitive(PrimitiveType.Cube);
            beam.name = name;
            beam.transform.SetParent(parent, false);
            beam.transform.localPosition = (from + to) * 0.5f;
            beam.transform.localRotation = Quaternion.FromToRotation(Vector3.up, delta / length);
            beam.transform.localScale = new Vector3(thickness, length, thickness);
            PrimitiveMaterialApplier.Apply(beam, color);
        }

        static void StripCollider(GameObject target)
        {
            var collider = target.GetComponent<Collider>();
            if (collider == null)
                return;

            if (Application.isPlaying)
                Object.Destroy(collider);
            else
                Object.DestroyImmediate(collider);
        }
    }
}
