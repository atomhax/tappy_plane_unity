using UnityEngine;
using System.Collections;

public class DestroyObsticles : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D coll){
		if(coll.gameObject.tag == "Obsticle"){
			Destroy (coll.gameObject);
		}
	}
}
