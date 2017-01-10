using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpilGames.Unity;
using SpilGames.Unity.Helpers;
using SpilGames.Unity.Utils;
using UnityEngine.UI;
using Facebook.Unity;
using SpilGames.Unity.Implementations;

public class GameController : MonoBehaviour
{

	public enum GameStates
	{
		Start,
		InGame,
		GameOver,
		Shop
	}

	public int playerScore = 0;

	public int latestRewardAmount = 0;
	public string latestRewardType = "";

	public Text gameTitleText, purchaceCompleteText, purchaceFailText;

	public PlayerController player;

	public Transform playerStartLocation;

	public float obsitcleSpawnFrequency;

	public float obsticleSpawnYVarience;

	public Transform obsticleSpawnPoint;

	public GameObject obsticlePrefab, obsticleMaster;

	public SpriteRenderer[] backgroundSpriteRenderes;

	public Sprite[] backgroundSprites;

	List<GameObject> listOfObsticles = new List<GameObject> ();

	public GameObject startPanel, ingamePanel, gameoverPanel, shopPanel, skinSelectPanel, tabsPanel, inGamePurchaseSuccessPanel, inGamePurchaseFailPanel, highScorePanel, moreGamesButton;

	// Facebook
	public static List<string> userIds = new List<string> ();
	public static List<string> userNames = new List<string> ();

	private Vector3 initialPosition;
 	private Quaternion initialRotation;

	void Awake ()
	{
		FB.Init (this.OnFBInitComplete);

	}

	// Use this for initialization
	void Start ()
	{

		initialPosition = player.gameObject.transform.position;
     		initialRotation = player.gameObject.transform.rotation;

		Spil.Instance.OnReward += Spil_Instance_OnReward;
		GetAndApplyGameConfig ();
		SetupNewGame ();

		Spil.Instance.OnDailyBonusOpen -= OnDailyBonusOpen;
		Spil.Instance.OnDailyBonusOpen += OnDailyBonusOpen;

		Spil.Instance.OnDailyBonusClosed -= OnDailyBonusClosed;
		Spil.Instance.OnDailyBonusClosed += OnDailyBonusClosed;

		Spil.Instance.OnDailyBonusNotAvailable -= OnDailyBonusNotAvailable;
		Spil.Instance.OnDailyBonusNotAvailable += OnDailyBonusNotAvailable;

		Spil.Instance.OnDailyBonusError -= OnDailyBonusError;
		Spil.Instance.OnDailyBonusError += OnDailyBonusError;

		Spil.Instance.OnDailyBonusReward -= OnDailyBonusReward;
		Spil.Instance.OnDailyBonusReward += OnDailyBonusReward;

		Spil.Instance.OnPlayerDataUpdated -= OnPlayerDataUpdated;
		Spil.Instance.OnPlayerDataUpdated += OnPlayerDataUpdated;

		Spil.Instance.OnReward -= RewardHandler;
        	Spil.Instance.OnReward += RewardHandler;

		FireTrackEventSample();

	}

	void Spil_Instance_OnReward (PushNotificationRewardResponse rewardResponse)
	{
		Spil.PlayerData.Wallet.Add (rewardResponse.data.eventData.currencyId, rewardResponse.data.eventData.reward, PlayerDataUpdateReasons.EventReward, "Push Notification");
	}

	public void AddToScore ()
	{
		playerScore++;
		if (playerScore > PlayerPrefs.GetInt ("HighScore", 0)) {
			PlayerPrefs.SetInt ("HighScore", playerScore);
		}	
	}

