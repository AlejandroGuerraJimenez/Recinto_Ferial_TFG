using UnityEngine;
using UnityEngine.Rendering;

namespace Fairground.View
{
    public static class PlaneMaterialFactory
    {
        public static Material Create(Material template, Color color)
        {
            var material = Instantiate(template);
            if (material == null)
                return null;

            Configure(material, color);
            return material;
        }

        public static void Tint(Material material, Color color)
        {
            if (material == null)
                return;

            ApplyColor(material, color);
        }

        public static void Release(Object asset)
        {
            if (asset == null)
                return;

            if (Application.isPlaying)
                Object.Destroy(asset);
            else
                Object.DestroyImmediate(asset);
        }

        static Material Instantiate(Material template)
        {
            if (template != null)
                return new Material(template);

            var shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Sprites/Default");
            return shader != null ? new Material(shader) : null;
        }

        static void Configure(Material material, Color color)
        {
            SetSurface(material);
            SetBlend(material);
            MarkTransparent(material);
            ApplyColor(material, color);
        }

        static void SetSurface(Material material)
        {
            Set(material, "_Surface", 1f);
            Set(material, "_ZWrite", 0f);
            Set(material, "_Cull", 0f);
        }

        static void SetBlend(Material material)
        {
            Set(material, "_Blend", 0f);
            Set(material, "_SrcBlend", (float)BlendMode.SrcAlpha);
            Set(material, "_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
            SetAlphaBlend(material);
        }

        static void SetAlphaBlend(Material material)
        {
            Set(material, "_SrcBlendAlpha", (float)BlendMode.One);
            Set(material, "_DstBlendAlpha", (float)BlendMode.OneMinusSrcAlpha);
        }

        static void MarkTransparent(Material material)
        {
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.SetOverrideTag("RenderType", "Transparent");
            material.renderQueue = (int)RenderQueue.Transparent;
        }

        static void Set(Material material, string property, float value)
        {
            if (material.HasProperty(property))
                material.SetFloat(property, value);
        }

        static void ApplyColor(Material material, Color color)
        {
            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", color);
            material.color = color;
        }
    }
}
