using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {
    
    public Vector3 direction, degrees = new Vector3(0, 5, 0);
	
	void LateUpdate () {
        transform.Translate(direction * Time.deltaTime);
        transform.Rotate(degrees * Time.deltaTime);
	}
}
