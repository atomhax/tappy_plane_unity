/*
 * Spil Games Unity SDK 2018
 * Version 3.1.0
 *
 * If you have any questions, don't hesitate to e-mail us at info@spilgames.com
 * Be sure to check the github page for documentation and the latest updates
 * https://github.com/spilgames/spil_event_unity_plugin
*/

using UnityEngine;
using System;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Helpers.GameData;
using SpilGames.Unity.Helpers.PlayerData;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Json;

namespace SpilGames.Unity {
    [HelpURL("http://www.spilgames.com/developers/integration/unity/unity-get-started/")]
    public partial class Spil : MonoBehaviour {

        private SpilGameDataHelper GameDataObject;
        /// <summary>
        /// Contains the game data: items, currencies, bundles (collections of items/currencies for softcurrency/hardcurrency transactions and gifting), Shop and Gacha boxes.
        /// For player-specific data such as owned items or currencies use the PlayerData object.
        /// </summary>
        public static SpilGameDataHelper GameData;

        private PlayerDataHelper PlayerDataObject;
        /// <summary>
        /// Contains data specific to the user: Owned items, Currencies and Gacha boxes.
        /// Provides methods for adding/deleting/buying items and currencies and opening gacha boxes.
        /// For saving/loading player data such as game progress, use the "Gamestate" related methods and events in Spil.Instance. 
        /// </summary>
        public static PlayerDataHelper PlayerData;

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        [SerializeField] public bool initializeOnAwake = true;

		/// <summary>
		/// Not intended for use by developers.
		/// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
		/// </summary>
		[SerializeField] public bool SpilLoggingEnabled = true;

        [Header("Privacy Policy Settings")]
        
        [SerializeField] private bool checkPrivacyPolicyAndroid = true;

        [SerializeField] private bool checkPrivacyPolicyIOS = true;
        
        [SerializeField] private bool checkPrivacyPolicyWeb = true;

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        public static bool CheckPrivacyPolicy {
            get
            {
#if UNITY_STANDALONE
                return false;
#elif UNITY_WEBGL
                return MonoInstance.checkPrivacyPolicyWeb;            
#elif UNITY_ANDROID
                return MonoInstance.checkPrivacyPolicyAndroid;
#elif UNITY_IPHONE || UNITY_TVOS
			    return MonoInstance.checkPrivacyPolicyIOS;
#endif
            }
        }

        [SerializeField] private bool useUnityPrefab;

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        public static bool UseUnityPrefab;

        public enum PrivacyPolicyPrefabOrientationEnum {
            Landscape,
            Portrait
        }

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        public PrivacyPolicyPrefabOrientationEnum PrefabOrientation;
        
        [Header("Android Settings")]
#if UNITY_ANDROID || UNITY_EDITOR
        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        [SerializeField] public string ProjectId = "";

        public enum enumAndroidStore
        {
            GooglePlay,
            Amazon
        }

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        [SerializeField] public enumAndroidStore AndroidStore = enumAndroidStore.GooglePlay;
#endif

        [Header("iOS Settings")]

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        [SerializeField] public string CustomBundleId;

        [Header("Editor Settings")]

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        [SerializeField] public bool EditorDebugMode = true;

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        [SerializeField] public string spilUserIdEditor;

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        public static string SpilUserIdEditor { get; set; }

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        [SerializeField] public string bundleIdEditor;

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        public static string BundleIdEditor { get; set; }

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        [SerializeField] public string iapPurchaseRequestValue;

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        public static string IapPurchaseRequest { get; set; }

        [Header("Reward Settings")] [Header("Ads")] [SerializeField]
        private string currencyName;

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        public static string CurrencyName { get; private set; }

        [SerializeField] private string currencyId;

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        public static string CurrencyId { get; private set; }

        [SerializeField] private int reward = 0;

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        public static int Reward { get; private set; }

        [Space(10)] [Header("Daily Bonus")] [SerializeField]
        private int dailyBonusId;

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        public static int DailyBonusId { get; private set; }

        [SerializeField] private string dailyBonusExternalId;

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        public static string DailyBonusExternalId { get; private set; }

        [SerializeField] private int dailyBonusAmount;

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        public static int DailyBonusAmount { get; private set; }

