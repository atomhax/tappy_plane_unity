using System;
using System.Collections.Generic;
using UnityEngine;
using SpilGames.Unity.Helpers;
using SpilGames.Unity.Helpers.GameData;
using SpilGames.Unity.Helpers.PlayerData;
using SpilGames.Unity.Json;
using SpilGames.Unity.Base.SDK;

namespace SpilGames.Unity.Base.Implementations {
#if UNITY_ANDROID
    public class SpilAndroidUnityImplementation : SpilUnityImplementationBase {
        #region Inherited members

        public override void SetPluginInformation(string PluginName, string PluginVersion) {
            CallNativeMethod("setPluginInformation", new object[] {
                PluginName,
                PluginVersion
            }, true);
        }

        #region Game config

        /// <summary>
        /// Returns the game config as a json string.
        /// This is not essential for developers so could be made private (getConfig T () uses it so it cannot be removed entirely) but might be handy for some developers so we left it in.
        /// </summary>
        /// <returns></returns>     
        public override string GetConfigAll() {
            return CallNativeMethod("getConfigAll");
        }

        /// <summary>
        /// Method that returns a configuration value from the game config based on key 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override string GetConfigValue(string key) {
            return CallNativeMethod("getConfigValue", key, true);
        }

        #endregion

        #region Packages and promotions

        /// <summary>
        /// Method that requests packages and promotions from the server.
        /// This is called automatically by the Spil SDK when the game starts.
        /// This is not essential so could be removed but might be handy for some developers so we left it in.
        /// </summary>
        public override void RequestPackages() {
            CallNativeMethod("requestPackages");
        }

        // Method that returns the all packages
        protected override string GetAllPackages() {
            Debug.Log("Get All Packages: " + CallNativeMethod("getAllPackages"));
            return CallNativeMethod("getAllPackages");
        }

        // Method that returns a package based on key
        protected override string GetPackage(string key) {
            Debug.Log("GetPackage: " + CallNativeMethod("getPackage", key, true));
            return CallNativeMethod("getPackage", key, true);
        }

        #endregion

        /// <summary>
        /// This method is marked as internal and should not be exposed to developers.
        /// The Spil Unity SDK is not packaged as a seperate assembly yet so this method is currently visible, this will be fixed in the future.
        /// Internal method names start with a lower case so you can easily recognise and avoid them.
        /// </summary>
        internal override void SpilInit(bool withPrivacyPolicy) {
#if UNITY_ANDROID
            Spil spil = GameObject.FindObjectOfType<Spil>();
            CallNativeMethod("init", new object[] {withPrivacyPolicy}, true);
            RegisterDevice(spil.ProjectId); 
            RequestPackages();
#endif
        }

        internal override void CheckPrivacyPolicy() {
            CallNativeMethod("checkPrivacyPolicy");
        }

        public override void ShowPrivacyPolicySettings() {
            if (!Spil.CheckPrivacyPolicy) {
                Debug.Log("Privacy Policy not enabled. Will not show privacy policy settings screen");
                return;
            }
            
            if (Spil.UseUnityPrefab) {
                PrivacyPolicyHelper.PrivacyPolicyObject = (GameObject) GameObject.Instantiate(Resources.Load("Spilgames/PrivacyPolicy/PrivacyPolicyUnity" + Spil.MonoInstance.PrefabOrientation));
                PrivacyPolicyHelper.PrivacyPolicyObject.SetActive(true);
            
                PrivacyPolicyHelper.Instance.ShowSettingsScreen(1);
            } else {
                CallNativeMethod("showPrivacyPolicySettings");
            }    
        }

        public override void SavePrivValue(int priv) {
            CallNativeMethod("savePrivValue", new object[] {priv}, true);
        }

        public override int GetPrivValue() {
            return Convert.ToInt32(CallNativeMethod("getPrivValue"));
        }

        public override void SetUserId(string providerId, string userId) {
            CallNativeMethod("setUserId", new object[] {providerId, userId}, true);
        }

