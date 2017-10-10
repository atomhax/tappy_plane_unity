using System;
using System.Collections.Generic;
using SpilGames.Unity;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Helpers.GameData;
using SpilGames.Unity.Helpers.PlayerData;
using SpilGames.Unity.Json;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
#if !UNITY_TVOS
using Facebook.Unity;

#endif

public class GameController : MonoBehaviour {
    public enum GameStates {
        Start,
        InGame,
        GameOver,
        Shop
    }

    public int playerScore = 0;
    public int tapperScore = 0;

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

    List<GameObject> listOfObsticles = new List<GameObject>();

    public GameObject startPanel,
        ingamePanel,
        gameoverPanel,
        shopPanel,
        skinSelectPanel,
        tabsPanel,
        inGamePurchaseSuccessPanel,
        inGamePurchaseFailPanel,
        highScorePanel,
        highScoreButton,
        moreGamesButton,
        dailyBonusButton,
        liveEventButton,
        FBLoginButton,
        FBLogoutButton,
        FBShareButton;

    // Facebook
    public static List<string> userIds = new List<string>();

    public static List<string> userNames = new List<string>();
    public bool fbLoggedIn = false;

    public bool overlayEnabled = false;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Awake() {
#if !UNITY_TVOS
        FB.Init(this.OnFBInitComplete);
#endif
    }

#if UNITY_TVOS
	void FixedUpdate ()
	{
		if (player.idleMode) 
		{
			if(Input.GetKeyDown(KeyCode.Joystick1Button15))
			{
				Debug.Log ("New game!");
				StartNewGame ();
			}
		} 
		else 
		{
			Debug.LogWarning ("Check!");
			if(Input.GetKeyDown(KeyCode.Joystick1Button14) || Input.GetKeyDown(KeyCode.Joystick1Button15))
			{
				Debug.Log ("Jump!");
				if (GameObject.Find ("GameOverPanel")) {
					Debug.LogWarning ("found!");
					GameObject.Find ("GameOverPanel").GetComponent<GameOverPanelController> ().Restart ();
				} else {
					player.Jump ();
				}
			}
		}
	}
	#endif

