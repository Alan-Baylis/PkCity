using UnityEngine;
using System.Linq;
using ClipperLib;
using System.Collections.Generic;

namespace Pk.Functions {
    public class Polygon {
        Vector2[] vertices;

        public Polygon(Vector2[] vertices) {
            this.vertices = vertices;
        }

        public Vector2[] Vertices2D() { return vertices; }

        public Vector3[] Vertices3D() { return vertices.ToList().ConvertAll(o => new Vector3(o.x, 0, o.y)).ToArray(); }

        public Line[] sides {
            get {
                var sides = new Line[vertices.Length - 1];
                for (int i = 0; i < vertices.Length - 1; i++)
                    sides[i] = new Line(vertices[i], vertices[i + 1]);
                return sides;
            }
        }

        public int[] Triangles(bool fixAntiNormal = true) {
            var tris = new Triangulator(vertices).Triangulate();
            return fixAntiNormal && AntiNormal(vertices, tris) ? tris.Reverse().ToArray() : tris;
        }

        public void Translate(Vector2 shift) {
            for (int i = 0; i < vertices.Count(); i++)
                vertices[i] += shift;
        }

        public void Grow(float grow, float miterLimit = 0) {
            Offset(grow, JoinType.jtMiter, miterLimit);
        }

        public void Round(float radius, float arcTolerance = 0.1f) {
            Grow(-radius);
            Offset(radius, JoinType.jtRound, arcTolerance);
        }

        public void Square(float radius) {
            Grow(-radius);
            Offset(radius, JoinType.jtSquare);
        }

        private void Offset(float grow, JoinType type, float tolerance = 0) {
            const int accuracy = 10000; // Because ClipperLib only deals in ints!
            tolerance *= accuracy;
            List<List<IntPoint>> solution = new List<List<IntPoint>>();
            var offset = new ClipperOffset();
            offset.ArcTolerance = tolerance;
            offset.MiterLimit = tolerance;
            offset.AddPath(vertices.Select(o => { return new IntPoint(Mathf.Round(o.x * accuracy), Mathf.Round(o.y * accuracy)); }).ToList(), type, EndType.etClosedPolygon);
            offset.Execute(ref solution, grow * accuracy);
            if (solution.Count() > 0)
                vertices = solution[0].Select(o => { return new Vector2(o.X, o.Y) / accuracy; }).ToArray();
        }

        public void Rotate(float radians) {
            Rotate(radians, center);
        }

        public void Rotate(float radians, Vector2 axis) {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i] = Point.Rotate(vertices[i], radians, axis);
        }

        public void Scale(float scale) {
            Scale(new Vector2(scale, scale));
        }

        public void Scale(Vector2 scale) {
            for (int i = 0; i < vertices.Count(); i++) {
                var vert = vertices[i];
                vert.x = center.x + ((vert.x - center.x) * scale.x);
                vert.y = center.y + ((vert.y - center.y) * scale.y);
            }
        }

        public Vector2 center {
            get {
                float x = 0, y = 0;
                foreach (var vertex in vertices) {
                    x += vertex.x;
                    y += vertex.y;
                }
                return new Vector2(x, y) / n;
            }

            set {
                Vector2 oldCenter = center;
                for (int i = 0; i < n; i++) {
                    vertices[i] += value - oldCenter;
                }
            }
        }

        public int n { get { return vertices.Length; } }

        // Returns true for 0 area
        public bool IsRectangle() {
            if (vertices.Length == 4) {
                Line
                    a = new Line(vertices[0], vertices[1]),
                    b = new Line(vertices[1], vertices[2]),
                    c = new Line(vertices[2], vertices[3]),
                    d = new Line(vertices[3], vertices[0]);
                if (a.p1 == c.p2)
                    return false;
                if (a.slope == -1f / b.slope && a.slope == c.slope && b.slope == d.slope)
                    return true;
            }
            return false;
        }
        
        public Rect FastBoundingRect() {
            if (IsRectangle())
                return new Rect(0, 0, new Line(vertices[0], vertices[1]).length, new Line(vertices[1], vertices[2]).length);
            else {
                float
                    minX = vertices[0].x,
                    maxX = minX,
                    minY = vertices[0].y,
                    maxY = minY;
                foreach (var vert in vertices.Skip(1)) {
                    if (vert.x < minX)
                        minX = vert.x;
                    if (vert.x > maxX)
                        maxX = vert.x;
                    if (vert.y < minY)
                        minY = vert.y;
                    if (vert.y > maxY)
                        maxY = vert.y;
                }
                return new Rect(minX, minY, maxX - minX, maxY - minY);
            }
        }
        
        public float AngleOfLongestSide() {
            Line longestSide = sides[0];
            foreach (var side in sides.Skip(1))
                if (side.length > longestSide.length)
                    longestSide = side;
            return Mathf.Atan(longestSide.slope);
        }

        public bool Inside(Vector2 point) {
            return polyCheck(point, vertices.Reverse().ToArray());

            /*
            int count = 0;
            foreach (var side in sides) {
                if (side.RayIntersects(point))
                    count++;
            }
            return count % 2 == 1;
            */
        }

        // From: http://codereview.stackexchange.com/questions/108857/point-inside-polygon-check
        // Quick and dirty hack
        private static bool polyCheck(Vector2 v, Vector2[] p) {
            int j = p.Length - 1;
            bool c = false;
            for (int i = 0; i < p.Length; j = i++) c ^= p[i].y > v.y ^ p[j].y > v.y && v.x < (p[j].x - p[i].x) * (v.y - p[i].y) / (p[j].y - p[i].y) + p[i].x;
            return c;
        }

        public static Polygon FromRect(Rect rect) {
            var vertices = new Vector2[] {
                rect.position,
                rect.position + new Vector2(0, rect.height),
                rect.position + new Vector2(rect.width, rect.height),
                rect.position + new Vector2(rect.width, 0),
            };

            return new Polygon(vertices);
        }

        public static bool AntiNormal(Vector2[] verts, int[] tris) {
            if (tris.Count() >= 2) {
                Vector3
                    a = new Vector3(verts[tris[0]].x, 0, verts[tris[0]].y),
                    b = new Vector3(verts[tris[1]].x, 0, verts[tris[1]].y),
                    c = new Vector3(verts[tris[2]].x, 0, verts[tris[2]].y),
                    u = b - a,
                    v = c - a;
                return Vector3.Cross(u, v).y < 0;
            }
            return false;
        }
    }
}