	public void SetupNewGame ()
	{
		ClearOutOldObsticles ();
		playerScore = 0;

		player.dead = false;
		player.idleMode = true;

		player.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		player.gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0; 
		player.gameObject.transform.position = initialPosition;
		player.gameObject.transform.rotation = initialRotation;

		UpdateUI (GameStates.Start);
		foreach (SpriteRenderer spriteRenderer in backgroundSpriteRenderes) {
			spriteRenderer.sprite = backgroundSprites [PlayerPrefs.GetInt ("Background", 0)];
		}
		player.SetupPlayerSkin ();

		Spil.Instance.OnGameStateUpdated -= OnGameStateUpdated;
		Spil.Instance.OnGameStateUpdated += OnGameStateUpdated;

		SavePrivateGameState ();

		Spil.Instance.OnAdAvailable -= Spil_Instance_OnAdAvailable;
		Spil.Instance.OnAdAvailable += Spil_Instance_OnAdAvailable;

		Spil.Instance.OnAdNotAvailable -= Spil_Instance_OnAdNotAvailable;
		Spil.Instance.OnAdNotAvailable += Spil_Instance_OnAdNotAvailable;

		Spil.Instance.OnAdFinished -= Spil_Instance_OnAdFinished;
		Spil.Instance.OnAdFinished += Spil_Instance_OnAdFinished;

		RequestMoreApps();
	}

	void ClearOutOldObsticles ()
	{
		for (int i = 0; i < listOfObsticles.Count; i++) {
			Destroy (listOfObsticles [i]);
		}
		listOfObsticles.Clear ();
	}

	void GetAndApplyGameConfig ()
	{
		try {
			JSONObject config = new JSONObject (Spil.Instance.GetConfigAll ());
			obsitcleSpawnFrequency = float.Parse (config.GetField ("obsitcleSpawnFrequency").str);
			player.jumpForce = float.Parse (config.GetField ("playerJumpForce").str);
			gameTitleText.text = config.GetField ("gameName").str;
		} catch {
			Debug.Log ("Setup Config Failed");
		}
	}

	public void UpdateSkins ()
	{
		foreach (SpriteRenderer spriteRenderer in backgroundSpriteRenderes) {
			spriteRenderer.sprite = backgroundSprites [PlayerPrefs.GetInt ("Background", 0)];
		}
		player.SetupPlayerSkin ();
	}


	public void StartNewGame ()
	{
		player.idleMode = false;
		player.dead = false;
		InvokeRepeating ("SpawnObsticle", 0, obsitcleSpawnFrequency);
		UpdateUI (GameStates.InGame);
		Spil.Instance.TrackLevelStartEvent ("MainGame");

	}

	public void GameOver ()
	{
		Spil.PlayerData.Wallet.Add (25, playerScore, PlayerDataUpdateReasons.LevelComplete, "Game Over Screen");
		CancelInvoke ("SpawnObsticle");
		UpdateUI (GameStates.GameOver);
		Spil.Instance.TrackPlayerDiesEvent ("MainGame");


	}

	void SpawnObsticle ()
	{
		float rand = Random.Range (-obsticleSpawnYVarience, obsticleSpawnYVarience);
		GameObject newObsticle = (GameObject)Instantiate (obsticlePrefab, new Vector3 (obsticleSpawnPoint.position.x, obsticleSpawnPoint.position.y + rand, 0), transform.rotation);
		newObsticle.transform.parent = obsticleMaster.transform;
		listOfObsticles.Add (newObsticle);
	}

	public void ToggleShop ()
	{
		shopPanel.SetActive (!shopPanel.activeInHierarchy);
	}

	public void OpenShop ()
	{
		shopPanel.SetActive (true);
	}

	public void OpenShop (PlayerCurrencyData currency) {
		shopPanel.SetActive (true);
		shopPanel.GetComponent<ShopPanelController>().ShowReward (currency);
	}

	public void ToggleHighScores ()
	{
		highScorePanel.SetActive (!highScorePanel.activeInHierarchy);
	}

	public void UpdateUI (GameStates gameState)
	{
		startPanel.SetActive (gameState == GameStates.Start);
		ingamePanel.SetActive (gameState == GameStates.InGame);
		gameoverPanel.SetActive (gameState == GameStates.GameOver);
		shopPanel.SetActive (gameState == GameStates.Shop);
	}

	public void InGamePurchaesFail (Bundle bundle)
	{
		purchaceFailText.text = "Purchase Not Possible \n you need \n";
		for (int i = 0; i < bundle.Prices.Count; i++) {
			if (bundle.Prices [i].Value > 0) {
				purchaceFailText.text += bundle.Prices [i].Value.ToString () + " " + Spil.GameData.GetCurrency (bundle.Prices [i].CurrencyId).Name;
			}
		}
		inGamePurchaseFailPanel.SetActive (true);
	}

