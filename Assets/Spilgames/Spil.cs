/*
 * Spil Games Unity SDK 2016
 * Version 2.4.0
 *
 * If you have any questions, don't hesitate to e-mail us at info@spilgames.com
 * Be sure to check the github page for documentation and the latest updates
 * https://github.com/spilgames/spil_event_unity_plugin
*/

using UnityEngine;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Helpers.GameData;
using System;
using SpilGames.Unity.Helpers.PlayerData;

namespace SpilGames.Unity {
    [HelpURL("http://www.spilgames.com/developers/integration/unity/unity-get-started/")]
    public class Spil : MonoBehaviour {
        private SpilGameDataHelper GameDataObject;
        public static SpilGameDataHelper GameData;

        private PlayerDataHelper PlayerDataObject;
        public static PlayerDataHelper PlayerData;

        [SerializeField] public bool initializeOnAwake = true;

        [SerializeField] public int invokeSeconds = 1;

        [Header("Android Settings")]
#if UNITY_ANDROID || UNITY_EDITOR
        [SerializeField] public string ProjectId = "";
#endif

        [Header("iOS Settings")] [SerializeField] public string CustomBundleId;

        [Header("Editor Settings")] [SerializeField] public bool EditorLogging = true;

        [SerializeField] public bool EditorDebugMode = true;

        [SerializeField] public string spilUserIdEditor;

        public static string SpilUserIdEditor { get; set; }

        [SerializeField] public string bundleIdEditor;

        public static string BundleIdEditor { get; private set; }

        [SerializeField] public string iapPurchaseRequestValue;

        public static string IapPurchaseRequest { get; private set; }

        [Header("Reward Settings")] [Header("Ads")] [SerializeField] private string currencyName;

        public static string CurrencyName { get; private set; }

        [SerializeField] private string currencyId;

        public static string CurrencyId { get; private set; }

        [SerializeField] private int reward = 0;

        public static int Reward { get; private set; }

        [Space(10)] [Header("Daily Bonus")] [SerializeField] private int dailyBonusId;

        public static int DailyBonusId { get; private set; }

        [SerializeField] private string dailyBonusExternalId;

        public static string DailyBonusExternalId { get; private set; }

        [SerializeField] private int dailyBonusAmount;

        public static int DailyBonusAmount { get; private set; }

        public enum DailyBonusRewardTypeEnum {
            CURRENCY,
            ITEM,
            EXTERNAL
        };

        public DailyBonusRewardTypeEnum DailyBonusRewardType;

        [Space(10)] [Header("Live Event")] [SerializeField] private int liveEventRewardId;

        public static int LiveEventRewardId { get; private set; }

        [SerializeField] private string liveEventExternalRewardId;

        public static string LiveEventExternalRewardId { get; private set; }

        [SerializeField] private int liveEventRewardAmount;

        public static int LiveEventRewardAmount { get; private set; }

        public enum LiveEventRewardTypeEnum {
            CURRENCY,
            ITEM,
            EXTERNAL
        };

        public LiveEventRewardTypeEnum LiveEventRewardType;

        [Space(10)] [Header("Token System")] [SerializeField] private string rewardToken;

        public static string RewardToken { get; private set; }

        public enum RewardFeatureTypeEnum {
            DEEPLINK,
        };

        public RewardFeatureTypeEnum RewardFeatureType;

        [SerializeField] private int tokenRewardId;

        public static int TokenRewardId { get; private set; }

        [SerializeField] private string tokenExternalRewardId;

        public static string TokenExternalRewardId { get; private set; }

        [SerializeField] private int tokenRewardAmount;

        public static int TokenRewardAmount { get; private set; }

        public enum TokenRewardTypeEnum {
            CURRENCY,
            ITEM,
            EXTERNAL
        };

        public TokenRewardTypeEnum TokenRewardType;

        public static Spil MonoInstance {
            get {
                GameObject spilSDKObject = GameObject.Find("SpilSDK");
                if (spilSDKObject == null) {
                    throw new NullReferenceException("Could not find a gameobject in the scene named \"SpilSDK\".");
                }
                return spilSDKObject.GetComponent<Spil>();
            }
        }

#if UNITY_EDITOR

        public static SpilUnityEditorImplementation Instance = new SpilUnityEditorImplementation();

#elif UNITY_ANDROID

		public static SpilAndroidUnityImplementation Instance = new SpilAndroidUnityImplementation();

#elif UNITY_IPHONE || UNITY_TVOS

