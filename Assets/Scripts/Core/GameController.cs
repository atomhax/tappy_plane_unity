using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;
using SpilGames.Unity.Utils;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	// different possible states of the game
	public enum GameStates
	{
		Start,
		InGame,
		GameOver,
		Shop
	}

	// the players score for each round
	public int playerScore = 0;

	public Text gameTitleText;


	//the player controller attached to the player gameobject 
	public PlayerController player;

	//the start position of the player at the beggining of each round
	public Transform playerStartLocation;

	//obsticle spawn variables
	public float obsitcleSpawnFrequency;
	public float obsticleSpawnYVarience;
	public Transform obsticleSpawnPoint;
	public GameObject obsticlePrefab;

	public SpriteRenderer[] backgroundSpriteRenderes;
	public Sprite[] backgroundSprites;

	//the list of all obsitcles spawned
	List<GameObject> listOfObsticles = new List<GameObject>();

	//the panels of the UI
	public GameObject startPanel, ingamePanel, gameoverPanel, shopPanel, skinSelectPanel;

	// Use this for initialization
	void Start () {
		GetAndApplyGameConfig ();
		SetupNewGame ();
	}

	public void AddToScore(){
		playerScore++;
		if(playerScore > PlayerPrefs.GetInt("HighScore",0)){
			PlayerPrefs.SetInt ("HighScore",playerScore);
		}	
	}
		
	public void SetupNewGame(){
		for (int i = 0; i < listOfObsticles.Count; i++) {
			Destroy (listOfObsticles[i]);
		}
		listOfObsticles.Clear ();
		playerScore = 0;
		player.playerBody.angularVelocity = 0;
		player.transform.position = playerStartLocation.position;
		player.transform.rotation = playerStartLocation.rotation;
		player.idleMode = true;
		UpdateUI (GameStates.Start);
		//set backgroundSprite
		foreach (SpriteRenderer r in backgroundSpriteRenderes) {
			r.sprite = backgroundSprites[PlayerPrefs.GetInt("Background",0)];
		}
		player.SetupPlayerSkin ();
	}


	void GetAndApplyGameConfig(){

		JSONObject config = new JSONObject( Spil.Instance.GetConfigAll ());

		obsitcleSpawnFrequency = float.Parse( config.GetField ("obsitcleSpawnFrequency").str);

		player.jumpForce = float.Parse (config.GetField ("playerJumpForce").str);

		gameTitleText.text = config.GetField ("gameName").str;
	}

	public void UpdateSkins(){
		foreach (SpriteRenderer r in backgroundSpriteRenderes) {
			r.sprite = backgroundSprites[PlayerPrefs.GetInt("Background",0)];
		}
		player.SetupPlayerSkin ();
	}


	public void StartNewGame(){
		player.idleMode = false;
		player.dead = false;

		InvokeRepeating ("SpawnObsticle", 0, obsitcleSpawnFrequency);
		UpdateUI (GameStates.InGame);
		Spil.Instance.SendlevelStartEvent ("MainGame");

	}

	public void GameOver(){
		PlayerData.AddToCoins (25,playerScore, "End Of Level");
		CancelInvoke ("SpawnObsticle");
		UpdateUI (GameStates.GameOver);
		Spil.Instance.SendplayerDiesEvent ("MainGame");

	}

	void SpawnObsticle(){
		float rand = Random.Range (-obsticleSpawnYVarience, obsticleSpawnYVarience);
		listOfObsticles.Add((GameObject)Instantiate (obsticlePrefab, new Vector3 (obsticleSpawnPoint.position.x, obsticleSpawnPoint.position.y + rand, 0), transform.rotation));
	}

	public void ToggleShop(){
		shopPanel.SetActive (!shopPanel.activeInHierarchy);
	}

	public void UpdateUI(GameStates gameState){
		startPanel.SetActive (gameState == GameStates.Start);
		ingamePanel.SetActive (gameState == GameStates.InGame);
		gameoverPanel.SetActive (gameState == GameStates.GameOver);
		shopPanel.SetActive (gameState == GameStates.Shop);
	}

	public void InGamePurchaesSuccess(){
		skinSelectPanel.SetActive (true);
		UpdateUI (GameStates.Start);
	}

}