    // Use this for initialization
    void Start() {
        Spil.Instance.OnReward -= Spil_Instance_OnReward;
        Spil.Instance.OnReward += Spil_Instance_OnReward;

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

        Spil.Instance.OnServerTimeRequestSuccess -= OnServerTimeRequestSuccess;
        Spil.Instance.OnServerTimeRequestSuccess += OnServerTimeRequestSuccess;

        Spil.Instance.OnServerTimeRequestFailed -= OnServerTimeRequestFailed;
        Spil.Instance.OnServerTimeRequestFailed += OnServerTimeRequestFailed;

        Spil.Instance.OnLiveEventAvailable -= OnLiveEventAvailable;
        Spil.Instance.OnLiveEventAvailable += OnLiveEventAvailable;

        Spil.Instance.OnLiveEventStageOpen -= OnLiveEventStageOpen;
        Spil.Instance.OnLiveEventStageOpen += OnLiveEventStageOpen;

        Spil.Instance.OnLiveEventStageClosed -= OnLiveEventStageClosed;
        Spil.Instance.OnLiveEventStageClosed += OnLiveEventStageClosed;

        Spil.Instance.OnLiveEventMetRequirements -= OnLiveEventMetRequirements;
        Spil.Instance.OnLiveEventMetRequirements += OnLiveEventMetRequirements;

        Spil.Instance.OnLiveEventError -= OnLiveEventError;
        Spil.Instance.OnLiveEventError += OnLiveEventError;

        Spil.Instance.OnLiveEventCompleted -= OnLiveEventCompleted;
        Spil.Instance.OnLiveEventCompleted += OnLiveEventCompleted;

        FireTrackEventSample();

//		Spil.Instance.PreloadItemAndBundleImages();

        Spil.Instance.RequestServerTime();
        Spil.Instance.RequestLiveEvent();

        Spil.Instance.OnRewardTokenReceived -= OnRewardTokenReceived;
        Spil.Instance.OnRewardTokenReceived += OnRewardTokenReceived;

        Spil.Instance.OnRewardTokenClaimed -= OnRewardTokenClaimed;
        Spil.Instance.OnRewardTokenClaimed += OnRewardTokenClaimed;

        Spil.Instance.OnRewardTokenClaimFailed -= OnRewardTokenClaimFailed;
        Spil.Instance.OnRewardTokenClaimFailed += OnRewardTokenClaimFailed;


        Spil.Instance.OnLoginSuccessful -= OnLoginSuccessful;
        Spil.Instance.OnLoginSuccessful += OnLoginSuccessful;

        Spil.Instance.OnLoginFailed -= OnLoginFailed;
        Spil.Instance.OnLoginFailed += OnLoginFailed;

        Spil.Instance.OnLogoutSuccessful -= OnLogoutSuccessful;
        Spil.Instance.OnLogoutSuccessful += OnLogoutSuccessful;

        Spil.Instance.OnLogoutFailed -= OnLogoutFailed;
        Spil.Instance.OnLogoutFailed += OnLogoutFailed;

        Spil.Instance.OnAuthenticationError -= OnAuthenticationError;
        Spil.Instance.OnAuthenticationError += OnAuthenticationError;

#if UNITY_ANDROID
        Spil.Instance.OnPermissionResponse -= OnPermissionResponse;
        Spil.Instance.OnPermissionResponse += OnPermissionResponse;
#endif
        GetAndApplyGameConfig();
        SetupNewGame();

#if UNITY_EDITOR
        if (Spil.RewardToken != null && !Spil.RewardToken.Equals("")) {
            Spil.TokenRewardTypeEnum rewardType = Spil.MonoInstance.TokenRewardType;
            Spil.Instance.ClaimToken(Spil.RewardToken, rewardType.ToString());
        }
#endif

        Debug.Log("Requesting Game State!!!!");
        Spil.Instance.RequestMyGameState();

        initialPosition = player.gameObject.transform.position;
        initialRotation = player.gameObject.transform.rotation;

        if (Spil.Instance.IsLoggedIn()) {
            FacebookLogin();
        }

        if (FB.IsInitialized && FB.IsLoggedIn) {
            highScoreButton.SetActive(true);
            FBShareButton.SetActive(true);
        }
    }

    void Spil_Instance_OnReward(PushNotificationRewardResponse rewardResponse) {
        Spil.PlayerData.Wallet.Add(rewardResponse.data.eventData.currencyId, rewardResponse.data.eventData.reward,
            PlayerDataUpdateReasons.EventReward, "Push Notification");
    }

    public void AddToScore() {
        playerScore++;
        if (playerScore > PlayerPrefs.GetInt("HighScore", 0)) {
            PlayerPrefs.SetInt("HighScore", playerScore);
        }
    }

    public void AddToTapper() {
        tapperScore++;
    }

    public void SetupNewGame() {
        ClearOutOldObsticles();
        playerScore = 0;
        tapperScore = 0;

        player.dead = false;
        player.idleMode = true;

        player.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        player.gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0;
        player.gameObject.transform.position = initialPosition;
        player.gameObject.transform.rotation = initialRotation;

        UpdateUI(GameStates.Start);
        foreach (SpriteRenderer spriteRenderer in backgroundSpriteRenderes) {
            spriteRenderer.sprite = backgroundSprites[PlayerPrefs.GetInt("Background", 0)];
        }
        player.SetupPlayerSkin();

        Spil.Instance.OnGameStateUpdated -= OnGameStateUpdated;
        Spil.Instance.OnGameStateUpdated += OnGameStateUpdated;

        SavePrivateGameState();

        Spil.Instance.OnAdAvailable -= Spil_Instance_OnAdAvailable;
        Spil.Instance.OnAdAvailable += Spil_Instance_OnAdAvailable;

        Spil.Instance.OnAdNotAvailable -= Spil_Instance_OnAdNotAvailable;
        Spil.Instance.OnAdNotAvailable += Spil_Instance_OnAdNotAvailable;

        Spil.Instance.OnAdFinished -= Spil_Instance_OnAdFinished;
        Spil.Instance.OnAdFinished += Spil_Instance_OnAdFinished;

        RequestMoreApps();

        UpdateUI(GameStates.Start);
    }

