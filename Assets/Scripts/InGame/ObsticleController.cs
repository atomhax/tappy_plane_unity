using UnityEngine;
using System.Collections;

public class ObsticleController : MonoBehaviour {



	public float obsticleMoveSpeed;




	// Update is called once per frame
	void FixedUpdate () {
	
		transform.Translate ((obsticleMoveSpeed * Vector2.left) * Time.deltaTime);

	}
}