        public override string GetUserId() {
            return CallNativeMethod("getUserId");
        }

        public override string GetDeviceId() {
            return CallNativeMethod("getDeviceId");
        }

        public override void SetCustomBundleId(string bundleId) {
            // TODO
        }

        /// <summary>
        /// Gets the user provider.
        /// </summary>
        /// <returns>The user provider native.</returns>
        public override string GetUserProvider() {
            return CallNativeMethod("getUserProvider");
        }

        /// <summary>
        /// Sets the state of the private game.
        /// </summary>
        /// <param name="privateData">Private data.</param>
        public override void SetPrivateGameState(string privateData) {
            CallNativeMethod("setPrivateGameState", new object[] {
                privateData
            }, true);
        }

        /// <summary>
        /// Gets the state of the private game.
        /// </summary>
        /// <returns>The private game state.</returns>
        public override string GetPrivateGameState() {
            return CallNativeMethod("getPrivateGameState");
        }

        /// <summary>
        /// Sets the public game state.
        /// </summary>
        /// <param name="publicData">Public data.</param>
        public override void SetPublicGameState(string publicData) {
            CallNativeMethod("setPublicGameState", new object[] {
                publicData
            }, true);
        }

        /// <summary>
        /// Sets the public game state.
        /// </summary>
        /// <returns>The public game state.</returns>
        public override string GetPublicGameState() {
            return CallNativeMethod("getPublicGameState");
        }

        /// <summary>
        /// Gets the public game state of other users.
        /// </summary>
        /// <param name="provider">Provider.</param>
        /// <param name="userIdsJsonArray">User identifiers json array.</param>
        public override void GetOtherUsersGameState(string provider, string userIdsJsonArray) {
            CallNativeMethod("getOtherUsersGameState", new object[] {
                provider,
                userIdsJsonArray
            }, true);
        }

        /// <summary>
        /// Sends an event to the native Spil SDK which will send a request to the back-end.
        /// Custom events may be tracked as follows:
        /// To track an simple custom event, simply call Spil.Instance.SendCustomEvent(String eventName); from anywhere in your code.
        /// To pass more information with the event create a &lt;String, String&gt; Dictionary and pass that as the second parameter like so:
        /// Dictionary&lt;String, String&gt; eventParams = new Dictionary&lt;String,String&gt;();
        /// eventParams.Add(“level”,levelName);
        /// Spil.Instance.SendCustomEvent(“PlayerDeath”, eventParams);
        /// See https://github.com/spilgames/spil_event_unity_plugin for more information on events.
        /// This method was previously called "TrackEvent".
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="dict"></param>
        internal override void SendCustomEventInternal(string eventName, Dictionary<string, object> dict) {
            Debug.Log("SpilSDK-Unity SendCustomEvent " + eventName);

            if (eventName.Equals("updatePlayerData") && dict.ContainsKey("inventory") &&
                dict["inventory"] is Dictionary<string, object>) {
                Dictionary<string, object> inventory = (Dictionary<string, object>) dict["inventory"];
                string inventoryAsString = JsonHelper.DictToJSONObject(inventory).ToString().Replace("\"", "\\\"");
                dict["inventory"] = inventoryAsString;
            }
            if (eventName.Equals("updatePlayerData") && dict.ContainsKey("wallet") &&
                dict["wallet"] is Dictionary<string, object>) {
                Dictionary<string, object> wallet = (Dictionary<string, object>) dict["wallet"];
                string walletAsString = JsonHelper.DictToJSONObject(wallet).ToString().Replace("\"", "\\\"");
                dict["wallet"] = walletAsString;
            }

            string parameters = null;
            if (dict != null) {
                parameters = JsonHelper.DictToJSONObject(dict).ToString();
            }

            CallNativeMethod("trackEvent", new object[] {
                eventName,
                parameters
            }, true);
        }