        public enum DailyBonusRewardTypeEnum {
            CURRENCY,
            ITEM,
            EXTERNAL
        };

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        public DailyBonusRewardTypeEnum DailyBonusRewardType;

        [Space(10)] [Header("Live Event")] [SerializeField]
        private int liveEventRewardId;

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        public static int LiveEventRewardId { get; private set; }

        [SerializeField] private string liveEventExternalRewardId;

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        public static string LiveEventExternalRewardId { get; private set; }

        [SerializeField] private int liveEventRewardAmount;

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        public static int LiveEventRewardAmount { get; private set; }

        public enum LiveEventRewardTypeEnum {
            CURRENCY,
            ITEM,
            EXTERNAL
        };

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        public LiveEventRewardTypeEnum LiveEventRewardType;

        [Space(10)] [Header("Token System")] [SerializeField]
        private string rewardToken;

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        public static string RewardToken { get; private set; }

        public enum RewardFeatureTypeEnum {
            DEEPLINK,
        };

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        public RewardFeatureTypeEnum RewardFeatureType;

        [SerializeField] private int tokenRewardId;

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        public static int TokenRewardId { get; private set; }

        [SerializeField] private string tokenExternalRewardId;

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        public static string TokenExternalRewardId { get; private set; }

        [SerializeField] private int tokenRewardAmount;

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        public static int TokenRewardAmount { get; private set; }

        public enum TokenRewardTypeEnum {
            CURRENCY,
            ITEM,
            EXTERNAL
        };

        /// <summary>
        /// Not intended for use by developers.
        /// SpilSDK settings can be changed via the SpilSDK menu in the inspector tab when selecting the SpilSDK object in your app's first scene.
        /// </summary>
        public TokenRewardTypeEnum TokenRewardType;
        
        [Header("WebGL Settings")]

        [SerializeField] public TextAsset defaultPlayerDataAsset;
        [SerializeField] public TextAsset defaultGameDataAsset;
        [SerializeField] public TextAsset defaultGameConfigAsset;
        [SerializeField] public GameObject privacyPolicyPopupPrefab;

        private static Spil monoInstance;

        /// <summary>
        /// Not intended for use by developers.
        /// Use Spil.Instance instead.
        /// </summary>
        public static Spil MonoInstance {
            get {
                if (monoInstance == null) {
                    monoInstance = FindObjectOfType<Spil>();
                    if (monoInstance == null) {
                        throw new NullReferenceException("Could not find a gameobject in the scene named \"SpilSDK\".");
                    }
                }

                return monoInstance;
            }
        }

#if UNITY_STANDALONE
        /// <summary>
        /// Main entry-point for developers, exposes all SpilSDK methods and events (except Spil.Initialize).
        /// </summary>
        public static SpilDummyUnityImplementation Instance = new SpilDummyUnityImplementation();

#elif UNITY_WEBGL
        /// <summary>
        /// Main entry-point for developers, exposes all SpilSDK methods and events (except Spil.Initialize).
        /// </summary>
        //public static SpilWebGLUnityImplementation Instance = new SpilWebGLUnityImplementation();        
        public static SpilWebGLUnityImplementation Instance = new SpilWebGLUnityImplementation();
        
#elif UNITY_EDITOR
        /// <summary>
        /// Main entry-point for developers, exposes all SpilSDK methods and events (except Spil.Initialize).
        /// </summary>
        public static SpilUnityEditorImplementation Instance = new SpilUnityEditorImplementation();

#elif UNITY_ANDROID
        /// <summary>
        /// Main entry-point for developers, exposes all SpilSDK methods and events (except Spil.Initialize).
        /// </summary>
		public static SpilAndroidUnityImplementation Instance = new SpilAndroidUnityImplementation();

#elif UNITY_IPHONE || UNITY_TVOS
        /// <summary>
        /// Main entry-point for developers, exposes all SpilSDK methods and events (except Spil.Initialize).
        /// </summary>
		public static SpiliOSUnityImplementation Instance = new SpiliOSUnityImplementation();
#endif

        void Awake() {            
//            if ((monoInstance != null) && (this != monoInstance)) {
//                if (Application.isPlaying) {
//                    Destroy (gameObject);
//                }
//                return;
//            }
//
//            monoInstance = this;
            
            if (Application.isPlaying) {
                DontDestroyOnLoad (gameObject);
            }

#if !UNITY_EDITOR
			GameDataObject = new SpilGameDataHelper ();
			GameData = GameDataObject;

			PlayerDataObject = new PlayerDataHelper ();
			PlayerData = PlayerDataObject;
#endif

            if (initializeOnAwake) {
                Initialize();
            }
        }

