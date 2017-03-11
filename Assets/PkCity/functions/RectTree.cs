using UnityEngine;
using System.Collections.Generic;

namespace Pk.Functions {
    public class RectTree {
        Rect?[] subRects;
        float minWidth, minHeight, maxWidth, maxHeight;

        public RectTree(float width, float height, float minWidth, float minHeight, float maxWidth, float maxHeight, int seed) {
            this.minWidth  = minWidth;
            this.minHeight = minHeight;
            this.maxWidth  = maxWidth;
            this.maxHeight = maxHeight;

            Random.InitState(seed);
            subRects = new Rect?[(int) Mathf.Ceil((width * height) / (minWidth * minHeight))];

            addRects(-width / 2, width / 2, -height / 2, height / 2);
        }
	
        public List<Rect> rects {
		    get {
			    List<Rect> list = new List<Rect>();
			    foreach (Rect? rect in subRects) {
				    if (rect.HasValue) {
					    list.Add((Rect) rect);
				    }
			    }
			    return list;
		    }
        }

        public int length {
		    get { return subRects.Length; }
	    }

        private void add(Rect rect) {
            for (int i = 0; i < subRects.Length; i++)
            {
                if (!subRects[i].HasValue) {
                    subRects[i] = rect;
                    break;
                }
            }
        }

        private void addRects(float x1, float x2, float y1, float y2, bool vertical = true) {
            if ((x2 - x1) >= minWidth && (y2 - y1) >= minHeight
                && (Random.value <= 0.5
                    || (x2 - x1) > maxWidth
                    || (y2 - y1) > maxHeight)) {
                float
                    u1 = vertical ? x1 : y1,
                    u2 = vertical ? x2 : y2,
                    min = vertical ? minWidth : minHeight;

                if (true) {
                    float subdivide = Random.Range(u1, u2);

                    if (vertical) {
                        if (subdivide - x1 >= min) addRects(x1, subdivide, y1, y2, !vertical);
                        if (x2 - subdivide >= min) addRects(subdivide, x2, y1, y2, !vertical);
                    } else {
                        if (subdivide - y1 >= min) addRects(x1, x2, y1, subdivide, !vertical);
                        if (y2 - subdivide >= min) addRects(x1, x2, subdivide, y2, !vertical);
                    }
                }
            } else {
                add(new Rect(x1, y1, x2 - x1, y2 - y1));
            }
        }
    }
}