	public void InGamePurchaesSuccess (string bundleName)
	{
		purchaceCompleteText.text = "Purchase Complete \n" + bundleName;
		inGamePurchaseSuccessPanel.SetActive (true);
	}

	public void LoadSkinsPanelAfterPurchase ()
	{
		inGamePurchaseSuccessPanel.SetActive (false);
		UpdateUI (GameStates.Start);
		skinSelectPanel.SetActive (true);
		tabsPanel.SetActive (false);
	}

	public void OnGameStateUpdated (string access)
	{
		if (access.Equals ("private")) {
			Debug.Log ("Private Game State Updated! Request new private game state!");
			string privateGameState = Spil.Instance.GetPrivateGameState ();
			Debug.Log ("New Private Game state: " + privateGameState);

		} else if (access.Equals ("public")) {
			Debug.Log ("Public Game State Updated! Request new public game state!");
			string publicGameState = Spil.Instance.GetPublicGameState ();
			Debug.Log ("New Public Game state: " + publicGameState);
		}
	}

	public void OnDailyBonusOpen ()
	{
		Debug.Log ("DailyBonusOpen");
	}

	public void OnDailyBonusClosed ()
	{
		Debug.Log ("DailyBonusClosed");
	}

	public void OnDailyBonusReward (string reward)
	{
		Debug.Log ("DailyBonusReward: " + reward);
	}

	public void OnDailyBonusError (SpilErrorMessage error)
	{
		Debug.Log ("DailyBonusError with reason: " + error.message);
	}

	public void OnDailyBonusNotAvailable ()
	{
		Debug.Log ("DailyBonusNotAvailable");
	}

	public void OnPlayerDataUpdated(string reason, PlayerDataUpdatedData updatedData) {
		if (reason == "Daily Bonus From Client") {
			PlayerCurrencyData currency = updatedData.currencies [0];
			OpenShop (currency);
		}
	}

	public void SavePrivateGameState ()
	{
		int backgroundId = PlayerPrefs.GetInt ("Background", 0);
		int skinId = PlayerPrefs.GetInt ("Skin", 0);

		PrivateGameState gameState = new PrivateGameState ();
		gameState.setBackground (backgroundId);
		gameState.setSkin (skinId);

		string gameStateJson = JsonHelper.getJSONFromObject (gameState);
		Spil.Instance.SetPrivateGameState (gameStateJson);
	}

	public void SavePublicGameState ()
	{
		int highScore = PlayerPrefs.GetInt ("HighScore", 0);

		PublicGameState gameState = new PublicGameState ();
		gameState.setHighScore (highScore);

		string gameStateJson = JsonHelper.getJSONFromObject (gameState);
		Spil.Instance.SetPublicGameState (gameStateJson);
	}

	private void OnFBInitComplete ()
	{
		Debug.Log ("Facebook Inistialised");
		Debug.Log ("Requesting Log In information");
		FB.LogInWithReadPermissions (new List<string> () { "public_profile", "email", "user_friends" }, this.HandleResult);
	}

	protected void HandleResult (IResult result)
	{
		if (result.ResultDictionary != null) {
			foreach (string key in result.ResultDictionary.Keys) {
				Debug.Log(key + " : " + result.ResultDictionary [key].ToString());
				if (key.Equals("user_id")) {
					Debug.Log("Saving User Id");
					Spil.Instance.SetUserId("Facebook", result.ResultDictionary [key].ToString());

					Debug.Log("Requesting friends list");
					FB.API("/me/friends?fields=id,name", HttpMethod.GET, HandleFriendsLoaded);
				}
			}
		}
	}

	protected void HandleFriendsLoaded (IResult result)
	{
		Debug.Log("FB friends loaded!");

		if (result.ResultDictionary != null) {
			if (result.ResultDictionary.ContainsKey ("data")) {
				List<object> data = (List<object>)result.ResultDictionary["data"];
				Debug.Log("FB friend count: " + data.Count);
				for (int i = 0; i < data.Count; i++) {
					Dictionary<string,object> userData = data[i] as Dictionary<string,object>;
					string userId = userData["id"] as string;
					string userName = userData["name"] as string;
					GameController.userIds.Add (userId);
					GameController.userNames.Add (userName);
				}
			}
		}

		RequestDailyBonus();
	}