    void ClearOutOldObsticles() {
        for (int i = 0; i < listOfObsticles.Count; i++) {
            Destroy(listOfObsticles[i]);
        }
        listOfObsticles.Clear();
    }

    void GetAndApplyGameConfig() {
        try {
            JSONObject config = new JSONObject(Spil.Instance.GetConfigAll());
            obsitcleSpawnFrequency = float.Parse(config.GetField("obsitcleSpawnFrequency").str);
            player.jumpForce = float.Parse(config.GetField("playerJumpForce").str);
            gameTitleText.text = config.GetField("gameName").str;
        }
        catch {
            Debug.Log("Setup Config Failed");
        }
    }

    public void UpdateSkins() {
        foreach (SpriteRenderer spriteRenderer in backgroundSpriteRenderes) {
            spriteRenderer.sprite = backgroundSprites[PlayerPrefs.GetInt("Background", 0)];
        }
        player.SetupPlayerSkin();
    }


    public void StartNewGame() {
        if (overlayEnabled) return;
        player.idleMode = false;
        player.dead = false;
        InvokeRepeating("SpawnObsticle", 0, obsitcleSpawnFrequency);
        UpdateUI(GameStates.InGame);
        Spil.Instance.TrackLevelStartEvent("MainGame");
    }

    public void GameOver() {
        Spil.PlayerData.Wallet.Add(25, playerScore, PlayerDataUpdateReasons.LevelComplete, "Game Over Screen");
        Spil.PlayerData.Inventory.Add(100077, tapperScore, PlayerDataUpdateReasons.LevelComplete, "Game Over Screen");
        CancelInvoke("SpawnObsticle");
        UpdateUI(GameStates.GameOver);
        Spil.Instance.TrackPlayerDiesEvent("MainGame");
    }

    void SpawnObsticle() {
        float rand = Random.Range(-obsticleSpawnYVarience, obsticleSpawnYVarience);
        GameObject newObsticle = (GameObject) Instantiate(obsticlePrefab,
            new Vector3(obsticleSpawnPoint.position.x, obsticleSpawnPoint.position.y + rand, 0), transform.rotation);
        newObsticle.transform.parent = obsticleMaster.transform;
        listOfObsticles.Add(newObsticle);
    }

    public void ToggleShop() {
        shopPanel.SetActive(!shopPanel.activeInHierarchy);
    }

    public void OpenShop() {
        shopPanel.SetActive(true);
    }

    public void OpenShop(PlayerCurrencyData currency) {
        shopPanel.SetActive(true);
        shopPanel.GetComponent<ShopPanelController>().ShowReward(currency);
    }

    public void ToggleHighScores() {
        highScorePanel.SetActive(!highScorePanel.activeInHierarchy);
#if UNITY_TVOS
		EventSystem eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
		eventSystem.firstSelectedGameObject = GameObject.Find("GoBackToStartButton");
		#endif
    }

    public void UpdateUI(GameStates gameState) {
        startPanel.SetActive(gameState == GameStates.Start);
        ingamePanel.SetActive(gameState == GameStates.InGame);
        gameoverPanel.SetActive(gameState == GameStates.GameOver);
        shopPanel.SetActive(gameState == GameStates.Shop);

#if UNITY_TVOS
		EventSystem eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
		switch (gameState) {
			case GameStates.Start:
				eventSystem.firstSelectedGameObject = GameObject.Find("ShopButton");
				GameObject.Find("FBShareButton").SetActive(false);
				break;
			case GameStates.InGame:
				//eventSystem.firstSelectedGameObject = GameObject.Find("ShopButton");
				break;
			case GameStates.GameOver:
				eventSystem.firstSelectedGameObject = GameObject.Find("RestartButton");
				break;
			case GameStates.Shop:
				GameObject.Find("HelpCenterButton").SetActive(false);
				eventSystem.firstSelectedGameObject = GameObject.Find("BackButton");
				break;
			default:
				break;
		}
		#endif
    }

    public void InGamePurchaesFail(Bundle bundle) {
        purchaceFailText.text = "Purchase Not Possible \n you need \n";
        for (int i = 0; i < bundle.Prices.Count; i++) {
            if (bundle.Prices[i].Value > 0) {
                purchaceFailText.text += bundle.Prices[i].Value.ToString() + " " +
                                         Spil.GameData.GetCurrency(bundle.Prices[i].CurrencyId).Name;
            }
        }
        inGamePurchaseFailPanel.SetActive(true);
    }

