using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;

public class GameOverPanelController : MonoBehaviour {

	public Text gameoverScoreText;

	public GameController gameController;

	void OnEnable(){
		DisplayGameoverScore ();
	}
	
	void DisplayGameoverScore(){
		gameoverScoreText.text = "STARS THIS ROUND: " + gameController.playerScore.ToString ();
		gameoverScoreText.text += "\nHIGHSCORE: " + PlayerPrefs.GetInt ("HighScore", 0).ToString ();

		Spil.SpilPlayerDataInstance.Wallet.Add (25, gameController.playerScore,PlayerDataUpdateReasons.LevelComplete);

		gameoverScoreText.text += "\nTOTAL STARS: " + Spil.SpilPlayerDataInstance.GetCurrencyBalance (25).ToString ();
	}

}