        /// <summary>
        /// Called automatically on awake when the "initialize on awake" checkbox is checked in the SpilSDK object's inspector view.
        /// If the checkbox is not checked this method needs to be called by the developer to initialise the SDK and make it ready for use.
        /// </summary>
        public void Initialize()
        {
            SpilLogging.Log("SpilSDK-Unity Init");
#if UNITY_STANDALONE
            SpilLogging.Error("Unsupported platform detected, skipping SpilSDK initialisation.");
            return;
#endif

            Instance.SetPluginInformation(SpilUnityImplementationBase.PluginName, SpilUnityImplementationBase.PluginVersion);       
#if UNITY_EDITOR || UNITY_WEBGL
            InitEditor();
#endif

#if UNITY_IOS
			if (!string.IsNullOrEmpty (CustomBundleId)) {
				Instance.SetCustomBundleId (CustomBundleId);
			}
#endif

#if UNITY_ANDROID
            //Check if Project Id is set
            if (ProjectId == null) {
                throw new UnityException(
                    "Project ID not set!! Please set your Project Id with the id provided by the Spil representative!");
            }
#endif

            UseUnityPrefab = useUnityPrefab;
            if (CheckPrivacyPolicy) {
                #if UNITY_WEBGL
                    Instance.CheckPrivacyPolicyUnity();
                #else
                if (useUnityPrefab) {
                    Instance.CheckPrivacyPolicyUnity();
                } else {
                    Instance.CheckPrivacyPolicy();
                }
                #endif
            } else {
                Instance.SpilInit(false);                
            }
            
            gameObject.name = "SpilSDK";


        }

        void InitEditor() {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(spilUserIdEditor)) {
                spilUserIdEditor = Guid.NewGuid().ToString();
            } else {
                SpilLogging.Log("Using a manually set user id. Social Login feature may not work properly!");
            }

            SpilUserIdEditor = spilUserIdEditor;
            SpilLogging.Log("SpilSDK-Unity Using SpilUserIdEditor: " + SpilUserIdEditor);
#elif UNITY_WEBGL
            SpilUserIdEditor = PlayerPrefs.GetString("SpilUserId");
            if (string.IsNullOrEmpty(SpilUserIdEditor)) {
                SpilUserIdEditor = Guid.NewGuid().ToString();
                PlayerPrefs.SetString("SpilUserId", SpilUserIdEditor);
            }
            SpilLogging.Log("SpilSDK-Unity Using SpilUserIdEditor: " + SpilUserIdEditor);
    
            string deviceId = Spil.Instance.GetDeviceId();
            SpilLogging.Log("SpilSDK-Unity Using DeviceId: " + deviceId);
#endif
            
            BundleIdEditor = bundleIdEditor;
            if (string.IsNullOrEmpty(bundleIdEditor)) {
                SpilLogging.Assert(!string.IsNullOrEmpty(bundleIdEditor), "SpilSDK-Unity No BundleIdEditor set!");
            }

            IapPurchaseRequest = iapPurchaseRequestValue;

            CurrencyName = currencyName;
            CurrencyId = currencyId;
            Reward = reward;

            DailyBonusId = dailyBonusId;
            DailyBonusExternalId = dailyBonusExternalId;
            DailyBonusAmount = dailyBonusAmount;

            LiveEventRewardId = liveEventRewardId;
            LiveEventExternalRewardId = liveEventExternalRewardId;
            LiveEventRewardAmount = liveEventRewardAmount;

            TokenRewardId = tokenRewardId;
            TokenExternalRewardId = tokenExternalRewardId;
            TokenRewardAmount = tokenRewardAmount;

            RewardToken = rewardToken;
            
#if UNITY_EDITOR || UNITY_WEBGL
            if (Spil.Instance.GetPrivValue() > -1)
            {
                Spil.Instance.SendCustomEventInternal("sessionStart", null);
            }
#endif
        }

