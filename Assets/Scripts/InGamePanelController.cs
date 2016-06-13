using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class InGamePanelController : MonoBehaviour {

	public Text currentScoreText;

	public GameController gameController;
	
	// Update is called once per frame
	void Update () {
		DisplayScore ();
	}

	void DisplayScore(){
		currentScoreText.text = "SCORE: " + gameController.playerScore.ToString ();
	}

}
