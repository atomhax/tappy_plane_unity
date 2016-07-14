using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;
using SpilGames.Unity.Utils;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public enum GameStates
	{
		Start,
		InGame,
		GameOver,
		Shop
	}
		
	public int playerScore = 0;

	public Text gameTitleText, purchaceCompleteText, purchaceFailText;

	public PlayerController player;

	public Transform playerStartLocation;

	public float obsitcleSpawnFrequency;

	public float obsticleSpawnYVarience;

	public Transform obsticleSpawnPoint;

	public GameObject obsticlePrefab, obsticleMaster;

	public SpriteRenderer[] backgroundSpriteRenderes;

	public Sprite[] backgroundSprites;

	List<GameObject> listOfObsticles = new List<GameObject>();

	public GameObject startPanel, ingamePanel, gameoverPanel, shopPanel, skinSelectPanel, tabsPanel, inGamePurchaseSuccessPanel, inGamePurchaseFailPanel;

	// Use this for initialization
	void Start () {
		Spil.Instance.OnReward += Spil_Instance_OnReward;
		GetAndApplyGameConfig ();
		SetupNewGame ();
	}

	void Spil_Instance_OnReward (PushNotificationRewardResponse rewardResponse)
	{
		Spil.SpilPlayerDataInstance.Wallet.Add (rewardResponse.data.eventData.currencyId,rewardResponse.data.eventData.reward, PlayerDataUpdateReasons.EventReward );
	}
		
	public void AddToScore(){
		playerScore++;
		if(playerScore > PlayerPrefs.GetInt("HighScore",0)){
			PlayerPrefs.SetInt ("HighScore",playerScore);
		}	
	}
		
	public void SetupNewGame(){
		ClearOutOldObsticles ();
		playerScore = 0;
		player.idleMode = true;
		UpdateUI (GameStates.Start);
		foreach (SpriteRenderer spriteRenderer in backgroundSpriteRenderes) {
			spriteRenderer.sprite = backgroundSprites[PlayerPrefs.GetInt("Background",0)];
		}
		player.SetupPlayerSkin ();
	}

	void ClearOutOldObsticles(){
		for (int i = 0; i < listOfObsticles.Count; i++) {
			Destroy (listOfObsticles[i]);
		}
		listOfObsticles.Clear ();
	}

	void GetAndApplyGameConfig(){
		try{
			JSONObject config = new JSONObject( Spil.Instance.GetConfigAll ());
			if (config.HasField ("obsitcleSpawnFrequency")) {
				if(float.Parse (config.GetField ("obsitcleSpawnFrequency").str) > 0){
					obsitcleSpawnFrequency = float.Parse (config.GetField ("obsitcleSpawnFrequency").str);
				}
				if(float.Parse (config.GetField ("playerJumpForce").str) > 0){
					player.jumpForce = float.Parse (config.GetField ("playerJumpForce").str);
				}
				gameTitleText.text = config.GetField ("gameName").str;
			}
		}catch{
			Debug.Log ("Setup Config Failed");
		}
	}

	public void UpdateSkins(){
		foreach (SpriteRenderer spriteRenderer in backgroundSpriteRenderes) {
			spriteRenderer.sprite = backgroundSprites[PlayerPrefs.GetInt("Background",0)];
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
		Spil.SpilPlayerDataInstance.Wallet.Add (25, playerScore,PlayerDataUpdateReasons.LevelComplete);
		CancelInvoke ("SpawnObsticle");
		UpdateUI (GameStates.GameOver);
		Spil.Instance.SendplayerDiesEvent ("MainGame");
	}

	void SpawnObsticle(){
		float rand = Random.Range (-obsticleSpawnYVarience, obsticleSpawnYVarience);
		GameObject newObsticle = (GameObject)Instantiate (obsticlePrefab, new Vector3 (obsticleSpawnPoint.position.x, obsticleSpawnPoint.position.y + rand, 0), transform.rotation);
		newObsticle.transform.parent = obsticleMaster.transform;
		listOfObsticles.Add(newObsticle);
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

	public void InGamePurchaesFail(Bundle bundle){
		purchaceFailText.text = "Purchase Not Possible \n you need \n";
		for(int i = 0 ; i < bundle.Prices.Count; i ++){
			if(bundle.Prices[i].Value > 0){
				purchaceFailText.text += bundle.Prices [i].Value.ToString () + " " + Spil.SpilGameDataInstance.GetCurrency (bundle.Prices [i].CurrencyId).Name;
			}
		}
		inGamePurchaseFailPanel.SetActive (true);
	}

	public void InGamePurchaesSuccess(string bundleName){
		purchaceCompleteText.text = "Purchase Complete \n" + bundleName;
		inGamePurchaseSuccessPanel.SetActive (true);
	}

	public void LoadSkinsPanelAfterPurchase(){
		inGamePurchaseSuccessPanel.SetActive (false);
		UpdateUI (GameStates.Start);
		skinSelectPanel.SetActive (true);
		tabsPanel.SetActive (false);
	}
		
}