        void OnApplicationQuit()
        {
#if UNITY_EDITOR || UNITY_WEBGL
            Spil.Instance.SendCustomEventInternal("sessionStop", null);
#endif
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void UserIdChangeRequest(string newUid) {
            Spil.Instance.fireUserIdChangeRequest(newUid);
        }
        
        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void UserIdChangeCompleted() {
            Spil.Instance.fireUserIdChangeCompleted();
        }
        
        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void AdStart() {
			Spil.Instance.fireAdStart();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void AdFinished(string response) {
			Spil.Instance.fireAdFinished(response);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void AdAvailable(string type) {
			Spil.Instance.fireAdAvailable(type);
        }
		
        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void AdNotAvailable(string type) {
			Spil.Instance.fireAdNotAvailable(type);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void OpenParentalGate() {
			Spil.Instance.fireOpenParentalGate();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void ConfigUpdated() {
			Spil.Instance.fireConfigUpdated();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void ConfigError(string error) {
			Spil.Instance.fireConfigError(error);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void OnResponseReceived(string response) {
            SpilLogging.Log("SpilSDK-Unity OnResponseReceived " + response);

            SpilResponse spilResponse = JsonHelper.getObjectFromJson<SpilResponse>(response);

            if (!spilResponse.type.ToLower().Trim().Equals("notificationreward")) return;
            PushNotificationRewardResponse rewardResponseData =
                JsonHelper.getObjectFromJson<PushNotificationRewardResponse>(response);
            fireOnRewardEvent(rewardResponseData);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        private void fireOnRewardEvent(PushNotificationRewardResponse rewardResponse) {
			Spil.Instance.fireOnRewardEvent(rewardResponse);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void SpilGameDataAvailable() {
			Spil.Instance.fireSpilGameDataAvailable();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void SpilGameDataError(string reason) {
			Spil.Instance.fireSpilGameDataError(reason);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void PlayerDataUpdated(string data) {
			Spil.Instance.firePlayerDataUpdated(data);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void PlayerDataEmptyGacha() {
			Spil.Instance.firePlayerDataEmptyGacha();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void GameStateUpdated(string access) {
            Spil.Instance.fireGameStateUpdated(access);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void OtherUsersGameStateLoaded(string message) {
			Spil.Instance.fireOtherUsersGameStateLoaded(message);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void SplashScreenOpen() {
            Spil.Instance.fireSplashScreenOpen();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void SplashScreenNotAvailable() {
	        Spil.Instance.fireSplashScreenNotAvailable();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void SplashScreenClosed() {
            Spil.Instance.fireSplashScreenClosed();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void SplashScreenOpenShop() {
            Spil.Instance.fireSplashScreenOpenShop();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void SplashScreenData(string payload) {
            Spil.Instance.fireSplashScreenData(payload);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void SplashScreenError(string error) {
			Spil.Instance.fireSplashScreenError(error);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void DailyBonusOpen() {
            Spil.Instance.fireDailyBonusOpen();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void DailyBonusAvailable(string type) {
            Spil.Instance.fireDailyBonusAvailable(type);
        }
        
        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void DailyBonusNotAvailable() {
			Spil.Instance.fireDailyBonusNotAvailable();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void DailyBonusClosed() {
            Spil.Instance.fireDailyBonusClosed();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void DailyBonusReward(string receivedReward) {
			Spil.Instance.fireDailyBonusReward(receivedReward);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void DailyBonusError(string error) {
			Spil.Instance.fireDailyBonusError(error);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void DeepLinkReceived(string deepLink) {
            Spil.Instance.fireDeepLinkReceived(deepLink);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void RewardTokenReceived(string response) {
            Spil.Instance.fireRewardTokenReceived(response);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void RewardTokenClaimed(string response) {
			Spil.Instance.fireRewardTokenClaimed(response);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void RewardTokenClaimFailed(string response) {
            Spil.Instance.fireRewardTokenClaimFailed(response);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void ImageLoadSuccess(string response) {
			Spil.Instance.fireImageLoadSuccess(response);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void ImageLoadFailed(string response) {
            Spil.Instance.fireImageLoadFailed(response);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void ImagePreloadingCompleted() {
            Spil.Instance.fireImagePreloadingCompleted();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void IAPValid(string data) {
            Spil.Instance.fireIAPValid(data);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void IAPInvalid(string message) {
            Spil.Instance.fireIAPInvalid(message);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void IAPRequestPurchase(string skuId) {
            Spil.Instance.fireIAPRequestPurchase(skuId);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void IAPServerError(string error) {
			Spil.Instance.fireIAPServerError(error);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void ServerTimeRequestSuccess(string time) {
			Spil.Instance.fireServerTimeRequestSuccess(time);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void ServerTimeRequestFailed(string error) {
			Spil.Instance.fireServerTimeRequestFailed(error);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void LiveEventAvailable() {
            Spil.Instance.fireLiveEventAvailable();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void LiveEventStageOpen() {
            Spil.Instance.fireLiveEventStageOpen();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void LiveEventStageClosed() {
            Spil.Instance.fireLiveEventStageClosed();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void LiveEventNotAvailable() {
            Spil.Instance.fireLiveEventNotAvailable();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void LiveEventError(string error) {
			Spil.Instance.fireLiveEventError(error);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void LiveEventReward(string receivedReward) {
			Spil.Instance.fireLiveEventReward(receivedReward);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void LiveEventUsedExternalItems(string items) {
            Spil.Instance.fireLiveEventUsedExternalItems(items);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void LiveEventMetRequirements(string sMetRequirements) {
			Spil.Instance.fireLiveEventMetRequirements(sMetRequirements);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void LiveEventCompleted() {
            Spil.Instance.fireLiveEventCompleted();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void LoginSuccessful(string message) {
			Spil.Instance.fireLoginSuccessful(message);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void LoginFailed(string error) {
			Spil.Instance.fireLoginFailed(error);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void RequestLogin() {
            Spil.Instance.fireRequestLogin();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void LogoutSuccessful() {
            Spil.Instance.fireLogoutSuccessful();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void LogoutFailed(string error) {
			Spil.Instance.fireLogoutFailed(error);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void AuthenticationError(string error) {
			Spil.Instance.fireAuthenticationError(error);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void UserDataMergeConflict(string data) {
			Spil.Instance.fireUserDataMergeConflict(data);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void UserDataMergeSuccessful() {
            Spil.Instance.fireUserDataMergeSuccessful();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void UserDataMergeFailed(string data) {
            Spil.Instance.fireUserDataMergeFailed(data);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void UserDataHandleMerge(string mergeType) {
            Spil.Instance.fireUserDataHandleMerge(mergeType);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void UserDataSyncError() {
            Spil.Instance.fireUserDataSyncError();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void UserDatalockError() {
            Spil.Instance.fireUserDataLockError();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void UserDataError(string sErrorMessage) {
			Spil.Instance.fireUserDataError(sErrorMessage);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void UserDataAvailable() {
            Spil.Instance.fireUserDataAvailable();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void PrivacyPolicyStatus(string sAccepted) {
			Spil.Instance.firePrivacyPolicyStatus(sAccepted);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void PackagesAvailable() {
            Spil.Instance.firePackagesAvailable();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void PackagesNotAvailable() {
            Spil.Instance.firePackagesNotAvailable();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void PromotionsAvailable() {
            Spil.Instance.firePromotionsAvailable();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void PromotionsNotAvailable() {
            Spil.Instance.firePromotionsNotAvailable();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void PromotionAmountBought(string data) {
            Spil.Instance.firePromotionAmountBought(data);
		}

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void TieredEventsAvailable() {
			Spil.Instance.fireTieredEventsAvailable();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void TieredEventsNotAvailable() {
            Spil.Instance.fireTieredEventsNotAvailable();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void TieredEventUpdated(string data) {
			Spil.Instance.fireTieredEventUpdated(data);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void TieredEventProgressOpen() {
            Spil.Instance.fireTieredEventProgressOpen();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void TieredEventProgressClosed() {
            Spil.Instance.fireTieredEventProgressClosed();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void TieredEventsError(string data) {
			Spil.Instance.fireTieredEventsError(data);
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void AssetBundlesAvailable() {
            Spil.Instance.fireAssetBundlesAvailable();
        }

        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void AssetBundlesNotAvailabl() {
            Spil.Instance.fireAssetBundlesNotAvailable();
        }

#if UNITY_ANDROID
        /// <summary>
        /// This method is meant for internal use only, it should not be used by developers.
        /// Developers can subscribe to events defined in Spil.Instance.
        /// </summary>
        public void PermissionResponse(string message) {
			Spil.Instance.firePermissionResponse(message);
        }
#endif
    }
}