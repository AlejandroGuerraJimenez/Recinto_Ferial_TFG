using System.Collections.Generic;
using Fairground.Model;
using UnityEngine;

namespace Fairground.View
{
    public enum PlaneVisualStyle
    {
        Hidden = 0,
        Idle = 1,
        Dimmed = 2,
        Highlighted = 3,
        Selected = 4,
    }

    /// <summary>
    /// Semi-transparent fill plus a boundary line for one detected surface.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PlaneSurfaceVisual : MonoBehaviour
    {
        [SerializeField] Material m_FillMaterial;
        [SerializeField] Material m_BorderMaterial;

        MeshRenderer _meshRenderer;
        LineRenderer _lineRenderer;
        MeshCollider _meshCollider;
        Material _fillInstance;
        Material _borderInstance;
        Mesh _standaloneMesh;
        bool _initialized;

        public DetectedPlane Snapshot { get; private set; }

        public bool HasSnapshot { get; private set; }

        public bool IsCandidate { get; private set; }

        public Material FillTemplate => m_FillMaterial;

        public Material BorderTemplate => m_BorderMaterial;

        void Awake() => EnsureInitialized();

        void OnDestroy()
        {
            PlaneMaterialFactory.Release(_fillInstance);
            PlaneMaterialFactory.Release(_borderInstance);
            PlaneMaterialFactory.Release(_standaloneMesh);
        }

        public void SetMaterialTemplates(Material fill, Material border)
        {
            m_FillMaterial = fill;
            m_BorderMaterial = border;
            RebuildMaterialInstances();
        }

        public void Bind(DetectedPlane snapshot, bool isCandidate)
        {
            Snapshot = snapshot;
            HasSnapshot = true;
            IsCandidate = isCandidate;
        }

        public void BuildStandaloneMesh(IReadOnlyList<Vector2> boundary)
        {
            EnsureInitialized();
            var mesh = PlaneBoundaryMesh.Build(boundary);
            if (mesh != null)
                Assign(mesh, boundary);
        }

        public void ApplyStyle(PlaneVisualStyle style)
        {
            var visible = style != PlaneVisualStyle.Hidden;
            SetRenderers(visible);
            if (visible)
                Paint(style);
        }

        void EnsureInitialized()
        {
            if (_initialized)
                return;

            CacheComponents();
            _initialized = true;
        }

        void CacheComponents()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _lineRenderer = GetComponent<LineRenderer>();
            _meshCollider = GetComponent<MeshCollider>();
            DisableConvex();
            ConfigureLine();
            RebuildMaterialInstances();
        }

        void DisableConvex()
        {
            if (_meshCollider != null)
                _meshCollider.convex = false;
        }

        void ConfigureLine()
        {
            if (_lineRenderer == null)
                return;

            _lineRenderer.loop = true;
            _lineRenderer.useWorldSpace = false;
            DisableLineShadows();
        }

        void DisableLineShadows()
        {
            _lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _lineRenderer.receiveShadows = false;
            _lineRenderer.numCornerVertices = 4;
            _lineRenderer.numCapVertices = 4;
        }

        void RebuildMaterialInstances()
        {
            ReleaseMaterials();
            _fillInstance = PlaneMaterialFactory.Create(m_FillMaterial, PlaneVisualPalette.IdleFill);
            _borderInstance = PlaneMaterialFactory.Create(m_BorderMaterial, PlaneVisualPalette.IdleBorder);
            AssignFill();
            AssignBorder();
        }

        void ReleaseMaterials()
        {
            PlaneMaterialFactory.Release(_fillInstance);
            PlaneMaterialFactory.Release(_borderInstance);
        }

        void AssignFill()
        {
            if (_meshRenderer == null || _fillInstance == null)
                return;

            _meshRenderer.sharedMaterial = _fillInstance;
            _meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _meshRenderer.receiveShadows = false;
        }

        void AssignBorder()
        {
            if (_lineRenderer != null && _borderInstance != null)
                _lineRenderer.sharedMaterial = _borderInstance;
        }

        void Assign(Mesh mesh, IReadOnlyList<Vector2> boundary)
        {
            _standaloneMesh = mesh;
            AssignFilter(mesh);
            AssignCollider(mesh);
            AssignOutline(boundary);
        }

        void AssignFilter(Mesh mesh)
        {
            var filter = GetComponent<MeshFilter>();
            if (filter != null)
                filter.sharedMesh = mesh;
        }

        void AssignCollider(Mesh mesh)
        {
            if (_meshCollider == null)
                return;

            _meshCollider.sharedMesh = null;
            _meshCollider.sharedMesh = mesh;
        }

        void AssignOutline(IReadOnlyList<Vector2> boundary)
        {
            if (_lineRenderer == null)
                return;

            _lineRenderer.positionCount = boundary.Count;
            for (var i = 0; i < boundary.Count; i++)
                _lineRenderer.SetPosition(i, PlaneBoundaryMesh.Outline(boundary[i]));
        }

        void SetRenderers(bool visible)
        {
            if (_meshRenderer != null)
                _meshRenderer.enabled = visible;
            if (_lineRenderer != null)
                _lineRenderer.enabled = visible;
            if (_meshCollider != null)
                _meshCollider.enabled = visible;
        }

        void Paint(PlaneVisualStyle style)
        {
            if (_fillInstance == null)
                return;

            var table = HasSnapshot && Snapshot.Classification == PlaneSemanticClassification.Table;
            PaintFill(PlaneVisualPalette.Fill(style, table));
            PaintBorder(style, table);
        }

        void PaintFill(Color color) => PlaneMaterialFactory.Tint(_fillInstance, color);

        void PaintBorder(PlaneVisualStyle style, bool table)
        {
            var color = PlaneVisualPalette.Border(style, table);
            PlaneMaterialFactory.Tint(_borderInstance, color);
            SetLine(color, PlaneVisualPalette.Width(style, table));
        }

        void SetLine(Color color, float width)
        {
            if (_lineRenderer == null)
                return;

            _lineRenderer.startColor = color;
            _lineRenderer.endColor = color;
            _lineRenderer.widthMultiplier = width;
        }
    }
}
