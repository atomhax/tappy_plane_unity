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
		SavePublicGameState();
		SceneManager.LoadScene (0);
	}

	public void SavePublicGameState(){
		int highScore = PlayerPrefs.GetInt("HighScore",0);

		PublicGameState gameState = new PublicGameState();
		gameState.setHighScore(highScore);

		string gameStateJson = JsonHelper.getJSONFromObject(gameState);
		Spil.Instance.SetPublicGameState(gameStateJson);
	}

}
