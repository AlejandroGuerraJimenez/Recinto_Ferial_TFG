using System.Collections.Generic;
using Fairground.Core.Planes;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Fairground.View
{
    public enum PlaneVisualStyle
    {
        Hidden = 0,
        Idle = 1,
        Dimmed = 2,
        Highlighted = 3,
        Selected = 4,
        Proposed = 5,
    }

    /// <summary>
    /// Semi-transparent fill plus a boundary line for one detected surface.
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(100)]
    public sealed class PlaneSurfaceVisual : MonoBehaviour
    {
        [SerializeField] Material m_FillMaterial;
        [SerializeField] Material m_BorderMaterial;

        MeshRenderer _meshRenderer;
        MeshFilter _meshFilter;
        LineRenderer _lineRenderer;
        MeshCollider _meshCollider;
        Material _fillInstance;
        Material _borderInstance;
        Mesh _standaloneMesh;
        bool _initialized;
        ARPlane _arPlane;
        PlaneVisualStyle _appliedStyle = PlaneVisualStyle.Hidden;

        public DetectedPlane Snapshot { get; private set; }

        public bool HasSnapshot { get; private set; }

        public bool IsCandidate { get; private set; }

        public PlaneRank Rank { get; private set; }

        public Material FillTemplate => m_FillMaterial;

        public Material BorderTemplate => m_BorderMaterial;

        public ARPlane Trackable => _arPlane;

        void Awake() => EnsureInitialized();

        void OnEnable()
        {
            EnsureInitialized();
            if (_arPlane != null)
                _arPlane.boundaryChanged += Lift;
        }

        void OnDisable()
        {
            if (_arPlane != null)
                _arPlane.boundaryChanged -= Lift;
        }

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

        public void ApplySnapshot(DetectedPlane snapshot, bool isCandidate, PlaneRank rank)
        {
            var changed = !HasSnapshot || IsCandidate != isCandidate || !Same(Snapshot, snapshot);
            Snapshot = snapshot;
            HasSnapshot = true;
            IsCandidate = isCandidate;
            Rank = rank;
            if (changed)
                _appliedStyle = (PlaneVisualStyle)(-1);
        }

        public void Bind(DetectedPlane snapshot, bool isCandidate)
        {
            ApplySnapshot(snapshot, isCandidate, default);
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
            if (visible)
                EnsureQuad();
            SetRenderers(visible);
            if (_appliedStyle == style)
                return;

            _appliedStyle = style;
            if (visible)
                Paint(style);
        }

        void EnsureQuad()
        {
            if (HasDeviceMesh() || !HasSnapshot || Snapshot.Boundary == null || Snapshot.Boundary.Count != 4)
                return;

            var boundary = new Vector2[4];
            for (var i = 0; i < 4; i++)
                boundary[i] = new Vector2(Snapshot.Boundary[i].X, Snapshot.Boundary[i].Y);
            BuildStandaloneMesh(boundary);
        }

        bool HasDeviceMesh()
        {
            return _meshFilter != null && _meshFilter.sharedMesh != null && _meshFilter.sharedMesh.vertexCount >= 3;
        }

        void Lift(ARPlaneBoundaryChangedEventArgs _)
        {
            LiftMesh();
            LiftLine();
        }

        void LiftMesh()
        {
            var mesh = _meshFilter != null ? _meshFilter.sharedMesh : null;
            if (mesh == null || !mesh.isReadable || mesh.vertexCount < 3)
                return;

            var vertices = mesh.vertices;
            var changed = false;
            for (var i = 0; i < vertices.Length; i++)
            {
                if (vertices[i].y >= PlaneBoundaryMesh.Lift - 0.001f)
                    continue;

                vertices[i].y = PlaneBoundaryMesh.Lift;
                changed = true;
            }

            if (!changed)
                return;

            mesh.vertices = vertices;
            mesh.RecalculateBounds();
        }

        void LiftLine()
        {
            if (_lineRenderer == null)
                return;

            for (var i = 0; i < _lineRenderer.positionCount; i++)
            {
                var point = _lineRenderer.GetPosition(i);
                point.y = PlaneBoundaryMesh.Lift;
                _lineRenderer.SetPosition(i, point);
            }
        }

        static bool Same(DetectedPlane current, DetectedPlane next)
        {
            return current.Id == next.Id
                && current.Classification == next.Classification
                && current.Alignment == next.Alignment
                && SamePose(current.Pose, next.Pose)
                && SameBoundary(current.Boundary, next.Boundary);
        }

        static bool SamePose(PlanePose current, PlanePose next)
        {
            var dx = current.PositionX - next.PositionX;
            var dy = current.PositionY - next.PositionY;
            var dz = current.PositionZ - next.PositionZ;
            if (dx * dx + dy * dy + dz * dz > 0.000001f)
                return false;

            var dot = current.RotationX * next.RotationX
                + current.RotationY * next.RotationY
                + current.RotationZ * next.RotationZ
                + current.RotationW * next.RotationW;
            return dot > 0.9999f || dot < -0.9999f;
        }

        static bool SameBoundary(IReadOnlyList<PlanePoint> current, IReadOnlyList<PlanePoint> next)
        {
            if (current == null || next == null || current.Count != next.Count)
                return false;

            for (var i = 0; i < current.Count; i++)
            {
                if (current[i].X != next[i].X || current[i].Y != next[i].Y)
                    return false;
            }

            return true;
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
            _arPlane = GetComponent<ARPlane>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshFilter = GetComponent<MeshFilter>();
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
            if (_meshFilter != null)
                _meshFilter.sharedMesh = mesh;
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
