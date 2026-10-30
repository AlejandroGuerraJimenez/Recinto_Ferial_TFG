using System.Collections.Generic;
using UnityEngine;

namespace Fairground.View.Attractions.Rendering
{
    /// <summary>
    /// Applies a URP Lit color to primitive renderers via a shared Color→Material cache.
    /// Template: AttractionPrimitiveLit (SetTemplate / Resources). Avoids per-renderer instances.
    /// </summary>
    public static class PrimitiveMaterialApplier
    {
        const string TemplateResourceName = "AttractionPrimitiveLit";

        static Material _template;
        static readonly Dictionary<Color32, Material> Cache = new Dictionary<Color32, Material>(64);

        public static void SetTemplate(Material template)
        {
            if (ReferenceEquals(template, _template))
                return;

            _template = template;
            Cache.Clear();
        }

        public static Material Shared(Color color) => GetOrCreateShared(color);

        public static void Apply(GameObject target, Color color)
        {
            var renderer = target != null ? target.GetComponent<Renderer>() : null;
            if (renderer == null)
                return;

            var material = GetOrCreateShared(color);
            if (material == null)
                return;

            renderer.sharedMaterial = material;
        }

        static Material GetOrCreateShared(Color color)
        {
            Color32 key = color;
            if (Cache.TryGetValue(key, out var cached) && cached != null)
                return cached;

            var created = CreateMaterial(color);
            if (created == null)
                return null;

            Cache[key] = created;
            return created;
        }

        static Material CreateMaterial(Color color)
        {
            var source = ResolveTemplate();
            Material material;

            if (IsUsable(source))
            {
                material = new Material(source);
            }
            else
            {
#if UNITY_EDITOR
                var shader = ResolveShaderEditorFallback();
                if (shader == null)
                {
                    Debug.LogError("PrimitiveMaterialApplier: no URP Lit template/shader available.");
                    return null;
                }

                material = new Material(shader);
#else
                Debug.LogError(
                    "PrimitiveMaterialApplier: missing AttractionPrimitiveLit. " +
                    "Assign primitiveLitTemplate on the scene installer or place it under Resources.");
                return null;
#endif
            }

            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", color);
            material.color = color;
            return material;
        }

        static bool IsUsable(Material material)
        {
            var shader = material != null ? material.shader : null;
            return shader != null && shader.name != "Hidden/InternalErrorShader";
        }

        static Material ResolveTemplate()
        {
            if (IsUsable(_template))
                return _template;

            _template = Resources.Load<Material>(TemplateResourceName);
            return _template;
        }

#if UNITY_EDITOR
        static Shader ResolveShaderEditorFallback()
        {
            return Shader.Find("Universal Render Pipeline/Lit")
                   ?? Shader.Find("Sprites/Default")
                   ?? Shader.Find("Standard");
        }
#endif
    }
}
