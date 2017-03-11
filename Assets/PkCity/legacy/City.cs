using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class City : MonoBehaviour {
    public float
		radius = 1000,
        depth  = 128;
    public bool randomSeed = true, circular = true;
	public int seed;
	
	RectTree rtree;
	List<Rect> rects;
	ArrayList buildings;

    void Start() {
        if (randomSeed) seed = Random.Range(int.MinValue, int.MaxValue);

        float longSide = circular ? 2 * radius : radius;

		rtree = new RectTree(radius, longSide, 8, 8, 32, 32, seed);
        rects = rtree.Rects;
		buildings = new ArrayList(rects.Count);
		
		spawnBuildings();
        foreach (GameObject bldg in buildings)
            SetLayerDeep(bldg, LayerMask.LayerToName(gameObject.layer));

        if (circular) {
            arcBuildings();
            removeIntersecting();
        }

        StaticBatchingUtility.Combine(gameObject);
    }
	
	void spawnBuildings() {
		for (int i = 0; i < rects.Count; i++) {
			Rect dimPos = rects[i];
			float height = Mathf.Pow(5 * Mathf.PerlinNoise(dimPos.center.x / 10, dimPos.center.y / 10), 3);
			
			GameObject bldg = ((GameObject) Instantiate(Resources.Load<GameObject>("Building"), new Vector3(dimPos.center.x, 0, dimPos.center.y), new Quaternion(0, 0, 0, 0)));
			bldg.transform.SetParent(transform);
			bldg.GetComponent<Building>().Initialize(dimPos, depth, height);
			
			buildings.Add(bldg);
        }
	}
	
	void arcBuildings() {
		Vector3 axis = transform.position + new Vector3(radius,0,0);
		foreach (GameObject bldg in buildings) {
			bldg.transform.RotateAround(axis, Vector3.up, 360 * bldg.transform.position.z / radius);
			bldg.transform.LookAt(axis);
			bldg.transform.position -= axis;
		}
	}
	
	void removeIntersecting() {
		for (int i = 0; i < buildings.Count; i++) {
			GameObject bldg = (GameObject) buildings[i];
			
			Bounds collider = bldg.GetComponent<Building>().boxCollider;
			for (int j = 0; j < buildings.Count; j++) {
				if (j != i) {
					Bounds collider2 = ((GameObject) buildings[j]).GetComponent<Building>().boxCollider;
					if (collider.Intersects(collider2)) {
						buildings.RemoveAt(i);
						Object.Destroy(bldg);
						i--;
						break;
					}
				}
			}
		}
	}

    // Sets layer of Game Object and all children
    static void SetLayerDeep(GameObject obj, string name)
    {
        obj.layer = LayerMask.NameToLayer(name);
        foreach (Transform child in obj.GetComponentsInChildren<Transform>())
            child.gameObject.layer = LayerMask.NameToLayer(name);
    }
}

// TODO implement getEnumerator(), use binary tree structure
public class RectTree {
    Rect?[] rects;
    float minWidth, minHeight, maxWidth, maxHeight;

    public RectTree(float width, float height, float minWidth, float minHeight, float maxWidth, float maxHeight, int seed) {
        this.minWidth  = minWidth;
        this.minHeight = minHeight;
        this.maxWidth  = maxWidth;
        this.maxHeight = maxHeight;

        Random.seed = seed;
        rects = new Rect?[(int)Mathf.Ceil((width * height) / (minWidth * minHeight))];

        addRects(-width / 2, width / 2, -height / 2, height / 2);
    }
	
    public List<Rect> Rects {
		get {
			List<Rect> list = new List<Rect>();
			foreach (Rect? rect in rects) {
				if (rect.HasValue) {
					list.Add((Rect) rect);
				}
			}
			return list;
		}
    }

    private void add(Rect rect) {
        for (int i = 0; i < rects.Length; i++)
        {
            if (!rects[i].HasValue) {
                rects[i] = rect;
                break;
            }
        }
    }
	
	public int Length {
		get { return rects.Length; }
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