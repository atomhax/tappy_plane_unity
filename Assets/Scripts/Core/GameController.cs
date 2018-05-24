using System;
using System.Collections.Generic;
using System.Xml;
using System.Text.RegularExpressions;
using System.Linq;
using SpilGames.Unity;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Helpers.AssetBundles;
using SpilGames.Unity.Helpers.GameData;
using SpilGames.Unity.Helpers.PlayerData;
using SpilGames.Unity.Json;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using AssetBundle = SpilGames.Unity.Helpers.AssetBundles.AssetBundle;
using Random = UnityEngine.Random;
#if !UNITY_TVOS
using Facebook.Unity;

#endif

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
    public int tapperScore = 0;

    public int latestRewardAmount = 0;
    public string latestRewardType = "";

    public Text gameTitleText, purchaceCompleteText, purchaceFailText;

    public PlayerController player;

    public ShopPanelController shopPanelController;

    public Transform playerStartLocation;

    public float obsitcleSpawnFrequency;

    public float obsticleSpawnYVarience;

    public Transform obsticleSpawnPoint;

    public GameObject obsticlePrefab, obsticleMaster;

    public SpriteRenderer[] backgroundSpriteRenderes;

    public Sprite[] backgroundSprites;

    List<GameObject> listOfObsticles = new List<GameObject>();

    private MergeConflictData localData;
    private MergeConflictData remoteData;

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
        tieredEventButton,
        FBLoginButton,
        FBLogoutButton,
        FBShareButton;

    public RuntimeAnimatorController goldPlaneController;
    public Sprite backgroundRuin;
    public Sprite backgroundTown;
    
    // Facebook
    public static List<string> userIds = new List<string>();

    public static List<string> userNames = new List<string>();
    public bool fbLoggedIn = false;

    public bool overlayEnabled = false;

    private bool showSyncDialog = true;
    private bool showMergeDialog = true;
    private bool showLockDialog = true;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Awake() {
        Analytics.enabled = false;
        Analytics.deviceStatsEnabled = false;
        
        Spil.Instance.OnPrivacyPolicyStatus -= OnPrivacyPolicyStatus;
        Spil.Instance.OnPrivacyPolicyStatus += OnPrivacyPolicyStatus;
        
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

        Spil.Instance.OnLiveEventNotAvailable -= OnLiveEventNotAvailable;
        Spil.Instance.OnLiveEventNotAvailable += OnLiveEventNotAvailable;

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
        
        Spil.Instance.OnTieredEventsAvailable -= OnTieredEventsAvailable;
        Spil.Instance.OnTieredEventsAvailable += OnTieredEventsAvailable;
        
        Spil.Instance.OnTieredEventsNotAvailable -= OnTieredEventsNotAvailable;
        Spil.Instance.OnTieredEventsNotAvailable += OnTieredEventsNotAvailable;

        Spil.Instance.OnTieredEventUpdated -= OnTieredEventUpdated;
        Spil.Instance.OnTieredEventUpdated += OnTieredEventUpdated;
        
        Spil.Instance.OnTieredEventProgressOpen -= OnTieredEventProgressOpen;
        Spil.Instance.OnTieredEventProgressOpen += OnTieredEventProgressOpen;
        
        Spil.Instance.OnTieredEventProgressClosed -= OnTieredEventProgressClosed;
        Spil.Instance.OnTieredEventProgressClosed += OnTieredEventProgressClosed;
        
        Spil.Instance.OnTieredEventsError -= OnTieredEventsError;
        Spil.Instance.OnTieredEventsError += OnTieredEventsError;
        
        Spil.Instance.OnAssetBundlesAvailable -= OnAssetBundlesAvailable;
        Spil.Instance.OnAssetBundlesAvailable += OnAssetBundlesAvailable;
        
        Spil.Instance.OnAssetBundlesNotAvailable -= OnAssetBundlesNotAvailable;
        Spil.Instance.OnAssetBundlesNotAvailable += OnAssetBundlesNotAvailable;
        
//		Spil.Instance.PreloadItemAndBundleImages();

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

        Spil.Instance.OnRequestLogin -= OnRequestLogin;
        Spil.Instance.OnRequestLogin += OnRequestLogin;

        Spil.Instance.OnUserDataAvailable -= OnUserDataAvailable;
        Spil.Instance.OnUserDataAvailable += OnUserDataAvailable;

        Spil.Instance.OnUserDataError -= OnUserDataError;
        Spil.Instance.OnUserDataError += OnUserDataError;

        Spil.Instance.OnUserDataMergeConflict -= OnUserDataMergeConflict;
        Spil.Instance.OnUserDataMergeConflict += OnUserDataMergeConflict;

        Spil.Instance.OnUserDataMergeSuccessful -= OnUserDataMergeSuccessful;
        Spil.Instance.OnUserDataMergeSuccessful += OnUserDataMergeSuccessful;

        Spil.Instance.OnUserDataMergeFailed -= OnUserDataMergeFailed;
        Spil.Instance.OnUserDataMergeFailed += OnUserDataMergeFailed;

        Spil.Instance.OnUserDataHandleMerge -= OnUserDataHandleMerge;
        Spil.Instance.OnUserDataHandleMerge += OnUserDataHandleMerge;

        Spil.Instance.OnUserDataSyncError -= OnUserDataSyncError;
        Spil.Instance.OnUserDataSyncError += OnUserDataSyncError;

        Spil.Instance.OnUserDataLockError -= OnUserDataLockError;
        Spil.Instance.OnUserDataLockError += OnUserDataLockError;

#if UNITY_ANDROID
        Spil.Instance.OnPermissionResponse -= OnPermissionResponse;
        Spil.Instance.OnPermissionResponse += OnPermissionResponse;
#endif
#if UNITY_EDITOR
        PlayerPrefs.SetInt("Background", 0);
        PlayerPrefs.SetInt("Skin", 0);
#endif

#if UNITY_EDITOR
        if (Spil.RewardToken != null && !Spil.RewardToken.Equals(""))
        {
			Spil.Instance.ClaimToken(Spil.RewardToken, "deeplink");
        }
#endif

        GetAndApplyGameConfig();
        SetupNewGame();

        initialPosition = player.gameObject.transform.position;
        initialRotation = player.gameObject.transform.rotation;

        if (!Spil.CheckPrivacyPolicy) {
            InitComponents();
        }
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
    void Start()
    {
    }

    private void InitComponents() {
#if !UNITY_TVOS
        FB.Init(OnFBInitComplete);
        Fabric.Runtime.Fabric.Initialize();
#endif
       
        Spil.Instance.RequestServerTime();
        Spil.Instance.RequestLiveEvent();
        Spil.Instance.RequestTieredEvents();
        SavePrivateGameState();
        RequestMoreApps();
        
        FireTrackEventSample();
    }
    
    public void OnPrivacyPolicyStatus(bool accepted) {
        if (accepted) {
            Debug.Log("Privacy Policy accepted!");
            
            InitComponents();
        } else {
            Debug.Log("Privacy Policy not accepted!");
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
        player.tapperCount = 0;

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

        Spil.Instance.OnAdAvailable -= Spil_Instance_OnAdAvailable;
        Spil.Instance.OnAdAvailable += Spil_Instance_OnAdAvailable;

        Spil.Instance.OnAdNotAvailable -= Spil_Instance_OnAdNotAvailable;
        Spil.Instance.OnAdNotAvailable += Spil_Instance_OnAdNotAvailable;

        Spil.Instance.OnAdFinished -= Spil_Instance_OnAdFinished;
        Spil.Instance.OnAdFinished += Spil_Instance_OnAdFinished;

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
        if (backgroundSpriteRenderes != null) {
            foreach (SpriteRenderer spriteRenderer in backgroundSpriteRenderes) {
                Sprite sprite = backgroundSprites[PlayerPrefs.GetInt("Background", 0)];
                if (sprite != null) {
                    spriteRenderer.sprite = sprite;
                } else {
                    spriteRenderer.sprite = backgroundSprites[0];
                }
            }
            player.SetupPlayerSkin();
        }
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
        purchaceFailText.text = "Purchase not possible!\nYou need more currency\n";
       
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
        shopPanelController.tabsPanel.gameObject.SetActive(false);
        shopPanelController.ResetShop();
        shopPanelController.CreateShop();
        shopPanelController.iapManager.iapPanelController.SetupIAPButtons();
        shopPanelController.RequestRewardVideo();
    }

    public void OnGameStateUpdated(string access) {
        if (access.Equals("private")) {
            Debug.Log("Private Game State Updated! Request new private game state!");
            string privateGameStateString = Spil.Instance.GetPrivateGameState();
            Debug.Log("New Private Game state: " + privateGameStateString);

            if (privateGameStateString != null && !privateGameStateString.Equals("")) {
                privateGameStateString = privateGameStateString.Replace("\\", "");
                PrivateGameState privateGameState = JsonHelper.getObjectFromJson<PrivateGameState>(privateGameStateString);
                if (privateGameState != null) {
                    PlayerPrefs.SetInt("Background", privateGameState.Background);
                    PlayerPrefs.SetInt("Skin", privateGameState.Skin);
            
                    player.SetupPlayerSkin();
                    foreach (SpriteRenderer spriteRenderer in backgroundSpriteRenderes) {
                        spriteRenderer.sprite = backgroundSprites[PlayerPrefs.GetInt("Background", 0)];
                    }
                }
            }
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
        Debug.Log("Facebook Initialised");
        
        if (Spil.Instance.IsLoggedIn()) {
#if UNITY_IOS
			if (FB.IsLoggedIn) {
				Debug.Log("FB already Logged In!");

				String socialId = AccessToken.CurrentAccessToken.UserId;
				String token = AccessToken.CurrentAccessToken.TokenString;
				if (socialId != null && token != null) {
					Debug.Log("Saving User Id");
					Spil.Instance.UserLogin(socialId, "facebook", token);

					Debug.Log("Requesting friends list");
					FB.API("/me/friends?fields=id,name", HttpMethod.GET, HandleFriendsLoaded);

					FBLoginButton.SetActive(false);
					FBLogoutButton.SetActive(true);
					highScoreButton.SetActive(true);
					FBShareButton.SetActive(true);
				}
			} else {
				FacebookLogin();
			}
#else
            FacebookLogin();
#endif
        }

		FB.GetAppLink(DeepLinkCallback);
		FB.Mobile.FetchDeferredAppLinkData(DeepLinkCallback);
    }

    public void FacebookLogin() {
#if !UNITY_TVOS
//        if (!FB.IsLoggedIn) {
            Debug.Log("Requesting Log In information");
            overlayEnabled = true;
            
            FB.LogInWithReadPermissions(new List<string>() {
                "public_profile",
                "email",
                "user_friends"
            }, this.HandleResult);
//        }
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

				if (key.Equals("error") || result.Cancelled) {
					if (!Spil.Instance.IsLoggedIn ()) {
						Spil.Instance.ShowNativeDialog ("Login Error", "Error communicating with the server!", "Ok");
					} else {
						ShowAuthErrorDialog ();
					}
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

	void DeepLinkCallback(IAppLinkResult result) {

		// Tested with FB dev console deeplink helper: tappyplane://token=DCWC0P78&reward=%5B%7B%22type%22%3A+%22CURRENCY%22%2C+%22amount%22%3A+1000%2C+%22id%22%3A+28%7D%5D
		// Tested with add links on custom made web page: http://splashscreens.cdn.spilcloud.com/4/FBDeeplinkTest4.html

		Debug.Log("FB DeeplinkCallback: " + result);
		if(!String.IsNullOrEmpty(result.Url)) {
			Debug.Log("FB Deeplink detected: " + result.Url);

			Dictionary<string, string> urlParams = GetParams (result.Url.Replace("://", "://&")); // Hack fix: Make sure the token parameter is detected.

			string rewardToken = urlParams["token"];
			if(!String.IsNullOrEmpty(rewardToken)) {

				Debug.Log("RewardToken: " + rewardToken);

				string rewardData = urlParams["reward"];
				if (!String.IsNullOrEmpty (rewardData)) {
					Spil.Instance.ClaimToken(rewardToken, "deeplink");
				}
			}
		}
	}

	static Dictionary<string, string> GetParams(string uri) {
		MatchCollection matches = Regex.Matches(uri, @"[\?&](([^&=]+)=([^&=#]*))", RegexOptions.None);
		return matches.Cast<Match>().ToDictionary(
			m => Uri.UnescapeDataString(m.Groups[2].Value),
			m => Uri.UnescapeDataString(m.Groups[3].Value)
		);
	}

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
        item1.uniqueItemId = Guid.NewGuid().ToString();
        item1.uniqueItemIdType = "test";
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

	private void OnLiveEventNotAvailable() {
		Debug.Log("Live Event Not Available");
		liveEventButton.SetActive(false);
	}

    public void OpenLiveEvent() {
        overlayEnabled = true;
       
        Spil.Instance.OpenLiveEvent();
    }

    private void OnLiveEventCompleted() {
        liveEventButton.SetActive(false);
        overlayEnabled = false;
    }

    private void OnTieredEventsAvailable() {
        Debug.Log("Tiered Event Available");
        tieredEventButton.GetComponentInChildren<Text>().text = Spil.Instance.GetAllTieredEvents()[0].name;
        tieredEventButton.SetActive(true);
    }
    
    private void OnTieredEventsNotAvailable() {
        Debug.Log("Tiered Event Not Available");
        tieredEventButton.SetActive(false);
    }
    
    private void OnTieredEventProgressOpen() {
        overlayEnabled = true;
    }
    
    private void OnTieredEventProgressClosed() {
        overlayEnabled = false;
    }
    
    private void OnTieredEventUpdated(TieredEventProgress tieredprogress) {
        Debug.Log("Update for Tiered Event: " + JsonHelper.getJSONFromObject(tieredprogress));
    }

    private void OnTieredEventsError(SpilErrorMessage error) {
        Debug.Log("Error for Tiered Event: " + JsonHelper.getJSONFromObject(error));
    }

    public void OpenTieredEvent() {
        if (!overlayEnabled) {
            overlayEnabled = true;
            Spil.Instance.ShowTieredEventProgress(Spil.Instance.GetAllTieredEvents()[0].id); 
        } 
    }
    
    private void OnAssetBundlesAvailable() {
        AssetBundlesHelper assetBundlesHelper = Spil.Instance.GetAssetBundles();
        StartCoroutine(AssetBundleController.DownloadGoldenPlaneBundle(this, assetBundlesHelper.GetAssetBundle("goldplane")));

        List<AssetBundle> assetBundles = Spil.Instance.GetAssetBundles().GetAssetBundlesOfType("background");
        foreach (AssetBundle assetBundle in assetBundles) {
            StartCoroutine(AssetBundleController.DownloadBackgroundBundle(this, assetBundle));
        }
    }

    private void OnAssetBundlesNotAvailable() {
        Debug.Log("Asset bundles not available!");
    }

    public void SetRuinBackground() {
        foreach (SpriteRenderer spriteRenderer in backgroundSpriteRenderes) {
            spriteRenderer.sprite = backgroundRuin;
        }
    }

    public void SetTownBackground() {
        foreach (SpriteRenderer spriteRenderer in backgroundSpriteRenderes) {
            spriteRenderer.sprite = backgroundTown;
        }
    }
    
    private void OnLoginSuccessful(bool resetData, string socialProvider, string s, bool isGuest) {
        Debug.Log("Login Successful!");

        if (isGuest) {
            Debug.Log("User is guest!");
            FacebookLogout();
        }

        if (resetData) {
            Spil.Instance.ResetData();
            Spil.Instance.RequestLiveEvent();
            Spil.Instance.RequestTieredEvents();
            
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
        
        Spil.Instance.ShowNativeDialog("Login Error", errorMessage.message, "Ok");
        
        FacebookLogout();
    }

    private void OnLogoutSuccessful() {
        Debug.Log("Logout Successful");

        Spil.Instance.ResetData();
        Spil.Instance.RequestLiveEvent();
        Spil.Instance.RequestTieredEvents();
            
        PlayerPrefs.SetInt("Background", 0);
        PlayerPrefs.SetInt("Skin", 0);
        
        player.SetupPlayerSkin();
        foreach (SpriteRenderer spriteRenderer in backgroundSpriteRenderes) {
            spriteRenderer.sprite = backgroundSprites[PlayerPrefs.GetInt("Background", 0)];
        }
        
        
    }

    private void OnLogoutFailed(SpilErrorMessage errorMessage) {
        Debug.Log("Logout failed! Error: " + errorMessage.message);
        
        Spil.Instance.ShowNativeDialog("Logout Error", errorMessage.message, "Ok");
    }

    private void OnAuthenticationError(SpilErrorMessage errorMessage) {
        Debug.Log("Authentication Error: " + errorMessage.message);
		
		ShowAuthErrorDialog();
    }

	private void ShowAuthErrorDialog() {
		Spil.Instance.ShowUnauthorizedDialog("Unauthorized", "The account you are currently using is not valid. Please select one of the actions to resolve the issue:", "Re-login", "Play as Guest");
	}
    
    private void OnRequestLogin() {
        Debug.Log("Login requested!");
        FacebookLogin();
    }

	void OnUserDataAvailable() {
		// Refresh the data
		Debug.Log("Private Game State Updated! Request new private game state!");
		string privateGameStateString = Spil.Instance.GetPrivateGameState();
		Debug.Log("New Private Game state: " + privateGameStateString);
		if (privateGameStateString != null && !privateGameStateString.Equals("")) {
			privateGameStateString = privateGameStateString.Replace("\\", "");
			PrivateGameState privateGameState = JsonHelper.getObjectFromJson<PrivateGameState>(privateGameStateString);
			if (privateGameState != null) {
				PlayerPrefs.SetInt("Background", privateGameState.Background);
				PlayerPrefs.SetInt("Skin", privateGameState.Skin);

				player.SetupPlayerSkin();
				foreach (SpriteRenderer spriteRenderer in backgroundSpriteRenderes) {
					spriteRenderer.sprite = backgroundSprites[PlayerPrefs.GetInt("Background", 0)];
				}
			}
		}
		
	    shopPanelController.updatePlayerValues();
	    
	    showMergeDialog = true;
	    showSyncDialog = true;
	}

	void OnUserDataError(SpilErrorMessage errorMessage) {
		Debug.Log("Error: " + errorMessage.message);

	    if (errorMessage.id == 38) {
	        showMergeDialog = true;
	        showSyncDialog = true;
	    }
	}

	void OnUserDataMergeConflict(MergeConflictData localData, MergeConflictData remoteData) {
	    if (showMergeDialog) {
	        this.localData = localData;
	        this.remoteData = remoteData;
	        string message = "Local time: " + localData.metaData.clientTime + "\nLocal device model: " + localData.metaData.deviceModel + "\n" +
	                         "\nRemote time: " + remoteData.metaData.clientTime + "\nRemote device model: " + remoteData.metaData.deviceModel;
	        Spil.Instance.ShowMergeConflictDialog("Merge Conflict", message, "Local", "Remote", "Merge");

	        showMergeDialog = false;
	        showSyncDialog = true;
	    }
	}

	void OnUserDataMergeSuccessful() {
		Spil.Instance.ShowNativeDialog ("Merge Successful", "The User Data was merged succesfully!", "Ok!");

		// Refresh the data
		Debug.Log("Private Game State Updated! Request new private game state!");
		string privateGameStateString = Spil.Instance.GetPrivateGameState();
		Debug.Log("New Private Game state: " + privateGameStateString);
		if (privateGameStateString != null && !privateGameStateString.Equals("")) {
			privateGameStateString = privateGameStateString.Replace("\\", "");
			PrivateGameState privateGameState = JsonHelper.getObjectFromJson<PrivateGameState>(privateGameStateString);
			if (privateGameState != null) {
				PlayerPrefs.SetInt("Background", privateGameState.Background);
				PlayerPrefs.SetInt("Skin", privateGameState.Skin);

				player.SetupPlayerSkin();
				foreach (SpriteRenderer spriteRenderer in backgroundSpriteRenderes) {
					spriteRenderer.sprite = backgroundSprites[PlayerPrefs.GetInt("Background", 0)];
				}
			}
		}
		Spil.PlayerData.UpdatePlayerData();
	    
	    shopPanelController.updatePlayerValues();
	    
	    showMergeDialog = true;
	    showSyncDialog = true;
	}

	void OnUserDataMergeFailed(string mergeData, string mergeType) {
	    showMergeDialog = true;
	    showSyncDialog = true;
	    
		Spil.Instance.ShowMergeFailedDialog ("Merge Failed", "User Data could not be merged. Please try again!", "Try Again", mergeData, mergeType);
	}

	void OnUserDataHandleMerge(string mergeType) {
	    string jsonString = null;
		if (mergeType == MergeConflict.Local) {
			jsonString = JsonHelper.getJSONFromObject (localData);
		} if (mergeType == MergeConflict.Remote) {
	        jsonString = JsonHelper.getJSONFromObject (remoteData);
		} if (mergeType == MergeConflict.Merge) {
			MergeConflictData mergedData = new MergeConflictData ();
			mergedData.playerData = localData.playerData;
			mergedData.gameState = remoteData.gameState;
			mergedData.metaData = localData.metaData;
	        jsonString = JsonHelper.getJSONFromObject (mergedData);
		}
	    
	    Spil.Instance.MergeUserData (jsonString, mergeType);
	}

	void OnUserDataSyncError() {
	    showMergeDialog = true;
	    if (showSyncDialog) {
	        Spil.Instance.ShowSyncErrorDialog ("Sync Error", "User Data synchronization error occurred. Please initiate merging process.", "Start merging process");
	        showSyncDialog = false;
	    }
		
	}

	void OnUserDataLockError() {
	    if (showLockDialog) {
	        Spil.Instance.ShowNativeDialog ("Lock Error", "User Data could not be written to the backend. Please try again in a few moments.", "Ok");
	        showLockDialog = false;
	           
	        Invoke("SetShowLockDialog", 5);
	    }
	
	}

    void SetShowLockDialog() {
        showLockDialog = true;
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