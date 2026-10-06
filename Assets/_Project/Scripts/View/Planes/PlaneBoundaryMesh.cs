using System.Collections.Generic;
using UnityEngine;

namespace Fairground.View
{
    public static class PlaneBoundaryMesh
    {
        public static Mesh Build(IReadOnlyList<Vector2> boundary)
        {
            if (!CanBuild(boundary))
                return null;

            var mesh = new Mesh { name = "Plane Fill" };
            Fill(mesh, boundary);
            return mesh;
        }

        public static Vector3 Outline(Vector2 point) => new Vector3(point.x, 0.005f, point.y);

        static bool CanBuild(IReadOnlyList<Vector2> boundary) => boundary != null && boundary.Count >= 3;

        static void Fill(Mesh mesh, IReadOnlyList<Vector2> boundary)
        {
            mesh.vertices = Vertices(boundary);
            mesh.triangles = Triangles(boundary.Count);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }

        static Vector3[] Vertices(IReadOnlyList<Vector2> boundary)
        {
            var vertices = new Vector3[boundary.Count];
            for (var i = 0; i < boundary.Count; i++)
                vertices[i] = new Vector3(boundary[i].x, 0f, boundary[i].y);
            return vertices;
        }

        static int[] Triangles(int count)
        {
            var triangles = new int[(count - 2) * 3];
            var index = 0;
            for (var i = 1; i < count - 1; i++)
                index = WriteTriangle(triangles, index, i);
            return triangles;
        }

        static int WriteTriangle(int[] triangles, int index, int vertex)
        {
            triangles[index++] = 0;
            triangles[index++] = vertex;
            triangles[index++] = vertex + 1;
            return index;
        }
    }
}