    public void InGamePurchaesSuccess(string bundleName) {
        purchaceCompleteText.text = "Purchase Complete \n" + bundleName;
        inGamePurchaseSuccessPanel.SetActive(true);
#if UNITY_TVOS
		EventSystem eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
		eventSystem.firstSelectedGameObject = GameObject.Find("OkButton");
		#endif
    }

    public void LoadSkinsPanelAfterPurchase() {
        inGamePurchaseSuccessPanel.SetActive(false);
    }

    public void OnGameStateUpdated(string access) {
        if (access.Equals("private")) {
            Debug.Log("Private Game State Updated! Request new private game state!");
            string privateGameState = Spil.Instance.GetPrivateGameState();
            Debug.Log("New Private Game state: " + privateGameState);
        } else if (access.Equals("public")) {
            Debug.Log("Public Game State Updated! Request new public game state!");
            string publicGameState = Spil.Instance.GetPublicGameState();
            Debug.Log("New Public Game state: " + publicGameState);
        }
    }

    public void OnDailyBonusOpen() {
        Debug.Log("DailyBonusOpen");
        overlayEnabled = true;
    }

    public void OnDailyBonusClosed() {
        Debug.Log("DailyBonusClosed");
        overlayEnabled = false;
    }

    public void OnDailyBonusReward(string reward) {
        Debug.Log("DailyBonusReward: " + reward);
    }

    public void OnDailyBonusError(SpilErrorMessage error) {
        Debug.Log("DailyBonusError with reason: " + error.message);
        overlayEnabled = false;
    }

    public void OnDailyBonusNotAvailable() {
        Debug.Log("DailyBonusNotAvailable");
    }

    public void OnPlayerDataUpdated(string reason, PlayerDataUpdatedData updatedData) {
        if (reason.Equals("Daily Bonus From Client")) {
            PlayerCurrencyData currency = updatedData.currencies[0];
            OpenShop(currency);
        }

        if (reason.Equals("Deeplink From Client") && updatedData.currencies.Count > 0) {
            PlayerCurrencyData currency = updatedData.currencies[0];
            OpenShop(currency);
        }
    }

    public void SavePrivateGameState() {
        int backgroundId = PlayerPrefs.GetInt("Background", 0);
        int skinId = PlayerPrefs.GetInt("Skin", 0);

        PrivateGameState gameState = new PrivateGameState();
        gameState.setBackground(backgroundId);
        gameState.setSkin(skinId);

        string gameStateJson = JsonHelper.getJSONFromObject(gameState);
        Spil.Instance.SetPrivateGameState(gameStateJson);
    }

    public void SavePublicGameState() {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        PublicGameState gameState = new PublicGameState();
        gameState.setHighScore(highScore);

        string gameStateJson = JsonHelper.getJSONFromObject(gameState);
        Spil.Instance.SetPublicGameState(gameStateJson);
    }

    private void OnFBInitComplete() {
        Debug.Log("Facebook Inistialised");
    }

    public void FacebookLogin() {
        Debug.Log("Requesting Log In information");
        overlayEnabled = true;
#if !UNITY_TVOS
        FB.LogInWithReadPermissions(new List<string>() {
            "public_profile",
            "email",
            "user_friends"
        }, this.HandleResult);
#endif
    }

    public void FacebookLogout() {
        FB.LogOut();
        Spil.Instance.UserLogout(false);
        FBLoginButton.SetActive(true);
        FBLogoutButton.SetActive(false);
        highScoreButton.SetActive(false);
        FBShareButton.SetActive(false);
    }

#if !UNITY_TVOS
    protected void HandleResult(IResult result) {
        overlayEnabled = false;
        fbLoggedIn = true;
        if (result.ResultDictionary != null) {
            string socialId = null;
            string token = null;
            foreach (string key in result.ResultDictionary.Keys) {
                Debug.Log(key + " : " + result.ResultDictionary[key].ToString());
                if (key.Equals("user_id")) {
                    Debug.Log("Saving User Id");
                    socialId = result.ResultDictionary[key].ToString();

                    Debug.Log("Requesting friends list");
                    FB.API("/me/friends?fields=id,name", HttpMethod.GET, HandleFriendsLoaded);
                }

                if (key.Equals("access_token")) {
                    FBLoginButton.SetActive(false);
                    FBLogoutButton.SetActive(true);
                    highScoreButton.SetActive(true);
                    FBShareButton.SetActive(true);

                    token = result.ResultDictionary[key].ToString();
                }
            }

            if (socialId != null && token != null) {
                Spil.Instance.UserLogin(socialId, "facebook", token);
            }
        }
    }