        /// <summary>
        /// This can be called to show a video, for instance after calling "SendrequestRewardVideoEvent()"
        /// and receiving an "AdAvailable" event the developer could call this method from the event handler.
        /// When calling this method "SendrequestRewardVideoEvent()" must first have been called to request and cache a video.
        /// If no video is available then nothing will happen.
        /// </summary>
        public override void PlayVideo(string location = null, string rewardType = null) {
            int priv = Spil.Instance.GetPrivValue();

            if (priv < 2 && priv > -1 && Spil.UseUnityPrefab) {
                ShowAdsScreen();
                return;
            }
            
            CallNativeMethod("playVideo", new object[] {
                location,
                rewardType
            }, true);
        }

        /// <summary>
        /// This can be called to show the "more apps" activity, for instance after calling "RequestMoreApps()"
        /// and receiving an "AdAvailable" event the developer could call this method from the event handler.
        /// When calling this method "RequestMoreApps()" must first have been called to request and cache a video.
        /// If no video is available then nothing will happen.
        /// </summary>
        public override void PlayMoreApps() {
            CallNativeMethod("playMoreApps");
        }

        /// <summary>
        /// Sends the "requestAd" event with the "moreApps" parameter to the native Spil SDK which will send a request to the back-end.
        /// When a response has been received from the back-end the SDK will fire either an "AdAvailable" or and "AdNotAvailable"
        /// event to which the developer can subscribe and for instance call PlayVideo(); or PlayMoreApps();
        /// </summary>
        public override void RequestMoreApps() {
            CallNativeMethod("requestMoreApps");
        }

        /// <summary>
        /// Sends the "requestRewardVideo" event to the native Spil SDK which will send a request to the back-end.
        /// When a response has been received from the back-end the SDK will fire either an "AdAvailable" or and "AdNotAvailable"
        /// event to which the developer can subscribe and for instance call PlayVideo();
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        public override void RequestRewardVideo(string location = null, string rewardType = null) {
            int priv = Spil.Instance.GetPrivValue();

            if (priv < 2 && priv > -1 && Spil.UseUnityPrefab) {
                fireAdAvailableEvent("rewardVideo");
                return;
            }
            
            CallNativeMethod("requestRewardVideo", new object[] {
                location,
                rewardType
            }, true);
        }

        /// <summary>
        /// Retrieves the Spil User Id so that developers can show this in-game for users.
        /// If users contact Spil customer service they can supply this Id so that 
        /// customer support can help them properly. Please make this Id available for users
        /// in one of your game's screens.
        /// </summary>
        public override string GetSpilUserId() {
            return CallNativeMethod("getSpilUserId");
        }

        /// <summary>
        /// Call to inform the SDK that the parental gate was (not) passes
        /// </summary>
        public override void ClosedParentalGate(bool pass) {
        }

        #region Spil Game Objects

        public override string GetSpilGameDataFromSdk() {
            return CallNativeMethod("getSpilGameData");
        }

        public override void RequestPromotions() {
            CallNativeMethod("requestPromotions");
        }

        public override string GetAllPromotions() {
            return CallNativeMethod("getAllPromotions");
        }

        public override string GetBundlePromotion(int bundleId) {
            return CallNativeMethod("getBundlePromotion", new object[] {
                bundleId
            }, true);
        }

        public override string GetPackagePromotion(string packageId) {
            return CallNativeMethod("getPackagePromotion", new object[] {
                packageId
            }, true);
        }

        public override void ShowPromotionScreen(int promotionId) {
            CallNativeMethod("showPromotionScreen", new object[] {
                promotionId
            }, true);
        }

        #endregion

        #region AssetBundles

        public override void RequestAssetBundles() {
            CallNativeMethod("requestAssetBundles");
        }

        public override string GetAllAssetBundles() {
            return CallNativeMethod("getAssetBundles");
        }

        #endregion
        
        #region Player Data

        public override void UpdatePlayerData() {
            CallNativeMethod("updatePlayerData");
        }

