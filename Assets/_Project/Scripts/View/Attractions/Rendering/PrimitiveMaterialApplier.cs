using UnityEngine;

namespace Fairground.View.Attractions.Rendering
{
    /// <summary>
    /// Applies a URP Lit color to primitive renderers using a build-safe material template.
    /// </summary>
    public static class PrimitiveMaterialApplier
    {
        static Material _template;

        public static void SetTemplate(Material template)
        {
            _template = template;
        }

        public static void Apply(GameObject target, Color color)
        {
            var renderer = target != null ? target.GetComponent<Renderer>() : null;
            if (renderer == null)
                return;

            renderer.material = CreateMaterial(color);
        }

        static Material CreateMaterial(Color color)
        {
            var source = ResolveTemplate();
            var material = source != null ? new Material(source) : new Material(ResolveShader());
            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", color);
            material.color = color;
            return material;
        }

        static Material ResolveTemplate()
        {
            if (_template != null)
                return _template;

            return Resources.Load<Material>("AttractionPrimitiveLit");
        }

        static Shader ResolveShader()
        {
            return Shader.Find("Universal Render Pipeline/Lit")
                   ?? Shader.Find("Sprites/Default")
                   ?? Shader.Find("Standard");
        }
    }
}
