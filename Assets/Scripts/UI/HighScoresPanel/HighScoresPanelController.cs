﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;
using UnityEngine.SceneManagement;

public class HighScoresPanelController : MonoBehaviour {

	public Text highScoreText;

	public GameController gameController;

	void OnEnable(){
		highScoreText.text = "No High Scores Recorded";
		Spil.Instance.OnOtherUsersGameStateDataLoaded += Spil_Instance_OnOtherUsersGameStateDataLoaded;
		RequestHighScores();
		Spil.Instance.RequestSplashScreen();
	}

	void OnDisable(){
		Spil.Instance.OnOtherUsersGameStateDataLoaded -= Spil_Instance_OnOtherUsersGameStateDataLoaded;
	}

	void RequestHighScores(){
		string facebookIds = GameController.GetFriendIdsJson();
		Spil.Instance.GetOtherUsersGameState("Facebook", facebookIds);
	}

	void DisplayHighScores(SpilGames.Unity.Utils.OtherUsersGameStateData data){
		highScoreText.text = "";

		int pos = 1;
		foreach(KeyValuePair<string, string> p in data.data){
			PublicGameState gameState = JsonHelper.getObjectFromJson<PublicGameState>(p.Value);
			highScoreText.text += pos.ToString() + ". " + GameController.GetNameForFbId(p.Key) + " [" + gameState.HighScore + "]\n";
			pos++;
		}

	}

	void Spil_Instance_OnOtherUsersGameStateDataLoaded (SpilGames.Unity.Utils.OtherUsersGameStateData data){
		DisplayHighScores(data);
	}

}