        public override string GetWalletFromSdk() {
            return CallNativeMethod("getWallet");
        }

        public override string GetInvetoryFromSdk() {
            return CallNativeMethod("getInventory");
        }

        public override void AddCurrencyToWallet(int currencyId, int amount, string reason, string location,
            string reasonDetails = null, string transactionId = null) {
            CallNativeMethod("addCurrencyToWallet", new object[] {
                currencyId,
                amount,
                reason,
                location,
                reasonDetails,
                transactionId
            }, true);
        }

        public override void SubtractCurrencyFromWallet(int currencyId, int amount, string reason, string location,
            string reasonDetails = null, string transactionId = null) {
            CallNativeMethod("subtractCurrencyFromWallet", new object[] {
                currencyId,
                amount,
                reason,
                location,
                reasonDetails,
                transactionId
            }, true);
        }

        public override void AddItemToInventory(int itemId, int amount, string reason, string location,
            string reasonDetails = null, string transactionId = null) {
            CallNativeMethod("addItemToInventory", new object[] {
                itemId,
                amount,
                reason,
                location,
                reasonDetails,
                transactionId
            }, true);
        }

        public override void SubtractItemFromInventory(int itemId, int amount, string reason, string location,
            string reasonDetails = null, string transactionId = null) {
            CallNativeMethod("subtractItemFromInventory", new object[] {
                itemId,
                amount,
                reason,
                location,
                reasonDetails,
                transactionId
            }, true);
        }

        public override void BuyBundle(int bundleId, string reason, string location, string reasonDetails = null,
            string transactionId = null) {
            CallNativeMethod("buyBundle", new object[] {
                bundleId,
                reason,
                location,
                reasonDetails,
                transactionId
            }, true);
        }

        public override void OpenGacha(int gachaId, string reason, string location, string reasonDetails = null) {
            CallNativeMethod("openGacha", new object[] {
                gachaId,
                reason,
                location,
                reasonDetails
            }, true);
        }

        public override void ResetPlayerData() {
            CallNativeMethod("resetPlayerData");
        }

        public override void ResetInventory() {
            CallNativeMethod("resetInventory");
        }

        public override void ResetWallet() {
            CallNativeMethod("resetWallet");
        }

        #endregion

        #region Image loading

        /// <summary>
        /// Used to get the image from the cache, based on the url provided.
        /// </summary>
        public override string GetImagePath(string url) {
            return CallNativeMethod("getImagePath", new object[] {
                url
            }, true);
        }

        /// <summary>
        /// Used to get the image from the cache, based on the url provided. ImageContext will be imageType = custom when it's not provided as parameter
        /// </summary>
        public override void RequestImage(string url, int id, string imageType) {
            CallNativeMethod("requestImage", new object[] {
                url,
                id,
                imageType
            }, true);
        }

        /// <summary>
        /// Clears the cache, useful in case when a lot of items have been updated.
        /// </summary>
        public override void ClearDiskCache() {
            CallNativeMethod("clearDiskCache");
        }

        /// <summary>
        /// This method loops through all the items and bundles and adds urls to images (if any) to a download queue if those images have not yet been download and saved to local storage.
        /// </summary>
        public override void PreloadItemAndBundleImages() {
            CallNativeMethod("preloadItemAndBundleImages");
        }

        #endregion

        #endregion

        #region Non inherited members (Android only members)

        #region DFP / Fyber / Chartboost

        /// <summary>
        /// Method that initiaties DFP Ads (to be used only for testing purposes).
        /// This is not essential for developers so could be hidden but it might be handy for some developers so we left it in.
        /// </summary>
        /// <param name="adUnitId"></param>
        public void TestStartDFP(string adUnitId) {
            CallNativeMethod("startDFP", adUnitId, true);
        }

