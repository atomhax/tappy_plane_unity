#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;
using SpilGames.Unity.Helpers;
using System;
using SpilGames.Unity.Base.UnityEditor.Responses;
using SpilGames.Unity.Json;
using SpilGames.Unity.Base.UnityEditor;
using SpilGames.Unity.Helpers.GameData;
using SpilGames.Unity.Helpers.PlayerData;

namespace SpilGames.Unity.Base.Implementations {
    public class SpilUnityEditorImplementation : SpilUnityImplementationBase {
        public static PlayerDataResponse pData;
        public static GameDataResponse gData;

        public static bool unauthorized = false;
        public static string spilToken;

        #region Inherited members

        public override void SetPluginInformation(string PluginName, string PluginVersion) {
            Response.pluginName = PluginName;
            Response.pluginVersion = PluginVersion;
        }

        #region Game config

        /// <summary>
        /// Returns the game config as a json string.
        /// This is not essential for developers so could be made private (getConfig T () uses it so it cannot be removed entirely) but might be handy for some developers so we left it in.
        /// </summary>
        /// <returns></returns>     
        public override string GetConfigAll() {
            return ConfigResponse.getConfigAll();
        }

        /// <summary>
        /// Method that returns a configuration value from the game config based on key 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override string GetConfigValue(string key) {
            return ConfigResponse.getConfigValue(key);
        }

        #endregion

        #region Packages and promotions

        /// <summary>
        /// Method that requests packages and promotions from the server.
        /// This is called automatically by the Spil SDK when the game starts.
        /// This is not essential so could be removed but might be handy for some developers so we left it in.
        /// </summary>
        public override void UpdatePackagesAndPromotions() {
            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "requestPackages";
            spilEvent.Send();
        }

        protected override string GetAllPackages() {
            return JsonHelper.getJSONFromObject(PackagesResponse.GamePackagesData);
        }

        protected override string GetPackage(string key) {
            return PackagesResponse.getPackage(key);
        }

        /// <summary>
        /// This method is marked as internal and should not be exposed to developers.
        /// The Spil Unity SDK is not packaged as a seperate assembly yet so this method is currently visible, this will be fixed in the future.
        /// Internal method names start with a lower case so you can easily recognise and avoid them.
        /// </summary>
        internal override string GetPromotions(string key) {
            return PackagesResponse.getPromotions(key);
        }

        #endregion

        /// <summary>
        /// This method is marked as internal and should not be exposed to developers.
        /// The Spil Unity SDK is not packaged as a seperate assembly yet so this method is currently visible, this will be fixed in the future.
        /// Internal method names start with a lower case so you can easily recognise and avoid them.
        /// </summary>
        internal override void SpilInit() {
            gData = new GameDataResponse();
            pData = new PlayerDataResponse();

            SetPluginInformation(PluginName, PluginVersion);

            RequestConfig();
            RequestGameData();
            RequestPlayerData();
            RequestGameState();
            AdvertisementInit();
            UpdatePackagesAndPromotions();
        }

        public override void ResetData() {
            gData = null;
            Spil.GameData = null;
            pData = null;
            Spil.PlayerData = null;
            
            SpilInit();
        }

        public override void ShowNativeDialog(string title, string message, string buttonText) {
            
        }

        internal void RequestConfig() {
            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "requestConfig";

            spilEvent.Send();
        }

        internal void RequestGameData() {
            gData = gData.InitialiseGameObjects();

            Spil.GameData = new SpilGameDataHelper(this);

            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "requestGameData";

            spilEvent.Send();
        }

        internal void RequestPlayerData() {
            pData.Wallet = pData.InitWallet();
            pData.Inventory = pData.InitInventory();

            Spil.PlayerData = new PlayerDataHelper(this);

            pData.SetInitalWalletValues();
            pData.SetInitalInventoryValues();

            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "requestPlayerData";

            JSONObject j1 = new JSONObject();
            j1.AddField("offset", pData.Wallet.offset);
            spilEvent.customData.AddField("wallet", j1);

            JSONObject j2 = new JSONObject();
            j2.AddField("offset", pData.Inventory.offset);
            spilEvent.customData.AddField("inventory", j2);

            spilEvent.Send();
        }

        internal void RequestGameState() {
            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "requestMyGameState";

            JSONObject json = new JSONObject(JSONObject.Type.ARRAY);
            json.Add("private");

            if (Response.externalId != null && Response.provider != null) {
                json.Add("public");
            }

            spilEvent.customData.AddField("access", json);

            spilEvent.Send();
        }