    protected void HandleFriendsLoaded(IResult result) {
        Debug.Log("FB friends loaded!");

        if (result.ResultDictionary != null) {
            if (result.ResultDictionary.ContainsKey("data")) {
                List<object> data = (List<object>) result.ResultDictionary["data"];
                Debug.Log("FB friend count: " + data.Count);
                for (int i = 0; i < data.Count; i++) {
                    Dictionary<string, object> userData = data[i] as Dictionary<string, object>;
                    string userId = userData["id"] as string;
                    string userName = userData["name"] as string;
                    userIds.Add(userId);
                    userNames.Add(userName);
                }
            }
        }
    }
#endif

    public void RequestDailyBonus() {
        Spil.Instance.RequestDailyBonus();
    }

    public static string GetNameForFbId(string fbId) {
        for (int i = 0; i < userIds.Count; i++) {
            string id = userIds[i];
            if (id == fbId) {
                return userNames[i];
            }
        }
        return "Me";
    }

    public static string GetFriendIdsJson() {
        string json = "[";

        // Add the own user id
        json += "\"" + Spil.Instance.GetUserId() + "\"";
        if (userIds.Count > 0) {
            json += ",";
        }

        // Add the friend user ids
        for (int i = 0; i < userIds.Count; i++) {
            string id = userIds[i];
            json += "\"" + id + "\"";
            if (i + 1 != userIds.Count) {
                json += ",";
            }
        }

        json += "]";
        return json;
    }

    public void FireTrackEventSample() {
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

//		Spil.Instance.TrackWalletInventoryEvent("Test1", "GameStart", currencies);
//		Spil.Instance.TrackWalletInventoryEvent("Test2", "GameStart", null, items);
//		Spil.Instance.TrackWalletInventoryEvent("Test3", "GameStart", currencies, items, "Main Menu", "GPA.1234-5678-9123-45678");
    }

    void RewardHandler(PushNotificationRewardResponse rewardResponse) {
        Debug.Log("Push notification reward received. CurrencyName: " + rewardResponse.data.eventData.currencyName +
                  " CurrencyId: " + rewardResponse.data.eventData.currencyId + " Reward: " +
                  rewardResponse.data.eventData.reward);

        // Show the reward
        PlayerCurrencyData currency = new PlayerCurrencyData();
        currency.id = rewardResponse.data.eventData.currencyId;
        currency.name = rewardResponse.data.eventData.currencyName;
        currency.delta = rewardResponse.data.eventData.reward;
        OpenShop(currency);
    }


    public void FBShare() {
#if !UNITY_TVOS
        Uri url = new Uri("http://files.cdn.spilcloud.com/10/1479133368_tappy_logo.png");
        FB.ShareLink(url, "Tappy Plane", "Check out Tappy Plane for iOS and Android!", url, null);
#endif
    }

    void Spil_Instance_OnAdAvailable(enumAdType adType) {
        if (adType == enumAdType.MoreApps) {
            moreGamesButton.SetActive(true);
        }
    }

    void Spil_Instance_OnAdNotAvailable(enumAdType adType) {
        if (adType == enumAdType.MoreApps) {
            moreGamesButton.SetActive(false);
        }
    }

    void Spil_Instance_OnAdFinished(SpilAdFinishedResponse adResponse) {
        if (adResponse.type.Equals("moreApps")) {
            RequestMoreApps();
        }
    }

    void OnRewardTokenReceived(string token, List<RewardObject> reward, string rewardType) {
        Debug.Log("Received reward with token: " + token + "-- Rewards: " + JsonHelper.getJSONFromObject(reward) +
                  "-- For Type: " + rewardType);
        Spil.Instance.ClaimToken(token, rewardType);
    }