        /// <summary>
        /// Method that initiaties Fyber Ads (to be used only for testing purposes).
        /// This is not essential for developers so could be hidden but it might be handy for some developers so we left it in.
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="token"></param>
        public void TestStartFyber(string appId, string token) {
            CallNativeMethod("startFyber", new object[] {
                appId,
                token
            }, true);
        }

        /// <summary>
        /// Method that shows Chartboost more apps (to be used only for testing purposes).
        /// This is not essential for developers so could be hidden but it might be handy for some developers so we left it in.
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSignature"></param>
        public void TestStartChartBoost(string appId, string appSignature) {
            CallNativeMethod("startChartboost", new object[] {
                appId,
                appSignature
            }, true);
        }

        /// <summary>
        /// Method that requests ads (to be used only for testing purposes).
        /// This is not essential for developers so could be hidden but it might be handy for some developers so we left it in.
        /// Use SendrequestRewardVideoEvent() if you want to request an ad!
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="adType"></param>
        /// <param name="parentalGate"></param>
        public override void TestRequestAd(string provider, string adType, bool parentalGate) {
            CallNativeMethod("requestAd", new object[] {
                provider,
                adType,
                parentalGate
            }, true);
        }

        #endregion

        #region Server Time

        public override void RequestServerTime() {
            CallNativeMethod("requestServerTime");
        }

        #endregion

        #region Live Event

        public override void RequestLiveEvent() {
            CallNativeMethod("requestLiveEvent");
        }

        public override void OpenLiveEvent() {
            CallNativeMethod("openLiveEvent");
        }

        public override string GetLiveEventConfig() {
            return CallNativeMethod("getLiveEventConfig");
        }

        public override long GetLiveEventStartDate() {
            return Convert.ToInt64(CallNativeMethod("getLiveEventStartDate"));
        }

        public override long GetLiveEventEndDate() {
            return Convert.ToInt64(CallNativeMethod("getLiveEventEndDate"));
        }

        #endregion

        #region Tiered Events

        public override void RequestTieredEvents() {
            CallNativeMethod("requestTieredEvents");
        }

        public override List<TieredEvent> GetAllTieredEvents() {
            string tieredEventsJson = CallNativeMethod("getAllTieredEvents");
            if (tieredEventsJson != null) {
                Debug.Log(tieredEventsJson);
                return JsonHelper.getObjectFromJson<List<TieredEvent>>(tieredEventsJson);
            }
            return null;
        }

        public override TieredEventProgress GetTieredEventProgress(int tieredEventId) {
            string tieredEventProgressJson = CallNativeMethod("getTieredEventProgress", new object[] {
                tieredEventId
            }, true);

            if (tieredEventProgressJson == null) {
                return null;
            }
            
            Debug.Log(tieredEventProgressJson);
            return JsonHelper.getObjectFromJson<TieredEventProgress>(tieredEventProgressJson);
        }

        public override void ShowTieredEventProgress(int tieredEventId) {
            CallNativeMethod("showTieredEventProgress", new object[] {
                tieredEventId
            }, true);
        }

        #endregion

        #region Social Login

        public override void UserLogin(string socialId, string socialProvider, string socialToken) {
            CallNativeMethod("userLogin", new object[] {
                socialId,
                socialProvider,
                socialToken
            }, true);
        }

        public override void UserLogout(bool global) {
            CallNativeMethod("userLogout", new object[] {
                global
            }, true);
        }

        public override void UserPlayAsGuest() {
            CallNativeMethod("userPlayAsGuest");
        }

        public override void ShowUnauthorizedDialog(string title, string message, string loginText,
            string playAsGuestText) {
            CallNativeMethod("showUnauthorizedDialog", new object[] {
                title,
                message,
                loginText,
                playAsGuestText
            }, true);
        }

        public override bool IsLoggedIn() {     
            return Convert.ToBoolean(CallNativeMethod("isLoggedIn"));
        }

        #endregion
        
        #region Userdata syncing

        public override void RequestUserData() {
            CallNativeMethod("requestUserData");
        }

