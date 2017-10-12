using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;
using UnityEngine.SceneManagement;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Json;

public class HighScoresPanelController : MonoBehaviour {

	public Text highScoreText;

	public GameController gameController;

	void OnEnable(){
		highScoreText.text = "No High Scores Recorded";
		Spil.Instance.OnOtherUsersGameStateDataLoaded += OnOtherUsersGameStateDataLoaded;
		RequestHighScores();
	}

	void OnDisable(){
		Spil.Instance.OnOtherUsersGameStateDataLoaded -= OnOtherUsersGameStateDataLoaded;
	}

	public void RequestHighScores(){
		string facebookIds = GameController.GetFriendIdsJson();
		Spil.Instance.GetOtherUsersGameState("Facebook", facebookIds);
	}

	void DisplayHighScores(OtherUsersGameStateData data){
		highScoreText.text = "";

		int pos = 1;
		foreach(KeyValuePair<string, string> p in data.data){
			PublicGameState gameState = JsonHelper.getObjectFromJson<PublicGameState>(p.Value);
			if(gameState != null){
				highScoreText.text += pos.ToString() + ". " + GameController.GetNameForFbId(p.Key) + " [" + gameState.HighScore + "]\n";
				pos++;
			}
		}
	}

	void OnOtherUsersGameStateDataLoaded (OtherUsersGameStateData data){
		DisplayHighScores(data);
	}

}