    void OnRewardTokenClaimed(List<RewardObject> reward, string rewardType) {
        Debug.Log("Claimed reward for: " + rewardType + "-- And reward: " + JsonHelper.getJSONFromObject(reward));
    }

    void OnRewardTokenClaimFailed(string rewardType, SpilErrorMessage error) {
        Debug.Log("Error claiming reward for: " + rewardType + "-- With message: " + error.message);
    }

    void OnServerTimeRequestSuccess(long time) {
        Debug.Log("Server time is: " + time);
    }

    void OnServerTimeRequestFailed(SpilErrorMessage errorMessage) {
        Debug.Log("Server failed to retrieve with error: " + errorMessage.message);
    }

    public void RequestMoreApps() {
        Spil.Instance.RequestMoreApps();
    }

    public void ShowMoreApps() {
        Spil.Instance.PlayMoreApps();
    }

    private void OnLiveEventError(SpilErrorMessage errorMessage) {
        overlayEnabled = false;
    }

    private void OnLiveEventMetRequirements(bool metRequirements) {
        if (!metRequirements) {
            overlayEnabled = false;
        }
    }

    private void OnLiveEventStageClosed() {
        overlayEnabled = false;
    }

    private void OnLiveEventStageOpen() {
        overlayEnabled = true;
    }

    private void OnLiveEventAvailable() {
        Debug.Log("Live Event Available");
        Debug.Log("Config for Live Event: " + Spil.Instance.GetLiveEventConfig());
        liveEventButton.SetActive(true);
    }

    public void OpenLiveEvent() {
        overlayEnabled = true;
        Spil.Instance.OpenLiveEvent();
    }

    private void OnLiveEventCompleted() {
        liveEventButton.SetActive(false);
        overlayEnabled = false;
    }

    private void OnLoginSuccessful(bool resetData, string socialProvider, string s, bool isGuest) {
        Debug.Log("Login Successful!");

        if (isGuest) {
            Debug.Log("User is guest!");
        }

        if (resetData) {
            Spil.Instance.ResetData();
            Spil.Instance.RequestLiveEvent();
            
            PlayerPrefs.SetInt("Background", 0);
            PlayerPrefs.SetInt("Skin", 0);
        
            player.SetupPlayerSkin();
            foreach (SpriteRenderer spriteRenderer in backgroundSpriteRenderes) {
                spriteRenderer.sprite = backgroundSprites[PlayerPrefs.GetInt("Background", 0)];
            }
        }
    }

    private void OnLoginFailed(SpilErrorMessage errorMessage) {
        Debug.Log("Login failed! Error: " + errorMessage.message);
    }

    private void OnLogoutSuccessful() {
        Debug.Log("Logout Successful");

        Spil.Instance.ResetData();
        Spil.Instance.RequestLiveEvent();
            
        PlayerPrefs.SetInt("Background", 0);
        PlayerPrefs.SetInt("Skin", 0);
        
        player.SetupPlayerSkin();
        foreach (SpriteRenderer spriteRenderer in backgroundSpriteRenderes) {
            spriteRenderer.sprite = backgroundSprites[PlayerPrefs.GetInt("Background", 0)];
        }
        
        
    }

    private void OnLogoutFailed(SpilErrorMessage errorMessage) {
        Debug.Log("Logout failed! Error: " + errorMessage.message);
    }

    private void OnAuthenticationError(SpilErrorMessage errorMessage) {
        Debug.Log("Authentication Error: " + errorMessage.message);

        Spil.Instance.UserLogout(false);

        Spil.Instance.ShowUnauthorizedDialog("Unauthorized", "The account you are currently using is not valid. Please select one of the actions to resolve the issue:", "Re-login", "Play as Guest");
    }

#if UNITY_ANDROID
    private void OnPermissionResponse(SpilAndroidUnityImplementation.PermissionResponseObject permissionResponse) {
        Debug.Log("Tappy Plane Permission Response -- Permission: " + permissionResponse.permission + ", Status: " +
                  permissionResponse.granted + ", Is Permanently Denied: " + permissionResponse.permanentlyDenied);
    }
#endif

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