        public override void MergeUserData(string mergeData, string mergeType) {
            CallNativeMethod("mergeUserData", new object[] {
                mergeData,
                mergeType,
            }, true);
        }

        public override void ShowMergeConflictDialog(string title, string message, string localButtonText, string remoteButtonText, string mergeButtonText = null) {
            CallNativeMethod("showMergeConflictDialog", new object[] {
                title,
                message,
                localButtonText,
                remoteButtonText,
                mergeButtonText
            }, true);
        }

        public override void ShowSyncErrorDialog(string title, string message, string startMergeButtonText) {
            CallNativeMethod("showSyncErrorDialog", new object[] {
                title,
                message,
                startMergeButtonText
            }, true);
        }

        public override void ShowMergeFailedDialog(string title, string message, string retryButtonText, string mergeData, string mergeType) {
            CallNativeMethod("showSyncErrorDialog", new object[] {
                title,
                message,
                retryButtonText,
                mergeData,
                mergeType
            }, true);
        }
        
        #endregion

        public override void ResetData() {
            CallNativeMethod("resetData");
        }

        public override void ShowNativeDialog(string title, string message, string buttonText) {
            CallNativeMethod("showNativeDialog", new object[] {
                title,
                message,
                buttonText
            }, true);
        }

        #region Push notifications

        /// <summary>
        /// Enables push notification for Android.
        /// Be sure to fill in the correct Project_ID in Spil.cs
        /// </summary>
        public void EnablePushNotifications() {
            CallNativeMethod("enableNotifications");
        }

        /// <summary>
        /// Disables push notification for Android.
        /// </summary>
        public void DisablePushNotifications() {
            CallNativeMethod("disableNotification");
        }

        #endregion

        // If Android, register device
        private void RegisterDevice(string projectID) {
            CallNativeMethod("registerDevice", projectID, true);
        }

        /// <summary>
        /// Used only by the Test app to check that calls were successfully made.
        /// This method is marked as "internal" and should not be exposed to developers.
        /// The Spil Unity SDK is not packaged as a seperate assembly yet so this method is currently visible, this will be fixed in the future.
        /// Internal method names start with a lower case so you can easily recognise and avoid them.
        /// </summary>
        internal string clearLog() {
            return CallNativeMethod("clearLog");
        }

        /// <summary>
        /// Used only by the Test app to check that calls were successfully made.
        /// This method is marked as internal and should not be exposed to developers.
        /// The Spil Unity SDK is not packaged as a seperate assembly yet so this method is currently visible, this will be fixed in the future.
        /// Internal method names start with a lower case so you can easily recognise and avoid them.
        /// </summary>
        internal string getLog() {
            return CallNativeMethod("getLog");
        }

        private string CallNativeMethod(string methodName) {
            return CallNativeMethod<object>(methodName, null, false);
        }

        /// <summary>
        /// Calls a native method with the given method name. When using param1 T does not need to be supplied, the type of the object passed as param1 will be used automatically.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="param1"></param>
        /// <param name="useParam1"></param>
        /// <returns></returns>
        private string CallNativeMethod<T>(string methodName, T param1 = null, bool useParam1 = false) where T : class {
            Debug.Log("SpilSDK-Unity CallNativeMethod " + methodName +
                      (param1 != null ? " param: " + param1.ToString() : ""));

            string value = null;
            using (AndroidJavaClass pClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                if (pClass != null) {
                    AndroidJavaObject instance = pClass.GetStatic<AndroidJavaObject>("currentActivity");
                    if (useParam1) {
                        // Required because apparently when object[] is passed as T instance.Call() cannot seem to find the right method signature (it assumes T is Object instead of object[] ??)
                        // TODO: Test if other types passed as T do work correctly.
                        object[] realParam = null;
                        if (param1 is object[]) {
                            realParam = (object[]) ((object) param1);
                        }
                        if (realParam != null) {
                            try {
                                value = instance.Call<string>(methodName, realParam);
                            }
                            catch (AndroidJavaException) {
                                // Method is probably a void method, try calling it without return type
                                // TODO: This could be confusing and return unexpected results? Could make seperate method for void method calls?
                                instance.Call(methodName, realParam);
                            }
                        }
                        else {
                            try {
                                value = instance.Call<string>(methodName, param1);
                            }
                            catch (AndroidJavaException) {
                                // Method is probably a void method, try calling it without return type
                                // TODO: This could be confusing and return unexpected results? Could make seperate method for void method calls?
                                instance.Call(methodName, param1);
                            }
                        }
                    }
                    else {
                        try {
                            value = instance.Call<string>(methodName);
                        }
                        catch (AndroidJavaException) {
                            // Method is probably a void method, try calling it without return type
                            // TODO: This could be confusing and return unexpected results? Could make seperate method for void method calls?
                            instance.Call(methodName);
                        }
                    }
                }
            }
            return value;
        }