        internal void AdvertisementInit() {
            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "advertisementInit";

            spilEvent.Send();
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
        public override void SendCustomEvent(string eventName, Dictionary<string, object> dict) {
            SpilLogging.Log("SpilSDK-Unity SendCustomEvent " + eventName);
            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = eventName;

            if (dict != null) {
                spilEvent.customData = JsonHelper.DictToJSONObject(dict);
                if (dict.ContainsKey("trackingOnly")) {
                    spilEvent.customData.RemoveField("trackingOnly");
                    spilEvent.customData.AddField("trackingOnly", true);
                    dict.Remove("trackingOnly");
                }
//                foreach (KeyValuePair<string, object> dictValue in dict) {
//                    if (dictValue.Key.Equals("wallet") || dictValue.Key.Equals("inventory")) {
//                        JSONObject json = new JSONObject((string) dictValue.Value);
//                        spilEvent.customData.AddField(dictValue.Key, json);
//                    } else {
//                        if (dictValue.Value is int) {
//                            spilEvent.customData.AddField(dictValue.Key, (int) dictValue.Value);
//                        } else if (dictValue.Value is string) {
//                            spilEvent.customData.AddField(dictValue.Key, dictValue.Value.ToString());
//                        } else if (dictValue.Value is bool) {
//                            spilEvent.customData.AddField(dictValue.Key, (bool) dictValue.Value);
//                        } else if (dictValue.Value is float) {
//                            spilEvent.customData.AddField(dictValue.Key, (float) dictValue.Value);
//                        } else if (dictValue.Value is JSONObject) {
//                            spilEvent.customData.AddField(dictValue.Key, (JSONObject) dictValue.Value);
//                        } else {
//                            spilEvent.customData.AddField(dictValue.Key, dictValue.Value.ToString());
//                        }
//                    }
//                }
            }

            spilEvent.Send();
        }

        /// <summary>
        /// This can be called to show a video, for instance after calling "SendrequestRewardVideoEvent()"
        /// and receiving an "AdAvailable" event the developer could call this method from the event handler.
        /// When calling this method "SendrequestRewardVideoEvent()" must first have been called to request and cache a video.
        /// If no video is available then nothing will happen.
        /// </summary>
        public override void PlayVideo(string location = null, string rewardType = null) {
            AdvertisementResponse.PlayVideo();
        }

        /// <summary>
        /// This can be called to show the "more apps" activity, for instance after calling "RequestMoreApps()"
        /// and receiving an "AdAvailable" event the developer could call this method from the event handler.
        /// When calling this method "RequestMoreApps()" must first have been called to request and cache a video.
        /// If no video is available then nothing will happen.
        /// </summary>
        public override void PlayMoreApps() {
            AdvertisementResponse.PlayMoreApps();
        }

        /// <summary>
        /// Sends the "requestAd" event with the "moreApps" parameter to the native Spil SDK which will send a request to the back-end.
        /// When a response has been received from the back-end the SDK will fire either an "AdAvailable" or and "AdNotAvailable"
        /// event to which the developer can subscribe and for instance call PlayVideo(); or PlayMoreApps();
        /// </summary>
        public override void RequestMoreApps() {
            SpilUnityImplementationBase.fireAdAvailableEvent("moreApps");
        }

        /// <summary>
        /// Sends the "requestRewardVideo" event to the native Spil SDK which will send a request to the back-end.
        /// When a response has been received from the back-end the SDK will fire either an "AdAvailable" or and "AdNotAvailable"
        /// event to which the developer can subscribe and for instance call PlayVideo();
        /// See http://www.spilgames.com/developers/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/ for more information on events.
        /// </summary>
        public override void RequestRewardVideo(string location = null, string rewardType = null) {
            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "requestRewardVideo";

            if (location != null) {
                spilEvent.customData.AddField("location", location);
            }

            if (rewardType != null) {
                spilEvent.customData.AddField("rewardType", rewardType);
            }

            spilEvent.Send();
        }

        /// <summary>
        /// When Fyber has shown a reward video and the user goes back to the game to receive his/her reward Fyber can
        /// automatically show a toast message with information about the reward, for instance "You've received 50 coins". 
        /// This is disabled by default to allow the developer to create a reward notification for the user.
        /// Developers can call SetShowToastOnVideoReward(true) to enable Fyber's automatic toast message.
        /// </summary>
        public override void SetShowToastOnVideoReward(bool value) {
        }

        /// <summary>
        /// Call to inform the SDK that the parental gate was (not) passes
        /// </summary>
        public override void ClosedParentalGate(bool pass) {
        }

        #endregion

        #region Spil Game Objects

        public override string GetSpilGameDataFromSdk() {
            return gData.GetGameObjects();
        }

        #endregion

        #region Image loading

        /// <summary>
        /// Used to get the image from the cache, based on the url provided.
        /// </summary>
        public override string GetImagePath(string url) {
            return url;
        }

        /// <summary>
        /// Used to get the image from the cache, based on the url provided.ImageContext will be imageType = custom when it's not provided as parameter
        /// </summary>
        public override void RequestImage(string url, int id, string imageType) {
            JSONObject jsonObject = new JSONObject();
            jsonObject.AddField("localPath", url);

            JSONObject imageContextJSON = new JSONObject();
            imageContextJSON.AddField("id", id);
            imageContextJSON.AddField("imageType", imageType);
            imageContextJSON.AddField("imageUrl", url);

            jsonObject.AddField("imageContext", imageContextJSON);

            SpilUnityImplementationBase.fireImageLoadSuccess(jsonObject.Print());
        }

        /// <summary>
        /// Clears the cache, useful in case when a lot of items have been updated.
        /// </summary>
        public override void ClearDiskCache() {
        }

        /// <summary>
        /// This method loops through all the items and bundles and adds urls to images (if any) to a download queue if those images have not yet been download and saved to local storage.
        /// </summary>
        public override void PreloadItemAndBundleImages() {
            SpilUnityImplementationBase.fireImagePreloadingCompleted();
        }

        #endregion

        #region Player Data

        public override void UpdatePlayerData() {
        }

        public override string GetWalletFromSdk() {
            return JsonHelper.getJSONFromObject(pData.Wallet);
        }

        public override string GetInvetoryFromSdk() {
            return JsonHelper.getJSONFromObject(pData.Inventory);
        }

        public override void AddCurrencyToWallet(int currencyId, int amount, string reason, string location,
            string reasonDetails = null, string transactionId = null) {
            pData.WalletOperation("add", currencyId, amount, reason, reasonDetails, location, transactionId);
        }

        public override void SubtractCurrencyFromWallet(int currencyId, int amount, string reason, string location,
            string reasonDetails = null, string transactionId = null) {
            pData.WalletOperation("subtract", currencyId, amount, reason, reasonDetails, location, transactionId);
        }

        public override void AddItemToInventory(int itemId, int amount, string reason, string location,
            string reasonDetails = null, string transactionId = null) {
            pData.InventoryOperation("add", itemId, amount, reason, reasonDetails, location, transactionId);
        }

        public override void SubtractItemFromInventory(int itemId, int amount, string reason, string location,
            string reasonDetails = null, string transactionId = null) {
            pData.InventoryOperation("subtract", itemId, amount, reason, reasonDetails, location, transactionId);
        }

        public override void BuyBundle(int bundleId, string reason, string location, string reasonDetails = null,
            string transactionId = null) {
            pData.BuyBundle(bundleId, reason, reasonDetails, location, transactionId);
        }

        public override void OpenGacha(int gachaId, string reason, string location, string reasonDetails = null) {
            pData.OpenGacha(gachaId, reason, reasonDetails, location);
        }

        public override void ResetPlayerData() {
            pData.ResetPlayerData();
            pData.SendUpdatePlayerDataEvent(true, true, PlayerDataUpdateReasons.Reset);
        }

        public override void ResetInventory() {
            pData.ResetInventory();
            pData.SendUpdatePlayerDataEvent(false, true, PlayerDataUpdateReasons.Reset);
        }

        public override void ResetWallet() {
            pData.ResetWallet();
            pData.SendUpdatePlayerDataEvent(true, false, PlayerDataUpdateReasons.Reset);
        }

        #endregion

        #region Non inherited members (Android only members)

        #region DFP / Fyber / Chartboost

        /// <summary>
        /// Method that initiaties DFP Ads (to be used only for testing purposes).
        /// This is not essential for developers so could be hidden but it might be handy for some developers so we left it in.
        /// </summary>
        /// <param name="adUnitId"></param>
        public void TestStartDFP(string adUnitId) {
        }

        /// <summary>
        /// Method that initiaties Fyber Ads (to be used only for testing purposes).
        /// This is not essential for developers so could be hidden but it might be handy for some developers so we left it in.
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="token"></param>
        public void TestStartFyber(string appId, string token) {
        }

        /// <summary>
        /// Method that shows Chartboost more apps (to be used only for testing purposes).
        /// This is not essential for developers so could be hidden but it might be handy for some developers so we left it in.
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSignature"></param>
        public void TestStartChartBoost(string appId, string appSignature) {
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
        }

        #endregion

        #region Push notifications

        /// <summary>
        /// Enables push notification for Android.
        /// Be sure to fill in the correct Project_ID in Spil.cs
        /// </summary>
        public void EnablePushNotifications() {
        }

        /// <summary>
        /// Disables push notification for Android.
        /// </summary>
        public void DisablePushNotifications() {
        }

        /// <summary>
        /// Disables the automatic register for push notifications for iOS.
        /// Should be called before the SpilInit() call!
        /// </summary>
        public void DisableAutomaticRegisterForPushNotifications() {
        }

        /// <summary>
        /// Registers for push notifications for iOS.
        /// Can be used then the automatic registration was disabled using: DisableAutomaticRegisterForPushNotifications();
        /// </summary>
        public void RegisterForPushNotifications() {
        }

        #endregion

        // If Android, register device
        private void RegisterDevice(string projectID) {
        }

        /// <summary>
        /// Retrieves the Spil User Id so that developers can show this in-game for users.
        /// If users contact Spil customer service they can supply this Id so that 
        /// customer support can help them properly. Please make this Id available for users
        /// in one of your game's screens.
        /// </summary>
        public override string GetSpilUserId() {
            return Spil.SpilUserIdEditor;
        }

        public override void SetUserId(string providerId, string userId) {
            Response.provider = providerId;
            Response.externalId = userId;
        }

        public override string GetUserId() {
            return Response.externalId;
        }

        public override void SetCustomBundleId(string bundleId) {
            // TODO
        }

        /// <summary>
        /// Gets the user provider.
        /// </summary>
        /// <returns>The user provider native.</returns>
        public override string GetUserProvider() {
            return Response.provider;
        }

        public override void RequestMyGameState() {
            RequestGameState();
        }

        /// <summary>
        /// Sets the state of the private game.
        /// </summary>
        /// <param name="privateData">Private data.</param>
        public override void SetPrivateGameState(string privateData) {
            GameStateResponse.SetPrivateGameState(privateData);
        }

        /// <summary>
        /// Gets the state of the private game.
        /// </summary>
        /// <returns>The private game state.</returns>
        public override string GetPrivateGameState() {
            return GameStateResponse.PrivateGameStateData;
        }

        /// <summary>
        /// Sets the public game state.
        /// </summary>
        /// <param name="publicData">Public data.</param>
        public override void SetPublicGameState(string publicData) {
            GameStateResponse.SetPublicGameState(publicData);
        }

        /// <summary>
        /// Gets the public game state.
        /// </summary>
        /// <returns>The public game state.</returns>
        public override string GetPublicGameState() {
            return GameStateResponse.PublicGameStateData;
        }

        /// <summary>
        /// Gets the public game state of other users.
        /// </summary>
        /// <param name="provider">Provider.</param>
        /// <param name="userIdsJsonArray">User identifiers json array.</param>
        public override void GetOtherUsersGameState(string provider, string userIdsJsonArray) {
            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "requestOtherUsersGameState";

            JSONObject users = new JSONObject(JSONObject.Type.ARRAY);

            List<String> userList = JsonHelper.getObjectFromJson<List<String>>(userIdsJsonArray);

            for (int i = 0; i < userList.Count; i++) {
                users.Add(userList[i]);
            }

            spilEvent.customData.AddField("provider", provider);
            spilEvent.customData.AddField("users", users);

            spilEvent.Send();
        }

        /// <summary>
        /// Used only by the Test app to check that calls were successfully made.
        /// This method is marked as "internal" and should not be exposed to developers.
        /// The Spil Unity SDK is not packaged as a seperate assembly yet so this method is currently visible, this will be fixed in the future.
        /// Internal method names start with a lower case so you can easily recognise and avoid them.
        /// </summary>
        internal string clearLog() {
            return "";
        }

        /// <summary>
        /// Used only by the Test app to check that calls were successfully made.
        /// This method is marked as internal and should not be exposed to developers.
        /// The Spil Unity SDK is not packaged as a seperate assembly yet so this method is currently visible, this will be fixed in the future.
        /// Internal method names start with a lower case so you can easily recognise and avoid them.
        /// </summary>
        internal string getLog() {
            return "";
        }

        private string CallNativeMethod(string methodName) {
            return "";
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
            string value = null;
            return value;
        }

        #endregion

        #region Customer support

        public override void ShowHelpCenter() {
            // TODO
        }

        public override void ShowContactCenter() {
            // TODO
        }

        public override void ShowHelpCenterWebview(string url) {
            // TODO
        }

        #endregion

        #region Daily Bonus

        public override void RequestDailyBonus() {
            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "requestDailyBonus";

            spilEvent.Send();
        }

        public override void RequestSplashScreen() {
            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "requestSplashscreen";

            spilEvent.Send();
        }

        #endregion

        #region Reward

        public override void ClaimToken(string token, string rewardType) {
            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "claimToken";

            spilEvent.customData.AddField("token", Spil.RewardToken);

            Spil.RewardFeatureTypeEnum tokenType = Spil.MonoInstance.RewardFeatureType;
            if (tokenType.Equals(Spil.RewardFeatureTypeEnum.DEEPLINK)) {
                spilEvent.customData.AddField("rewardType", "deepLink");
            }

            spilEvent.Send();
        }

        #endregion

        #region Permissions

        public void RequestDangerousPermission(string permission, string rationale) {
            SpilLogging.Log("Requested permission: " + permission);
        }

        #endregion

        #region Environemnt Changing

        public void SetProductionEnvironment() {
            SpilLogging.Log("Set environment: production!");
        }

        public void SetStagingEnvironment() {
            SpilLogging.Log("Set environment: staging!");
        }

        #endregion

        #region Server Time

        public override void RequestServerTime() {
            long currentTime =
                (long) (TimeZoneInfo.ConvertTimeToUtc(DateTime.Now) -
                        new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalMilliseconds;
            string time = currentTime.ToString();
            fireServerTimeRequestSuccess(time);
        }

        #endregion

        #region Live Event

        public override void RequestLiveEvent() {
            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "requestLiveEvent";

            spilEvent.Send();
        }

        public override void OpenLiveEvent() {
            LiveEventResponse.OpenLiveEvent();
        }

        public override string GetLiveEventConfig() {
            return LiveEventResponse.GetLiveEventConfig();
        }

        public override long GetLiveEventStartDate() {
            return LiveEventResponse.GetLiveEventStartDate();
        }

        public override long GetLiveEventEndDate() {
            return LiveEventResponse.GetLiveEventEndDate();
        }

        #endregion

        #region Social Login

        public override void UserLogin(string socialId, string socialProvider, string socialToken) {
            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "userLogin";

            spilEvent.customData.AddField("socialId", socialId);
            spilEvent.customData.AddField("socialProvider", socialProvider);
            spilEvent.customData.AddField("socialToken", socialToken);

            spilEvent.Send();
        }

        public override void UserLogout(bool global) {
            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "userLogout";

            spilEvent.customData.AddField("newUid", Spil.SpilUserIdEditor);
            spilEvent.customData.AddField("global", global);

            spilEvent.Send();

            if (!global) {
                SetUserId(null, null);
                Spil.SpilUserIdEditor = Guid.NewGuid().ToString();
                spilToken = null;
                SpilUnityImplementationBase.fireLogoutSuccessful();
            }
        }

        public override void UserPlayAsGuest() {
            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "userPlayAsGuest";

            string newUid = Guid.NewGuid().ToString();
            spilEvent.customData.AddField("newUid", newUid);

            spilEvent.Send();

            Spil.SpilUserIdEditor = newUid;
            unauthorized = false;
            spilToken = null;

            JSONObject loginResponse = new JSONObject();
            loginResponse.AddField("resetData", true);
            loginResponse.AddField("socialProvider", (string) null);
            loginResponse.AddField("socialId", (string) null);
            loginResponse.AddField("isGuest", true);

            SpilUnityImplementationBase.fireLoginSuccessful(loginResponse.Print());
        }

        public override void ShowUnauthorizedDialog(string title, string message, string loginText,
            string playAsGuestText) {
            SocialLoginResponse.ShowUnauthorizedDialog(title, message, loginText, playAsGuestText);
        }

        public override bool IsLoggedIn() {
            return spilToken != null;
        }

        #endregion
    }
}
#endif