	private void RequestDailyBonus(){
		Spil.Instance.RequestDailyBonus();
	}

	public static string GetNameForFbId(string fbId) {
		for (int i = 0; i < GameController.userIds.Count; i++) {
			string id = GameController.userIds [i];
			if (id == fbId) {
				return GameController.userNames [i];
			}
		}
		return "Me"; 
	}

	public static string GetFriendIdsJson() {
		string json = "[";

		// Add the own user id
		json += "\"" + Spil.Instance.GetUserId() + "\"";
		if (GameController.userIds.Count > 0) {
			json += ",";
		}

		// Add the friend user ids
		for (int i = 0; i < GameController.userIds.Count; i++) {
			string id = GameController.userIds [i];
			json += "\"" + id + "\"";
			if (i + 1 != GameController.userIds.Count) {
				json += ",";
			}
		}

		json += "]";
		return json; 
	}

	public void FireTrackEventSample(){

		List<TrackingCurrency> currencies = new List<TrackingCurrency>();

		TrackingCurrency currency1 = new TrackingCurrency();
		currency1.name = "TestCurrency";
		currency1.currentBalance = 110;
		currency1.delta = 10;
		currency1.type = 0;
		currencies.Add(currency1);

		List<TrackingItem> items = new List<TrackingItem>();

		TrackingItem item1 = new TrackingItem();
		item1.name = "TestItem";
		item1.amount = 2;
		item1.delta = 1;
		item1.type = 0;
		items.Add(item1);

//		Spil.Instance.TrackWalletInventoryEvent("Test1", "GameStart", currencies, null);
//		Spil.Instance.TrackWalletInventoryEvent("Test2", "GameStart", null, items);
//		Spil.Instance.TrackWalletInventoryEvent("Test3", "GameStart", currencies, items);

	}

	void RewardHandler(PushNotificationRewardResponse rewardResponse)
    	{
        	Debug.Log("Push notification reward received. CurrencyName: " + rewardResponse.data.eventData.currencyName + " CurrencyId: " + rewardResponse.data.eventData.currencyId + " Reward: " + rewardResponse.data.eventData.reward);
    	}


	public void FBShare ()
	{
		System.Uri url = new System.Uri ("http://files.cdn.spilcloud.com/10/1479133368_tappy_logo.png");
		FB.ShareLink (url, "Tappy Plane", "Check out Tappy Plane for iOS and Android!", url, null);
	}

	void Spil_Instance_OnAdAvailable (SpilGames.Unity.Utils.enumAdType adType)
	{
		if(adType == SpilGames.Unity.Utils.enumAdType.MoreApps){
			moreGamesButton.SetActive(true);
		}
	}

	void Spil_Instance_OnAdNotAvailable (SpilGames.Unity.Utils.enumAdType adType){
		if(adType == SpilGames.Unity.Utils.enumAdType.MoreApps){
			moreGamesButton.SetActive(false);
		}
	}

	void Spil_Instance_OnAdFinished(SpilAdFinishedResponse adResponse){
		if(adResponse.type.Equals("moreApps")){
			RequestMoreApps();
		}
	}

	public void RequestMoreApps(){
		Spil.Instance.RequestMoreApps();
	}

	public void ShowMoreApps(){
		Spil.Instance.PlayMoreApps();
	}

	/*public void FBShareScore ()
	{
		System.Uri url = new System.Uri ("http://files.cdn.spilcloud.com/10/1479133368_tappy_logo.png");
		FB.ShareLink (url, "Tappy Plane", "I've just completed Tappy Plane and collected " + playerScore.ToString() + " stars!", null, null);
	}

	public void FBShareReward ()
	{
		FB.FeedShare
		System.Uri url = new System.Uri ("http://files.cdn.spilcloud.com/10/1479133368_tappy_logo.png");
		FB.ShareLink (url, "Tappy Plane", "I've just been awarded with " + latestRewardAmount.ToString() + " free " + latestRewardType + "!", null, null);
	}*/
}