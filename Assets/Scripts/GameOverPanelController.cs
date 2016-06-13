﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class GameOverPanelController : MonoBehaviour {

	public Text gameoverScoreText;

	public GameController gameController;

	void OnEnable(){
		DisplayGameoverScore ();
	}
	
	void DisplayGameoverScore(){
		gameoverScoreText.text = "SCORE: " + gameController.playerScore.ToString ();
		gameoverScoreText.text += "\nHIGHSCORE: " + PlayerPrefs.GetInt ("HighScore", 0).ToString ();
	}


}
