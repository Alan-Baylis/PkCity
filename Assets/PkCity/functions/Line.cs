using System;
using UnityEngine;

namespace Pk.Functions {
    public class Line {
        public Vector2 p1, p2;

        public Line(Vector2 p1, Vector2 p2) {
            this.p1 = p1;
            this.p2 = p2;
        }

        public Vector2 Midpoint() {
            return (p1 + p2) / 2;
        }

        public float length {
            get { return Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) * Mathf.Pow(p2.y - p1.y, 2)); }
        }

        public float slope {
            get { return (p2.y - p1.y) / (p2.x - p1.x); }
        }

        public static Vector2 Intersect(Vector2 p1, float m1, Vector2 p2, float m2) {
            return Intersect(m1, -1, (m1 * p1.x) - p1.y, m2, -1, (m2 * p2.x) - p2.y);
        }

        public static Vector2 Intersect(float a1, float b1, float c1, float a2, float b2, float c2) {
            float d = a1 * b2 - a2 * b1;
            if (d == 0)
                throw new ArgumentException("Lines are parallel");
            return new Vector2(b2 * c1, a1 * c2) / d;
        }

        // From: https://rosettacode.org/wiki/Ray-casting_algorithm
        // TODO fix
        public bool RayIntersects(Vector2 point) {
            const float infinity = float.MaxValue;
            Vector2 min, max;
            if (p1.y < p2.y) {
                min = p1;
                max = p2;
            } else {
                min = p2;
                max = p1;
            }
            if (point.y == min.y || point.y == max.y)
                point.y += 0.1f;

            if (point.y < min.y || point.y > max.y)
                return false;
            else {
                if (point.x > Mathf.Max(min.x, max.x)) {
                    return false;
                } else {
                    if (point.x < Mathf.Min(min.x, max.x))
                        return true;
                    else {
                        float red, blue;
                        if (min.x != max.x)
                            red = (max.y - min.y) / (max.x - min.x);
                        else
                            red = infinity;
                        if (point.x != max.x)
                            blue = (point.y - min.y) / (point.x - min.x);
                        else
                            blue = infinity;
                        if (blue >= red)
                            return true;
                        else
                            return false;
                    }
                }
            }
        }
    }
}
