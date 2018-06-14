using UnityEngine;
using System.Collections;

public class MoveLeft : MonoBehaviour {

	public float obsticleMoveSpeed;

	
	// Update is called once per frame
	void Update () {
		float scrollSpeed = PlayerPrefs.GetFloat("Speed", 1f);
		transform.Translate ((obsticleMoveSpeed * Vector2.left) * Time.deltaTime * scrollSpeed);

	}
}
