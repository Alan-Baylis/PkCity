using System;
using UnityEngine;

namespace Pk {
    public abstract class Util {
        public static float Floor(float n, float increment = 1) {
            return Mathf.Floor(n / increment) * increment;
        }

        public static float Ceil(float n, float increment = 1) {
            return Mathf.Ceil(n / increment) * increment;
        }

        public static Vector3 V23(Vector2 v2) {
            return new Vector3(v2.x, 0, v2.y);
        }

        public static Vector2 V32(Vector3 v3) {
            return new Vector2(v3.x, v3.z);
        }

        public static Vector2[] RectVerts(Rect rect) {
            return new Vector2[] {
                new Vector2(rect.xMin, rect.yMin),
                new Vector2(rect.xMin, rect.yMax),
                new Vector2(rect.xMax, rect.yMin),
                new Vector2(rect.xMax, rect.yMax),
            };
        }
    }

    public struct IntRect {
        public int x, y, width, height;

        public IntRect(int x, int y, int width, int height) {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
    }
}
