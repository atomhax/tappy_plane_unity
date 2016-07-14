using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;
using UnityEngine.SceneManagement;
public class GameOverPanelController : MonoBehaviour {

	public Text gameoverScoreText;

	public GameController gameController;

	void OnEnable(){
		DisplayGameoverScore ();
	}
	
	void DisplayGameoverScore(){
		gameoverScoreText.text = "STARS THIS ROUND: " + gameController.playerScore.ToString ();
		gameoverScoreText.text += "\nHIGHSCORE: " + PlayerPrefs.GetInt ("HighScore", 0).ToString ();
        gameoverScoreText.text += "\nTOTAL STARS: " + (Spil.PlayerData.GetCurrencyBalance (25) + gameController.playerScore).ToString ();
	}

	public void Restart(){
		SceneManager.LoadScene (0);
	}

}
