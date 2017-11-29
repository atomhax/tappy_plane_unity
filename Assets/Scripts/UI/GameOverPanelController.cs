using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;
using UnityEngine.SceneManagement;
using SpilGames.Unity.Json;


public class GameOverPanelController : MonoBehaviour {

	public Text starsScoreText;
	public Text tappersScoreText;
	

	public GameController gameController;

	void OnEnable(){
		DisplayGameoverScore ();
	}
	
	void DisplayGameoverScore(){
		starsScoreText.text = gameController.playerScore.ToString ();
		tappersScoreText.text = gameController.tapperScore.ToString();
	}

	public void Restart(){
		SavePublicGameState();
		gameController.SetupNewGame();
	}

	public void SavePublicGameState(){
		int highScore = PlayerPrefs.GetInt("HighScore",0);

		PublicGameState gameState = new PublicGameState();
		gameState.setHighScore(highScore);

		string gameStateJson = JsonUtility.ToJson(gameState);
		Spil.Instance.SetPublicGameState(gameStateJson);
	}

}
