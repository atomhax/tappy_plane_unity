using UnityEngine;
using System.Collections;
using SpilGames.Unity;
using System.Collections.Generic;
using SpilGames.Unity.Helpers.PlayerData;

public class PlayerController : MonoBehaviour {

	public Rigidbody2D playerBody;

	public GameController gameController;

	public RuntimeAnimatorController[] playerSkinAnimators;

	public float jumpForce;

	public float maxYVelocity;

    public bool dead = false;

	public bool idleMode = true;

	public int tapperCount = 0;

	void Start(){
		SetupPlayerSkin ();
	}

	// Update is called once per frame
	void Update () {
		if(idleMode){
			IdleMode ();
		}
		CheckForSpeed ();
	}

	public void Jump(){
		if (dead && !idleMode) {
			return;
		}
		if (!dead && !idleMode) {
			tapperCount++;
			if (tapperCount % 10 == 0) {
				gameController.AddToTapper();
			}
		}
		playerBody.AddForce (jumpForce * Vector2.up, ForceMode2D.Impulse);
	}

	void CheckForSpeed(){
		if(playerBody.velocity.y > maxYVelocity){
			playerBody.velocity = new Vector2 (0, maxYVelocity);
		}
	}

	void OnTriggerEnter2D(Collider2D coll){
		if(!dead && coll.gameObject.tag == "Obsticle"){
			Die ();
		}
		if(!dead && coll.gameObject.tag == "ScoreChecker"){
			gameController.AddToScore ();
			coll.gameObject.SetActive (false);
		}
	}

	void Die(){
		dead = true;
		playerBody.AddForce (10 * new Vector2 (-0.5f, 1), ForceMode2D.Impulse);
		playerBody.AddTorque (Random.Range (-200f, 200f));
		Invoke ("GameOver", 2);
	}

	void GameOver(){
		gameController.GameOver ();
	}
		
	void IdleMode(){
		if(transform.position.y < 0){
			Jump ();
		}
	}
		
	public void SetupPlayerSkin() {
		RuntimeAnimatorController animatorController = playerSkinAnimators[PlayerPrefs.GetInt("Skin", 0)];

		if (animatorController != null) {
			GetComponent<Animator>().runtimeAnimatorController = animatorController;
		} else {
			GetComponent<Animator>().runtimeAnimatorController = playerSkinAnimators[0];
		}
	}
}