		public static SpiliOSUnityImplementation Instance = new SpiliOSUnityImplementation();

#endif

        void Awake() {
            if (initializeOnAwake) {
                Initialize();
            }
        }

        public void Initialize() {
            Debug.Log("SpilSDK-Unity Init");

            Instance.SetPluginInformation(SpilUnityImplementationBase.PluginName,
                SpilUnityImplementationBase.PluginVersion);

#if UNITY_EDITOR

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

            Instance.SpilInit();
            DontDestroyOnLoad(gameObject);
            gameObject.name = "SpilSDK";

#if !UNITY_EDITOR

			GameDataObject = new SpilGameDataHelper (Instance);
			GameData = GameDataObject;

			PlayerDataObject = new PlayerDataHelper (Instance);
			PlayerData = PlayerDataObject;

#endif
        }

        public void InitEditor() {
            if (string.IsNullOrEmpty(spilUserIdEditor)) {
                spilUserIdEditor = Guid.NewGuid().ToString();
            }
            SpilUserIdEditor = spilUserIdEditor;
            Debug.Log("SpilSDK-Unity Using SpilUserIdEditor: " + SpilUserIdEditor);

            BundleIdEditor = bundleIdEditor;
            if (string.IsNullOrEmpty(bundleIdEditor)) {
                Debug.Assert(!string.IsNullOrEmpty(bundleIdEditor), "SpilSDK-Unity No BundleIdEditor set!");
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
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// Developers can subscribe to the Spil.Instance.AdStarted event.
        /// </summary>
        public void AdStart() {
            SpilUnityImplementationBase.fireAdStartedEvent();
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// Developers can subscribe to the Spil.Instance.AdFinished event.
        /// </summary>
        public void AdFinished(string response) {
            SpilUnityImplementationBase.fireAdFinishedEvent(response);
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// Developers can subscribe to the Spil.Instance.AdAvailable event.
        /// </summary>
        public void AdAvailable(string type) {
            SpilUnityImplementationBase.fireAdAvailableEvent(type);
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// Developers can subscribe to the Spil.Instance.AdNotAvailable event.
        /// </summary>
        public void AdNotAvailable(string type) {
            SpilUnityImplementationBase.fireAdNotAvailableEvent(type);
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// Developers can subscribe to the Spil.Instance.OpenParentalGate event.
        /// </summary>
        public void OpenParentalGate() {
            SpilUnityImplementationBase.fireOpenParentalGateEvent();
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// Developers can subscribe to the Spil.Instance.ConfigUpdated event.
        /// </summary>
        public void ConfigUpdated() {
            SpilUnityImplementationBase.fireConfigUpdatedEvent();
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// Developers can subscribe to the Spil.Instance.ConfigUpdated event.
        /// </summary>
        public void ConfigError(string error) {
            SpilUnityImplementationBase.fireConfigError(error);
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// </summary>
        public void OnResponseReceived(string response) {
            SpilUnityImplementationBase.OnResponseReceived(response);
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// </summary>
        public void SpilGameDataAvailable() {
            SpilUnityImplementationBase.fireSpilGameDataAvailable();
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// </summary>
        public void SpilGameDataError(string reason) {
            SpilUnityImplementationBase.fireSpilGameDataError(reason);
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// </summary>
        public void PlayerDataUpdated(string data) {
            SpilUnityImplementationBase.firePlayerDataUpdated(data);
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// </summary>
        public void PlayerDataEmptyGacha() {
            SpilUnityImplementationBase.firePlayerDataEmptyGacha();
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// </summary>
        public void GameStateUpdated(string access) {
            SpilUnityImplementationBase.fireGameStateUpdated(access);
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// </summary>
        public void OtherUsersGameStateLoaded(string message) {
            SpilUnityImplementationBase.fireOtherUsersGameStateLoaded(message);
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// </summary>
        public void SplashScreenOpen() {
            SpilUnityImplementationBase.fireSplashScreenOpen();
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// </summary>
        public void SplashScreenNotAvailable() {
            SpilUnityImplementationBase.fireSplashScreenNotAvailable();
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// </summary>
        public void SplashScreenClosed() {
            SpilUnityImplementationBase.fireSplashScreenClosed();
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// </summary>
        public void SplashScreenOpenShop() {
            SpilUnityImplementationBase.fireSplashScreenOpenShop();
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// </summary>
        public void SplashScreenData(string payload) {
            SpilUnityImplementationBase.fireSplashScreenData(payload);
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// </summary>
        public void SplashScreenError(string error) {
            SpilUnityImplementationBase.fireSplashScreenError(error);
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// </summary>
        public void DailyBonusOpen() {
            SpilUnityImplementationBase.fireDailyBonusOpen();
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// </summary>
        public void DailyBonusNotAvailable() {
            SpilUnityImplementationBase.fireDailyBonusNotAvailable();
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// </summary>
        public void DailyBonusClosed() {
            SpilUnityImplementationBase.fireDailyBonusClosed();
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// </summary>
        public void DailyBonusReward(string receivedReward) {
            SpilUnityImplementationBase.fireDailyBonusReward(receivedReward);
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// </summary>
        public void DailyBonusError(string error) {
            SpilUnityImplementationBase.fireDailyBonusError(error);
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// </summary>
        public void RewardTokenReceived(string response) {
            SpilUnityImplementationBase.fireRewardTokenReceived(response);
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// </summary>
        public void RewardTokenClaimed(string response) {
            SpilUnityImplementationBase.fireRewardTokenClaimed(response);
        }

        /// <summary>
        /// This method is called by the native Spil SDK, it should not be used by developers.
        /// </summary>
        public void RewardTokenClaimFailed(string response) {
            SpilUnityImplementationBase.fireRewardTokenClaimFailed(response);
        }

        /// <summary>
        /// This event will be called when an image has been downloaded.
        /// </summary>
        public void ImageLoadSuccess(string response) {
            SpilUnityImplementationBase.fireImageLoadSuccess(response);
        }

        /// <summary>
        /// This event will be called when an image has failed downloadibg.
        /// </summary>
        public void ImageLoadFailed(string response) {
            SpilUnityImplementationBase.fireImageLoadFailed(response);
        }

        /// <summary>
        /// This event indicates that the operation for preloading item and bundle images has been completed.
        /// </summary>
        public void ImagePreloadingCompleted() {
            SpilUnityImplementationBase.fireImagePreloadingCompleted();
        }

        /// <summary>
        /// This event indicates that the IAP has been validated with the SLOT backend.
        /// </summary>
        public void IAPValid(string data) {
            SpilUnityImplementationBase.fireIAPValid(data);
        }

        /// <summary>
        /// This event indicates that the IAP was invalid when checked with the SLOT backend.
        /// </summary>
        public void IAPInvalid(string message) {
            SpilUnityImplementationBase.fireIAPInvalid(message);
        }

        /// <summary>
        /// This event indicates that an IAP request was fired from the SDK.
        /// </summary>
        public void IAPRequestPurchase(string skuId) {
            SpilUnityImplementationBase.fireIAPRequestPurchase(skuId);
        }

        /// <summary>
        /// This event indicates that the IAP was invalid when checked with the SLOT backend.
        /// </summary>
        public void IAPServerError(string error) {
            SpilUnityImplementationBase.fireIAPServerError(error);
        }

        /// <summary>
        /// This event indicates that the IAP was invalid when checked with the SLOT backend.
        /// </summary>
        public void ServerTimeRequestSuccess(string time) {
            SpilUnityImplementationBase.fireServerTimeRequestSuccess(time);
        }

        /// <summary>
        /// This event indicates that the IAP was invalid when checked with the SLOT backend.
        /// </summary>
        public void ServerTimeRequestFailed(string error) {
            SpilUnityImplementationBase.fireServerTimeRequestFailed(error);
        }

        /// <summary>
        /// This event indicates that the Live Event Stage has been opened.
        /// </summary>
        public void LiveEventAvailable() {
            SpilUnityImplementationBase.fireLiveEventAvailable();
        }

        /// <summary>
        /// This event indicates that the Live Event Stage has been opened.
        /// </summary>
        public void LiveEventStageOpen() {
            SpilUnityImplementationBase.fireLiveEventStageOpen();
        }

        /// <summary>
        /// This event indicates that the Live Event Stage has been closed.
        /// </summary>
        public void LiveEventStageClosed() {
            SpilUnityImplementationBase.fireLiveEventStageClosed();
        }

        /// <summary>
        /// This event indicates that the Live Event is not available.
        /// </summary>
        public void LiveEventNotAvailable() {
            SpilUnityImplementationBase.fireLiveEventNotAvailable();
        }

        /// <summary>
        /// This event indicates that an error occured for the Live Event.
        /// </summary>
        public void LiveEventError(string error) {
            SpilUnityImplementationBase.fireLiveEventError(error);
        }

        /// <summary>
        /// This event indicates the reward given for the Live Event.
        /// </summary>
        public void LiveEventReward(string receivedReward) {
            SpilUnityImplementationBase.fireLiveEventReward(receivedReward);
        }

        /// <summary>
        /// This event indicates the reward given for the Live Event.
        /// </summary>
        public void LiveEventUsedExternalItems(string items) {
            SpilUnityImplementationBase.fireLiveEventReward(items);
        }

        /// <summary>
        /// This event indicates if the user met the requirements to receive the reward.
        /// </summary>
        public void LiveEventMetRequirements(string metRequirements) {
            SpilUnityImplementationBase.fireLiveEventMetRequirements(Convert.ToBoolean(metRequirements));
        }

        /// <summary>
        /// This event indicates if the Live Event was completed by the user.
        /// </summary>
        public void LiveEventCompleted() {
            SpilUnityImplementationBase.fireLiveEventCompleted();
        }

        /// <summary>
        /// This event indicates if the Social Login was completed by the user.
        /// </summary>
        public void LoginSuccessful(string message) {
            SpilUnityImplementationBase.fireLoginSuccessful(message);
        }

        /// <summary>
        /// This event indicates if the Social Login failed.
        /// </summary>
        public void LoginFailed(string error) {
            SpilUnityImplementationBase.fireLoginFailed(error);
        }

        /// <summary>
        /// This event indicates if Social Login should be initiated.
        /// </summary>
        public void RequestLogin() {
            SpilUnityImplementationBase.fireRequestLogin();
        }

        /// <summary>
        /// This event indicates if the Social Logout was completed by the user.
        /// </summary>
        public void LogoutSuccessful() {
            SpilUnityImplementationBase.fireLogoutSuccessful();
        }

        /// <summary>
        /// This event indicates if the Social Logout failed.
        /// </summary>
        public void LogoutFailed(string error) {
            SpilUnityImplementationBase.fireLogoutFailed(error);
        }

        /// <summary>
        /// This event indicates if an authentication error has occured on any event after the Social Login.
        /// </summary>
        public void AuthenticationError(string error) {
            SpilUnityImplementationBase.fireAuthenticationError(error);
        }

		/// <summary>
		/// This event indicates if a merge conflict occured
		/// </summary>
		public void UserDataMergeConflict(string data) {
			SpilUnityImplementationBase.fireUserDataMergeConflict(data);
		}

		/// <summary>
		/// This event indicates if a merge conflict was successfully resolved.
		/// </summary>
		public void UserDataMergeSuccessful() {
			SpilUnityImplementationBase.fireUserDataMergeSuccessful();
		}

		/// <summary>
		/// This event indicates if a merge conflict failed.
		/// </summary>
		public void UserDataMergeFailed(string mergeData, string mergeType) {
			SpilUnityImplementationBase.fireUserDataMergeFailed(mergeData, mergeType);
		}

		/// <summary>
		/// This event indicates if a merge conflict needs to be handled for a certain type.
		/// </summary>
		public void UserDataHandleMerge(string mergeType) {
			SpilUnityImplementationBase.fireUserDataHandleMerge(mergeType);
		}

		/// <summary>
		/// This event indicates if a userdata sync error occured.
		/// </summary>
		public void UserDataSyncError() {
			SpilUnityImplementationBase.fireUserDataSyncError();
		}

		/// <summary>
		/// This event indicates if a userdata lock occured.
		/// </summary>
		public void UserDatalockError() {
			SpilUnityImplementationBase.fireUserDatalockError();
		}

		/// <summary>
		/// This event indicates if a general userdata error occured.
		/// </summary>
		public void UserDataError(string errorMessage) {
			SpilUnityImplementationBase.fireUserDataError(errorMessage);
		}

		/// <summary>
		/// This event indicates if an authentication error has occured on any event after the Social Login.
		/// </summary>
		public void UserDataAvailable() {
			SpilUnityImplementationBase.fireUserDataAvailable();
		}

#if UNITY_ANDROID
        /// <summary>
        /// This event indicates the status of the permission request.
        /// Only Android.
        /// </summary>
        public void PermissionResponse(string message) {
            SpilUnityImplementationBase.firePermissionResponse(message);
        }
#endif
    }
}