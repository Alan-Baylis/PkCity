using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Pk.Functions {
    // Dual iterative Delauney-Voronoi graph
    // Only tolerates new points from the exterior! //TODO
    public class DualGraph {
        List<Vector2> points;
        Delaunay.Voronoi voronoi;

        public DualGraph(List<Vector2> points) {
            this.points = points;
            voronoi = new Delaunay.Voronoi(points, new uint[points.Count].ToList(), BoundingRect());
        }

        public Polygon GetPolygon(Vector2 point) {
            if (points.Contains(point)) {
                var region = new Polygon(voronoi.Region(point).ToArray());
                return TouchesRect(region) ? null : region;
            } else
                return null;
        }

        Rect BoundingRect() {
            if (points.Count != 0) {
                float
                    minX = points[0].x,
                    minY = points[0].y,
                    maxX = points[0].x,
                    maxY = points[0].y;

                foreach (Vector2 point in points) {
                    if (point.x < minX)
                        minX = point.x;
                    if (point.y < minY)
                        minY = point.y;
                    if (point.x > maxX)
                        maxX = point.x;
                    if (point.y > maxY)
                        maxY = point.y;
                }
                return new Rect(
                    minX - 1,
                    minY - 1,
                    (maxX - minX) + 1,
                    (maxY - minY) + 1);
            } else
                return new Rect();
        }

        // Only works for interior polygons
        bool OnRect(Polygon poly) {
            var verts = poly.Vertices2D();
            for (int i = 0; i < poly.n - 1; i++)
                if (OnRect(verts[i], verts[i + 1]))
                    return true;
            return false;
        }

        // Only works for interior polygons
        bool TouchesRect(Polygon poly) {
            var verts = poly.Vertices2D();
            for (int i = 0; i < poly.n - 1; i++)
                if (OnRect(verts[i]) || OnRect(verts[i + 1]))
                    return true;
            return false;
        }

        // Only works for interior segments
        bool OnRect(Vector2 point1, Vector2 point2) {
            return OnRect(point1) && OnRect(point1);
        }

        // Only works for interior points
        bool OnRect(Vector2 point) {
            return point.x == voronoi.plotBounds.xMin || point.x == voronoi.plotBounds.xMax || point.y == voronoi.plotBounds.yMin || point.y == voronoi.plotBounds.yMax;
        }
    }
}
