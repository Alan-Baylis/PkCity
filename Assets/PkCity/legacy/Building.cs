using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {
	public Material
		wallMat,
		roofMat;
	
	Rect dimPos;
	float
		depth,
		height,
		LODThreshold = 128,
		quadColorThreshold = 0.75f,
		ledgeHeight = 0.25f,
		ledgeWidth  = Random.Range(1, 2);
	Color[] quadColors = new Color[] {
		new Color(0.78f,0.78f,0.78f), // Grey
		new Color(1,0.5f,0),         // Orange
		new Color(0,1,1), // Cyan
		new Color(0.5f, 1, 0) // Green
	};
    GameObject quadCube_nobottom, quadCube_nobottomtop;
    GameObject baseCollider, foundation;
	GameObject[] floors, ledges;
	
	public void Initialize(Rect dimPos, float depth, float height) {
		this.dimPos = dimPos;
		this.depth  = depth;
		this.height = height;

        quadCube_nobottom    = Resources.Load<GameObject>("Quadcube_nobottom");
        quadCube_nobottomtop = Resources.Load<GameObject>("Quadcube_nobottomtop");

		baseCollider = makeChild(PrimitiveType.Cube);
		baseCollider.transform.position += new Vector3(0, -depth / 2 + height / 2, 0);
		baseCollider.transform.localScale = new Vector3(dimPos.width, depth + height, dimPos.height);
		baseCollider.GetComponent<Renderer>().enabled = false;
		
		addFoundation();
		addFloor();
		addRoof();
	}
	
    /*
	public void Update() {
		if (highLOD) {
			for (int i = 3; i < transform.childCount; i++) {
				transform.GetChild(i).gameObject.active = true;
			}
		} else {
			for (int i = 3; i < transform.childCount; i++) {
				transform.GetChild(i).gameObject.active = false;
			}
		}
	}
    */
	
	public Bounds boxCollider {
		get { return baseCollider.GetComponent<Renderer>().bounds; }
	}
	
	void addFoundation() {
		foundation = makeChild(quadCube_nobottomtop);
        foundation.transform.position += new Vector3(0, -depth / 2 + height / 2, 0);
        foundation.transform.localScale = new Vector3(dimPos.width, depth + height, dimPos.height);
		
		foreach (Transform childQuad in transform.GetChild(1)) {
			Vector2 quadSize = new Vector2(childQuad.lossyScale.x, childQuad.lossyScale.y);
			childQuad.gameObject.GetComponent<Renderer>().material = tiledMat(wallMat, quadSize);
		}
		
		if (Random.value >= quadColorThreshold) {
			GameObject colorQuad = foundation.transform.GetChild(Random.Range(0,foundation.transform.childCount)).gameObject;
			colorQuad.GetComponent<Renderer>().material.SetColor("_Color", quadColors[Random.Range(0, quadColors.Length)]);
		}
	}
	
	void addFloor() {
		if (true) {
			//addFloor();
		}
	}
	
	void addRoof() {
		GameObject roof = makeChild(PrimitiveType.Plane);
		roof.transform.position += new Vector3(0, height, 0);
		roof.transform.localScale = new Vector3(dimPos.width, 1, dimPos.height) / 10;
		
		roof.GetComponent<Renderer>().material = tiledMat(roofMat, new Vector2(dimPos.width, dimPos.height));
		
		//addRoofLedges();
		addRoofProps();
	}
	
	void addRoofLedges() {		
		ledges = new GameObject[] {
			makeChild(quadCube_nobottom),
			makeChild(quadCube_nobottom),
			makeChild(quadCube_nobottom),
			makeChild(quadCube_nobottom),
		};
			
		ledges[0].transform.localScale = new Vector3(dimPos.width, ledgeHeight, ledgeWidth);
		ledges[0].transform.position  += new Vector3(0, height + ledgeHeight / 2, dimPos.height / 2 - ledgeWidth / 2);
		
		ledges[1].transform.localScale = new Vector3(ledgeWidth, ledgeHeight, dimPos.height - 2 * ledgeWidth);
		ledges[1].transform.position  += new Vector3(dimPos.width / 2 - ledgeWidth / 2, height + ledgeHeight / 2, 0);
		
		ledges[2].transform.localScale = new Vector3(dimPos.width, ledgeHeight, ledgeWidth);
		ledges[2].transform.position  += new Vector3(0, height + ledgeHeight / 2, -dimPos.height / 2 + ledgeWidth / 2);
		
		ledges[3].transform.localScale = new Vector3(ledgeWidth, ledgeHeight, dimPos.height - 2 * ledgeWidth);
		ledges[3].transform.position  += new Vector3(-dimPos.width / 2  + ledgeWidth / 2, height + ledgeHeight /2, 0);
	
		foreach (GameObject child in ledges)
			foreach (Transform childQuad in child.transform) {
				Vector2 quadSize = new Vector2(childQuad.lossyScale.x, childQuad.lossyScale.y);
				childQuad.gameObject.GetComponent<Renderer>().material = tiledMat(roofMat, quadSize);
			}
	}
	
	void addRoofProps() {
	}
	
	bool highLOD {
		get { return LODThreshold * LODThreshold > (Camera.main.transform.position - transform.position).sqrMagnitude; }
	}
	
	GameObject makeChild(PrimitiveType type) {
		Transform child = GameObject.CreatePrimitive(type).transform;
		child.SetParent(transform);
		child.position = transform.position;
		child.rotation = transform.rotation;
		
		return child.gameObject;
	}
	
	GameObject makeChild(GameObject clone) {
		Transform child = ((GameObject) Instantiate(clone, transform.position, transform.rotation)).transform;
		child.SetParent(transform);
		child.position = transform.position;
		child.rotation = transform.rotation;
		
		return child.gameObject;
	}
	
	Material tiledMat(Material mat, Vector2 apply) {
		Material tmat = new Material(mat);
		Vector2 tscale = mat.GetTextureScale("_MainTex");
		tmat.SetTextureScale("_MainTex", new Vector2(apply.x / tscale.x, apply.y / tscale.y));
		return tmat;
	}
}