        #endregion

        #region Customer support

        public override void ShowHelpCenterWebview(string url) {
            CallNativeMethod("showZendeskWebViewHelpCenter", new object[] {
                url,
            }, true);
        }

        #endregion

        #region Web

        public override void RequestDailyBonus() {
            CallNativeMethod("requestDailyBonus");
        }

        public override void RequestSplashScreen(string type) {
            CallNativeMethod("requestSplashScreen", new object[] {
                type,
            }, true);
        }

        #endregion

        #region Reward

        public override void ClaimToken(string token, string rewardType) {
            CallNativeMethod("claimToken", new object[] {
                token,
                rewardType
            }, true);
        }

        #endregion

        #region Permissions

        public void RequestDangerousPermission(string permission, string rationale, string denyRationale) {
            CallNativeMethod("requestDangerousPermission", new object[] {
                permission,
                rationale,
                denyRationale
            }, true);
        }

        public class Permissions {
            public static string READ_CALENDAR = "android.permission.READ_CALENDAR";
            public static string WRITE_CALENDAR = "android.permission.WRITE_CALENDAR";
            public static string CAMERA = "android.permission.CAMERA";
            public static string READ_CONTACTS = "android.permission.READ_CONTACTS";
            public static string WRITE_CONTACTS = "android.permission.WRITE_CONTACTS";
            public static string GET_ACCOUNTS = "android.permission.GET_ACCOUNTS";
            public static string ACCESS_FINE_LOCATION = "android.permission.ACCESS_FINE_LOCATION";
            public static string ACCESS_COARSE_LOCATION = "android.permission.ACCESS_COARSE_LOCATION";
            public static string RECORD_AUDIO = "android.permission.RECORD_AUDIO";
            public static string READ_PHONE_STATE = "android.permission.READ_PHONE_STATE";
            public static string CALL_PHONE = "android.permission.CALL_PHONE";
            public static string SEND_SMS = "android.permission.SEND_SMS";
            public static string RECEIVE_SMS = "android.permission.RECEIVE_SMS";
            public static string READ_SMS = "android.permission.READ_SMS";
            public static string WRITE_EXTERNAL_STORAGE = "android.permission.WRITE_EXTERNAL_STORAGE";
            public static string READ_EXTERNAL_STORAGE = "android.permission.READ_EXTERNAL_STORAGE";
        }

        public class PermissionResponseObject {
            public string permission;
            public bool granted;
            public bool permanentlyDenied;
        }

        #endregion

        #region Environemnt Changing

        /// <summary>
        /// Sets the production environment for SLOT.
        /// Internal purposes only.
        /// </summary>
        public void SetProductionEnvironment() {
            CallNativeMethod("setProductionEnvironment");
        }

        /// <summary>
        /// Sets the staging environment for SLOT.
        /// Internal purposes only.
        /// Not to be confused with Production Testing Environment.
        /// </summary>
        public void SetStagingEnvironment() {
            CallNativeMethod("setStagingEnvironment");
        }

        #endregion
    }
#endif
}