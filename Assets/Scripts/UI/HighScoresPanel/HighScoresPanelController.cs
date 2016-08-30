using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;
using UnityEngine.SceneManagement;

public class HighScoresPanelController : MonoBehaviour {

	const string glyphs= "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

	public Text highScoreText;

	public GameController gameController;

	void OnEnable(){
		highScoreText.text = "No High Scores Recorded";
		Spil.Instance.OnOtherUsersGameStateDataLoaded += Spil_Instance_OnOtherUsersGameStateDataLoaded;
		RequestHighScores();
	}

	void OnDisable(){
		Spil.Instance.OnOtherUsersGameStateDataLoaded -= Spil_Instance_OnOtherUsersGameStateDataLoaded;
	}

	void RequestHighScores(){
		string facebookIds = Spil.Instance.GetConfigValue("facebookIds");
		Spil.Instance.GetOtherUsersGameState("Facebook", facebookIds);
	}

	void DisplayHighScores(SpilGames.Unity.Utils.OtherUsersGameStateData data){

		int charAmount = Random.Range(3, 3);
		highScoreText.text = "";

		foreach(KeyValuePair<string, string> p in data.data){
			PublicGameState gameState = JsonHelper.getObjectFromJson<PublicGameState>(p.Value);

			string user = "";
			for(int i=0; i<charAmount; i++)
			{
			     user += glyphs[Random.Range(0, glyphs.Length)];
			}

			highScoreText.text = highScoreText.text + "USER: " + user + " ----- SCORE: " + gameState.HighScore + " \n";
		}

	}

	void Spil_Instance_OnOtherUsersGameStateDataLoaded (SpilGames.Unity.Utils.OtherUsersGameStateData data){
		DisplayHighScores(data);
	}

}
