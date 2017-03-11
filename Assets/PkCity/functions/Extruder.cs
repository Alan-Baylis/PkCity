using UnityEngine;
using System.Linq;

namespace Pk.Functions {
    public abstract class Extruder {
        public static Mesh Extrude(Polygon poly, float height, bool endcap = false) {
            var mesh = new Mesh();

            var originalVerts = poly.Vertices3D();
            var verts = originalVerts.Select(o => { o.y += height; return o; }).ToArray().Concat(originalVerts.Reverse()).ToArray();

            var polyTris = poly.Triangles();
            var tris = !endcap ? polyTris : polyTris.Concat(polyTris.Select(o => { o += poly.n; return o; })).ToArray();

            for (int i = 0; i < poly.n; i++) {
                int
                    j = (i + 1) % poly.n,
                    k = (2 * poly.n) - 1 - i,
                    l = k - 1;
                if (i < poly.n - 1)
                    tris = tris.Concat(new int[] { k, j, i, j, k, l }).ToArray();
                else
                    tris = tris.Concat(new int[] { k, j, i }).ToArray();
            }
            tris = tris.Concat(new int[] { (2 * poly.n) - 1, 0, poly.n }).ToArray();

            mesh.vertices = verts;
            mesh.triangles = tris;

            return mesh;
        }

        public static Mesh ExtrudeCCW(Polygon poly, float height, bool endcap = false) {
            var mesh = new Mesh();

            var originalVerts = poly.Vertices3D();
            var verts = originalVerts.Select(o => { o.y = height; return o; }).ToArray().Concat(originalVerts.Reverse()).ToArray();

            var polyTris = poly.Triangles();
            var tris = !endcap ? polyTris : polyTris.Concat(polyTris.Select(o => { o += poly.n; return o; })).ToArray();

            for (int i = 0; i < poly.n; i++) {
                int
                    j = (i + 1) % poly.n,
                    k = (2 * poly.n) - 1 - i,
                    l = k - 1;
                if (i < poly.n - 1)
                    tris = tris.Concat(new int[] { l, k, j, i, j, k }).ToArray();
                else
                    tris = tris.Concat(new int[] { i, j, k }).ToArray();
            }
            tris = tris.Concat(new int[] { poly.n, 0, (2 * poly.n) - 1 }).ToArray();

            mesh.vertices = verts;
            mesh.triangles = tris;

            mesh.Optimize();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            return mesh;
        }
    }
}
