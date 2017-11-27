﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class InGamePanelController : MonoBehaviour {

	public Text currentScoreText;
	public Text currentTapperText;

	public GameController gameController;
	
	// Update is called once per frame
	void Update () {
		DisplayScore ();
	}

	void DisplayScore(){
		currentScoreText.text = gameController.playerScore.ToString ();
		currentTapperText.text = gameController.tapperScore.ToString();
	}

}
