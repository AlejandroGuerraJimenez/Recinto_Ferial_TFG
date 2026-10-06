using UnityEngine;

namespace Fairground.View.Attractions.Rendering
{
    /// <summary>
    /// Applies a simple lit color to primitive renderers.
    /// </summary>
    public static class PrimitiveMaterialApplier
    {
        public static void Apply(GameObject target, Color color)
        {
            var renderer = target != null ? target.GetComponent<Renderer>() : null;
            if (renderer == null)
                return;

            renderer.material = CreateMaterial(color);
        }

        static Material CreateMaterial(Color color)
        {
            var material = new Material(ResolveShader()) { color = color };
            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", color);

            return material;
        }

        static Shader ResolveShader()
        {
            return Shader.Find("Universal Render Pipeline/Lit")
                   ?? Shader.Find("Sprites/Default")
                   ?? Shader.Find("Standard");
        }
    }
}
