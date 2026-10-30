using System.Collections.Generic;
using Fairground.View.Attractions.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace Fairground.View.Attractions.BalloonThrow
{
    /// <summary>
    /// Short carnival blast: a flash, an expanding ring, fire, sparks, smoke and shell bits.
    /// </summary>
    public sealed class BombBurst : MonoBehaviour
    {
        const int RingPoints = 28;
        const float Golden = 2.39996323f;

        static Material _flame;
        static Material _smoke;
        readonly List<Mote> _motes = new List<Mote>(40);
        readonly MaterialPropertyBlock _color = new MaterialPropertyBlock();
        LineRenderer _ring;
        LineRenderer _ringOuter;
        float _age;

        public static GameObject Create(Vector3 position)
        {
            var root = new GameObject("Burst");
            root.transform.position = position;
            root.AddComponent<BombBurst>().Build();
            return root;
        }

        void Build()
        {
            Add(PrimitiveType.Sphere, Vector3.zero, 0f, Vector3.zero, 0.12f, 0.72f, 0.16f, new Color(1f, 0.97f, 0.82f, 1f), false, Flame());
            _ring = Ring("Ring", 0.05f);
            _ringOuter = Ring("RingOuter", 0.035f);

            for (int i = 0; i < 12; i++)
            {
                Color fire = i % 3 == 0
                    ? new Color(1f, 0.92f, 0.45f, 1f)
                    : i % 3 == 1
                        ? new Color(1f, 0.48f, 0.08f, 1f)
                        : new Color(0.92f, 0.16f, 0.05f, 1f);
                Add(
                    PrimitiveType.Sphere,
                    Spread(i, 12),
                    Mathf.Lerp(2.2f, 3.6f, (i % 5) / 4f),
                    new Vector3(0f, 0.6f, 0f),
                    0.07f,
                    0.24f,
                    Mathf.Lerp(0.34f, 0.5f, (i % 4) / 3f),
                    fire,
                    false,
                    Flame());
            }

            for (int i = 0; i < 8; i++)
            {
                Add(
                    PrimitiveType.Cube,
                    Spread(i + 3, 8),
                    Mathf.Lerp(4.2f, 6.4f, (i % 4) / 3f),
                    new Vector3(0f, -2.8f, 0f),
                    0.035f,
                    0.012f,
                    Mathf.Lerp(0.18f, 0.3f, (i % 3) / 2f),
                    new Color(1f, 0.86f, 0.35f, 1f),
                    true,
                    Flame());
            }

            for (int i = 0; i < 7; i++)
            {
                Add(
                    PrimitiveType.Sphere,
                    Spread(i + 1, 7),
                    Mathf.Lerp(0.45f, 1.05f, (i % 4) / 3f),
                    new Vector3(0f, 0.85f, 0f),
                    0.12f,
                    0.4f,
                    Mathf.Lerp(0.65f, 0.9f, (i % 3) / 2f),
                    new Color(0.22f, 0.2f, 0.18f, 0.55f),
                    false,
                    Smoke());
            }

            for (int i = 0; i < 6; i++)
            {
                var bit = Add(
                    PrimitiveType.Cube,
                    Spread(i + 2, 6),
                    Mathf.Lerp(1.4f, 2.4f, (i % 3) / 2f),
                    new Vector3(0f, -4.5f, 0f),
                    0.05f,
                    0.012f,
                    0.45f,
                    new Color(0.04f, 0.04f, 0.045f, 1f),
                    false,
                    null);
                PrimitiveMaterialApplier.Apply(bit.Transform.gameObject, new Color(0.04f, 0.04f, 0.045f, 1f));
            }
        }

        void Update()
        {
            float dt = Time.deltaTime;
            _age += dt;
            bool alive = false;
            for (int i = 0; i < _motes.Count; i++)
            {
                Mote mote = _motes[i];
                mote.Age += dt;
                float life = mote.Age / mote.Life;
                if (life >= 1f)
                {
                    mote.Transform.gameObject.SetActive(false);
                    _motes[i] = mote;
                    continue;
                }

                alive = true;
                mote.Velocity += mote.Gravity * dt;
                mote.Transform.localPosition += mote.Velocity * dt;
                float size = Mathf.Lerp(mote.StartSize, mote.EndSize, life);
                mote.Transform.localScale = mote.Spark
                    ? new Vector3(size, size, size * 4.5f)
                    : Vector3.one * size;
                if (mote.Spark)
                    mote.Transform.rotation = Quaternion.LookRotation(mote.Velocity.sqrMagnitude > 0.001f ? mote.Velocity : Vector3.forward);

                if (mote.Tint != null)
                    Paint(mote.Tint, mote.Color, mote.Smoke ? SmokeAlpha(life) : 1f - life);

                _motes[i] = mote;
            }

            PaintRing(_ring, 0.08f, 0.95f, 0.32f, 0.07f, new Color(1f, 0.62f, 0.12f, 0.9f));
            PaintRing(_ringOuter, 0.16f, 1.25f, 0.48f, 0.045f, new Color(1f, 0.28f, 0.06f, 0.7f));
            if (!alive && _age > 0.5f)
                Destroy(gameObject);
        }

        void PaintRing(LineRenderer ring, float from, float to, float life, float width, Color color)
        {
            if (ring == null)
                return;

            float u = Mathf.Clamp01(_age / life);
            float radius = Mathf.Lerp(from, to, u);
            for (int i = 0; i < RingPoints; i++)
            {
                float angle = i / (float)RingPoints * Mathf.PI * 2f;
                ring.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f));
            }

            Color faded = color;
            faded.a *= 1f - u;
            ring.startColor = faded;
            ring.endColor = faded;
            ring.widthMultiplier = Mathf.Lerp(width, width * 0.25f, u);
        }

        Mote Add(
            PrimitiveType primitive,
            Vector3 direction,
            float speed,
            Vector3 gravity,
            float startSize,
            float endSize,
            float life,
            Color color,
            bool spark,
            Material material)
        {
            var piece = GameObject.CreatePrimitive(primitive);
            piece.name = spark ? "Spark" : "Puff";
            piece.transform.SetParent(transform, false);
            piece.transform.localPosition = direction * 0.04f;
            piece.transform.localScale = Vector3.one * startSize;
            if (spark)
                piece.transform.rotation = Quaternion.LookRotation(direction);

            var collider = piece.GetComponent<Collider>();
            if (collider != null)
                Destroy(collider);

            var renderer = piece.GetComponent<Renderer>();
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            if (material != null)
            {
                renderer.sharedMaterial = material;
                Paint(renderer, color, color.a);
            }

            var mote = new Mote
            {
                Transform = piece.transform,
                Tint = material != null ? renderer : null,
                Velocity = direction * speed,
                Gravity = gravity,
                StartSize = startSize,
                EndSize = endSize,
                Life = life,
                Color = color,
                Spark = spark,
                Smoke = material == Smoke(),
            };
            _motes.Add(mote);
            return mote;
        }

        LineRenderer Ring(string name, float width)
        {
            var ringObject = new GameObject(name);
            ringObject.transform.SetParent(transform, false);
            var ring = ringObject.AddComponent<LineRenderer>();
            ring.loop = true;
            ring.positionCount = RingPoints;
            ring.useWorldSpace = false;
            ring.numCapVertices = 2;
            ring.shadowCastingMode = ShadowCastingMode.Off;
            ring.receiveShadows = false;
            ring.material = Flame();
            ring.widthMultiplier = width;
            ring.alignment = LineAlignment.View;
            return ring;
        }

        void Paint(Renderer renderer, Color color, float alpha)
        {
            color.a = alpha;
            renderer.GetPropertyBlock(_color);
            _color.SetColor("_BaseColor", color);
            _color.SetColor("_Color", color);
            renderer.SetPropertyBlock(_color);
        }

        static float SmokeAlpha(float life)
        {
            float fade = life < 0.2f ? life / 0.2f : 1f - (life - 0.2f) / 0.8f;
            return Mathf.Clamp01(fade) * 0.55f;
        }

        static Vector3 Spread(int index, int count)
        {
            float y = 1f - (index + 0.5f) / Mathf.Max(1, count) * 2f;
            float ring = Mathf.Sqrt(Mathf.Max(0f, 1f - y * y));
            float theta = index * Golden;
            var direction = new Vector3(Mathf.Cos(theta) * ring, y, Mathf.Sin(theta) * ring);
            return (direction + Vector3.up * 0.35f + Vector3.back * 0.3f).normalized;
        }

        static Material Flame() => _flame != null ? _flame : (_flame = Surface(true));

        static Material Smoke() => _smoke != null ? _smoke : (_smoke = Surface(false));

        static Material Surface(bool additive)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit")
                ?? Shader.Find("Sprites/Default")
                ?? Shader.Find("Unlit/Transparent");
            var material = new Material(shader);
            material.SetFloat("_Surface", 1f);
            material.SetFloat("_Blend", additive ? 2f : 0f);
            material.SetFloat("_ZWrite", 0f);
            material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", additive ? (int)BlendMode.One : (int)BlendMode.OneMinusSrcAlpha);
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.DisableKeyword("_ALPHAMODULATE_ON");
            material.SetOverrideTag("RenderType", "Transparent");
            material.renderQueue = (int)RenderQueue.Transparent;
            material.color = Color.white;
            return material;
        }

        struct Mote
        {
            public Transform Transform;
            public Renderer Tint;
            public Vector3 Velocity;
            public Vector3 Gravity;
            public float StartSize;
            public float EndSize;
            public float Life;
            public float Age;
            public Color Color;
            public bool Spark;
            public bool Smoke;
        }
    }
}
