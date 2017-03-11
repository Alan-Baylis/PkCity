using UnityEngine;
using System.Collections.Generic;

namespace Pk.Functions {
    public abstract class Point {
        public static Vector2 Rotate(Vector2 point, float radians, Vector2 axis) {
            return new Vector2(
                Mathf.Cos(radians) * (point.x - axis.x) - Mathf.Sin(radians) * (point.y - axis.y) + axis.x,
                Mathf.Sin(radians) * (point.x - axis.x) + Mathf.Cos(radians) * (point.y - axis.y) + axis.y
                );
        }

        public static List<Vector2> Around(Vector2 pos, float radius, float seed, float resolution = 1, float threshold = 0.25f) {
            var points = new List<Vector2>();

            for (float i = Util.Ceil(pos.x - radius, resolution); i <= Util.Floor(pos.x + radius, resolution); i += resolution)
                for (float j = Util.Ceil(pos.y - radius, resolution); j <= Util.Floor(pos.y + radius, resolution); j += resolution) {
                    var test = new Vector2(i, j);
                    // Same point will always return the same value.
                    Random.InitState((test * seed).ToString().GetHashCode());
                    if (Vector2.Distance(pos, test) <= radius && Random.value <= threshold)
                        points.Add(test);
                }

            return points;
        }
    }
}
