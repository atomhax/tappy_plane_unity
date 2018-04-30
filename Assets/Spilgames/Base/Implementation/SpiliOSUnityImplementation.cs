using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Json;
using System.Collections;
using SpilGames.Unity.Helpers;
using System.Runtime.Serialization.Formatters;

namespace SpilGames.Unity.Base.Implementations
{
#if UNITY_IPHONE || UNITY_TVOS
    public class SpiliOSUnityImplementation : SpilUnityImplementationBase
    {

    #region Inherited members

        public override void SetPluginInformation(string PluginName, string PluginVersion)
        {
            setPluginInformationNative(PluginName, PluginVersion);
        }

        [DllImport("__Internal")]
        private static extern void setPluginInformationNative(string pluginName, string pluginVersion);

    #endregion

    #region Game config

        /// <summary>
        /// Returns the game config as a json string.
        /// This is not essential for developers so could be made private (getConfig<T>() uses it so it cannot be removed entirely) but might be handy for some developers so we left it in.
        /// </summary>
        /// <returns></returns>
        public override string GetConfigAll()
        {
            return getConfigNative();
        }

        /// <summary>
        /// Method that returns a configuration value from the game config based on key 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override string GetConfigValue(string key)
        {
            return getConfigValueNative(key);
        }


    #endregion


    #region Packages and promotions

        /// <summary>
        /// Method that requests packages and promotions from the server.
        /// This is called automatically by the Spil SDK when the game starts.
        /// This is not essential so could be removed but might be handy for some developers so we left it in.
        /// </summary>
        public override void RequestPackages() {
            requestPackagesNative();
        }
        [DllImport("__Internal")]
        private static extern void requestPackagesNative();
        
        public override void RequestPromotions() {
            requestPromotionsNative();
        }
        [DllImport("__Internal")]
        private static extern void requestPromotionsNative();

        // Method that returns the all packages
        protected override string GetAllPackages() {
            return getAllPackagesNative();
        }
        [DllImport("__Internal")]
        private static extern string getAllPackagesNative();
        
        // Method that returns a package based on key
        protected override string GetPackage(string key) {
            return getPackageNative(key);
        }
        [DllImport("__Internal")]
        private static extern string getPackageNative(string keyName);

        public override string GetAllPromotions() {
            return getAllPromotionsNative();
        }
        [DllImport("__Internal")]
        private static extern string getAllPromotionsNative();

        public override string GetBundlePromotion(int bundleId) {
            return getBundlePromotionNative(bundleId);
        }
        [DllImport("__Internal")]
        private static extern string getBundlePromotionNative(int bundleId);
        
        public override string GetPackagePromotion(string packageId) {
            return getPackagePromotionNative(packageId);
        }
        private static extern string getPackagePromotionNative(string packageId);
        
        public override void ShowPromotionScreen(int promotionId) {
            showPromotionScreenNative(promotionId);
        }
        [DllImport("__Internal")]
        private static extern void showPromotionScreenNative(int promotionId);
        
    #endregion

        /// <summary>
        /// This method is marked as internal and should not be exposed to developers.
        /// The Spil Unity SDK is not packaged as a seperate assembly yet so this method is currently visible, this will be fixed in the future.
        /// Internal method names start with a lower case so you can easily recognise and avoid them.
        /// </summary>
        internal override void SpilInit(bool withPrivacyPolicy)
        {
            JSONObject options = new JSONObject();
            options.AddField("isUnity", true);
            options.AddField("privacyPolicyEnabled", withPrivacyPolicy);
            initEventTrackerWithOptions(options.ToString());
            RequestPackages();
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
        protected override void SendCustomEvent(string eventName, Dictionary<string, object> dict)
        {
            Debug.Log("SpilSDK-Unity SendCustomEvent \"" + eventName + "\"" + (dict == null ? "" : " params: " + JsonHelper.DictToJSONObject(dict).ToString()));

            string parameters = null;

            if (dict != null)
            {
                parameters = JsonHelper.DictToJSONObject(dict).ToString();
                if (parameters != "{}")
                {
                    trackEventWithParamsNative(eventName, parameters);
                }
                else
                {
                    trackEventNative(eventName);
                }
            }
            else
            {
                trackEventNative(eventName);
            }
        }

        public override void RequestRewardVideo(string location = null, string rewardType = null)
        {
            int priv = Spil.Instance.GetPrivValue();

            if (priv < 2 && priv > -1) {
                fireAdAvailableEvent("rewardVideo");
                return;
            }
            
            requestRewardVideoNative(location, rewardType);
        }

        [DllImport("__Internal")]
        private static extern void requestRewardVideoNative(string location, string rewardType);

        /// <summary>
        /// This can be called to show a video, for instance after calling "SendrequestRewardVideoEvent()"
        /// and receiving an "AdAvailable" event the developer could call this method from the event handler.
        /// When calling this method "SendrequestRewardVideoEvent()" must first have been called to request and cache a video.
        /// If no video is available then nothing will happen.
        /// </summary>
        public override void PlayVideo(string location = null, string rewardType = null)
        {
            int priv = Spil.Instance.GetPrivValue();

            if (priv < 2 && priv > -1 && Spil.UseUnityPrefab) {
                ShowAdsScreen();
                return;
            }
            
            playRewardVideoNative(location, rewardType);
        }

        [DllImport("__Internal")]
        private static extern void playRewardVideoNative(string location, string rewardType);

        /// <summary>
        /// When Fyber has shown a reward video and the user goes back to the game to receive his/her reward Fyber can
        /// automatically show a toast message with information about the reward, for instance "You've received 50 coins". 
        /// This is disabled by default to allow the developer to create a reward notification for the user.
        /// Developers can call SetShowToastOnVideoReward(true) to enable Fyber's automatic toast message.
        /// </summary>
        /*public override void SetShowToastOnVideoReward(bool value)
        {
            showToastOnVideoReward(value);
        }

        [DllImport("__Internal")]
        private static extern void showToastOnVideoReward(bool show);*/

        /// <summary>
        /// Call to inform the SDK that the parental gate was (not) passes
        /// </summary>
        public override void ClosedParentalGate(bool pass)
        {
            closedParentalGateNative(pass);
        }

        [DllImport("__Internal")]
        private static extern void closedParentalGateNative(bool pass);

        /// <summary>
        /// Sends the "requestAd" event with the "moreApps" parameter to the native Spil SDK which will send a request to the back-end.
        /// When a response has been received from the back-end the SDK will fire either an "AdAvailable" or and "AdNotAvailable"
        /// event to which the developer can subscribe and for instance call PlayVideo(); or PlayMoreApps();
        /// </summary>
        public override void RequestMoreApps()
        {
            requestMoreAppsNative();
        }

        [DllImport("__Internal")]
        private static extern void requestMoreAppsNative();

        /// <summary>
        /// This can be called to show the "more apps" activity, for instance after calling "RequestMoreApps()"
        /// and receiving an "AdAvailable" event the developer could call this method from the event handler.
        /// When calling this method "RequestMoreApps()" must first have been called to request and cache a video.
        /// If no video is available then nothing will happen.
        /// </summary>
        public override void PlayMoreApps()
        {
            showMoreAppsNative();
        }

        [DllImport("__Internal")]
        private static extern void showMoreAppsNative();

        /// <summary>
        /// Method that initiaties a Test Ad.
        /// This is not essential for developers so could be hidden but it might be handy for some developers so we left it in.
        /// </summary>
        /// <param name="adUnitId"></param>
        public override void TestRequestAd(string providerName, string adType, bool parentalGate)
        {
            devRequestAdNative(providerName, adType, parentalGate);
        }

        [DllImport("__Internal")]
        private static extern void devRequestAdNative(string providerName, string adTypeName, bool parentalGate);

        /// <summary>
        /// Retrieves the Spil User Id so that developers can show this in-game for users.
        /// If users contact Spil customer service they can supply this Id so that 
        /// customer support can help them properly. Please make this Id available for users
        /// in one of your game's screens.
        /// </summary>
        public override string GetSpilUserId()
        {
            return getSpilUserIdNative();
        }
        [DllImport("__Internal")]
        private static extern string getSpilUserIdNative();

        /// <summary>
        /// Retrieves the custom User Id
        /// </summary>
        public override string GetUserId()
        {
            return getExternalUserIdNative();
        }
        [DllImport("__Internal")]
        private static extern string getExternalUserIdNative();

        /// <summary>
        /// Gets the user provider.
        /// </summary>
        /// <returns>The user provider native.</returns>
        public override string GetUserProvider()
        {
            return getExternalUserProviderNative();
        }
        [DllImport("__Internal")]
        private static extern string getExternalUserProviderNative();

        /// <summary>
        /// Sets the custom User Id for a provider
        /// </summary>
        /// <param name="providerId"></param>
        /// <param name="userId"></param>
        public override void SetUserId(string providerId, string userId)
        {
            setExternalUserIdNative(providerId, userId);
        }
        [DllImport("__Internal")]
        private static extern void setExternalUserIdNative(string providerId, string userId);

        public override void SetCustomBundleId(string bundleId)
        {
            setCustomBundleIdNative(bundleId);
        }
        [DllImport("__Internal")]
        private static extern void setCustomBundleIdNative(string bundleId);

        /// <summary>
        /// Sets the state of the private game.
        /// </summary>
        /// <param name="privateData">Private data.</param>
        public override void SetPrivateGameState(string privateData)
        {
            setPrivateGameStateNative(privateData);
        }

        [DllImport("__Internal")]
        private static extern void setPrivateGameStateNative(string privateData);

        /// <summary>
        /// Gets the state of the private game.
        /// </summary>
        /// <returns>The private game state.</returns>
        public override string GetPrivateGameState()
        {
            return getPrivateGameStateNative();
        }

        [DllImport("__Internal")]
        private static extern string getPrivateGameStateNative();

        /// <summary>
        /// Sets the public game state.
        /// </summary>
        /// <param name="publicData">Public data.</param>
        public override void SetPublicGameState(string publicData)
        {
            setPublicGameStateNative(publicData);
        }

        [DllImport("__Internal")]
        private static extern void setPublicGameStateNative(string publicData);

        /// <summary>
        /// Gets the public game state.
        /// </summary>
        /// <returns>The public game state.</returns>
        public override string GetPublicGameState()
        {
            return getPublicGameStateNative();
        }

        [DllImport("__Internal")]
        private static extern string getPublicGameStateNative();

        /// <summary>
        /// Gets the public game state of other users.
        /// </summary>
        /// <param name="provider">Provider.</param>
        /// <param name="userIdsJsonArray">User identifiers json array.</param>
        public override void GetOtherUsersGameState(string provider, string userIdsJsonArray)
        {
            getOtherUsersGameStateNative(provider, userIdsJsonArray);
        }

        [DllImport("__Internal")]
        private static extern void getOtherUsersGameStateNative(string provider, string userIdsJsonArray);

    #region Spil Game Objects

        public override string GetSpilGameDataFromSdk()
        {
            return getSpilGameDataNative();
        }

        [DllImport("__Internal")]
        private static extern string getSpilGameDataNative();

    #endregion

    #region Image loading

        /// <summary>
        /// Used to get the image from the cache, based on the url provided.
        /// </summary>
        public override string GetImagePath(string url)
        {
            if (!string.IsNullOrEmpty(getImagePathNative(url)))
            {
                return "file://" + getImagePathNative(url);
            }
            else
            {
                return null;
            }
        }

        [DllImport("__Internal")]
        private static extern string getImagePathNative(string url);

        /// <summary>
        /// Used to get the image from the cache, based on the url provided.ImageContext will be imageType = custom when it's not provided as parameter
        /// </summary>
        public override void RequestImage(string url, int id, string imageType)
        {
            requestImageNative(url, id, imageType);
        }

        [DllImport("__Internal")]
        private static extern void requestImageNative(string url, int id, string imageType);

        /// <summary>
        /// Clears the cache, useful in case when a lot of items have been updated.
        /// </summary>
        public override void ClearDiskCache()
        {
            clearDiskCacheNative();
        }

        [DllImport("__Internal")]
        private static extern void clearDiskCacheNative();

        /// <summary>
        /// This method loops through all the items and bundles and adds urls to images (if any) to a download queue if those images have not yet been download and saved to local storage.
        /// </summary>
        public override void PreloadItemAndBundleImages()
        {
            preloadItemAndBundleImagesNative();
        }

        [DllImport("__Internal")]
        private static extern void preloadItemAndBundleImagesNative();

    #endregion

    #region Player Data

        public override void UpdatePlayerData()
        {
            updatePlayerDataNative();
        }

        [DllImport("__Internal")]
        private static extern void updatePlayerDataNative();

        public override string GetWalletFromSdk()
        {
            return getWalletNative();
        }

        [DllImport("__Internal")]
        private static extern string getWalletNative();

        public override string GetInvetoryFromSdk()
        {
            return getInventoryNative();
        }

        [DllImport("__Internal")]
        private static extern string getInventoryNative();

        public override void AddCurrencyToWallet(int currencyId, int amount, string reason, string location, string reasonDetails = null, string transactionId = null)
        {
            addCurrencyToWalletNative(currencyId, amount, reason, location, reasonDetails, transactionId);
        }

        [DllImport("__Internal")]
        private static extern void addCurrencyToWalletNative(int currencyId, int amount, string reason, string location, string reasonDetails, string transactionId);

        public override void SubtractCurrencyFromWallet(int currencyId, int amount, string reason, string location, string reasonDetails = null, string transactionId = null)
        {
            subtractCurrencyFromWalletNative(currencyId, amount, reason, location, reasonDetails, transactionId);
        }

        [DllImport("__Internal")]
        private static extern void subtractCurrencyFromWalletNative(int currencyId, int amount, string reason, string location, string reasonDetails, string transactionId);

        public override void AddItemToInventory(int itemId, int amount, string reason, string location, string reasonDetails = null, string transactionId = null)
        {
            addItemToInventoryNative(itemId, amount, reason, location, reasonDetails, transactionId);
        }

        [DllImport("__Internal")]
        private static extern void addItemToInventoryNative(int itemId, int amount, string reason, string location, string reasonDetails, string transactionId);

        public override void SubtractItemFromInventory(int itemId, int amount, string reason, string location, string reasonDetails = null, string transactionId = null)
        {
            subtractItemFromInventoryNative(itemId, amount, reason, location, reasonDetails, transactionId);
        }

        [DllImport("__Internal")]
        private static extern void subtractItemFromInventoryNative(int itemId, int amount, string reason, string location, string reasonDetails, string transactionId);

        public override void BuyBundle(int bundleId, string reason, string location, string reasonDetails = null, string transactionId = null)
        {
            buyBundleNative(bundleId, reason, location, reasonDetails, transactionId);
        }

        [DllImport("__Internal")]
        private static extern void buyBundleNative(int bundleId, string reason, string location, string reasonDetails, string transactionId);

        public override void OpenGacha(int gachaId, string reason, string location, string reasonDetails = null)
        {
            openGachaNative(gachaId, reason, reasonDetails, location);
        }

        [DllImport("__Internal")]
        private static extern void openGachaNative(int bundleId, string reason, string reasonDetails, string location);

        public override void ResetPlayerData()
        {
            resetPlayerDataNative();
        }

        [DllImport("__Internal")]
        private static extern void resetPlayerDataNative();

        public override void ResetInventory()
        {
            resetInventoryNative();
        }

        [DllImport("__Internal")]
        private static extern void resetInventoryNative();

        public override void ResetWallet()
        {
            resetWalletNative();
        }

        [DllImport("__Internal")]
        private static extern void resetWalletNative();

    #endregion

    #region Game config

        [DllImport("__Internal")]
        private static extern string getConfigNative();

        [DllImport("__Internal")]
        private static extern string getConfigValueNative(string keyName);


    #endregion

    #region Reward

        [DllImport("__Internal")]
        private static extern void claimTokenNative(string token, string rewardType);

        public override void ClaimToken(string token, string rewardType)
        {
            claimTokenNative(token, rewardType);
        }

    #endregion

    #region Server Time

        [DllImport("__Internal")]
        private static extern void requestServerTimeNative();

        public override void RequestServerTime()
        {
            requestServerTimeNative();
        }

    #endregion

    #region Live Event

        public override void RequestLiveEvent()
        {
            requestLiveEventNative();
        }

        [DllImport("__Internal")]
        private static extern void requestLiveEventNative();

        public override void OpenLiveEvent()
        {
            openLiveEventNative();
        }

        [DllImport("__Internal")]
        private static extern void openLiveEventNative();

        public override string GetLiveEventConfig()
        {
            return getLiveEventConfigNative();
        }

        [DllImport("__Internal")]
        private static extern string getLiveEventConfigNative();

        public override long GetLiveEventStartDate()
        {
            return Convert.ToInt64(getLiveEventStartDateNative());
        }

        [DllImport("__Internal")]
        private static extern string getLiveEventStartDateNative();

        public override long GetLiveEventEndDate()
        {
            return Convert.ToInt64(getLiveEventEndDateNative());
        }

        [DllImport("__Internal")]
        private static extern string getLiveEventEndDateNative();

    #endregion

    #region Tiered Events

        public override void RequestTieredEvents()
        {
            //requestTieredEventsNative();
        }

        //[DllImport("__Internal")]
        //private static extern string requestTieredEventsNative();

        public override List<TieredEvent> GetAllTieredEvents()
        {
            /*string tieredEventsJson = getAllTieredEventsNative();
            if (tieredEventsJson != null)
            {
                // TODO: Test this, probably wont work because the list is inside a root node.
                return JsonHelper.getObjectFromJson<List<TieredEvent>>(tieredEventsJson);
            }*/
            return null;
        }
        
        public override TieredEventProgress GetTieredEventProgress(int tieredEventId) {
            return new TieredEventProgress();
        }

        //[DllImport("__Internal")]
        //private static extern string getAllTieredEventsNative();

        public override void ShowTieredEventProgress(int tieredEventId)
        {
            //showTieredEventProgressNative(tieredEventId);
        }

        //[DllImport("__Internal")]
        //private static extern void showTieredEventProgressNative(int tieredEventId);

    #endregion

    #region Customer support

        public override void ShowHelpCenterWebview(string url)
        {
            showHelpCenterWebviewNative(url);
        }

        [DllImport("__Internal")]
        private static extern void showHelpCenterWebviewNative(string url);


    #endregion


    #region Web

        public override void RequestDailyBonus()
        {
            requestDailyBonusNative();
        }

        [DllImport("__Internal")]
        private static extern void requestDailyBonusNative();

        public override void RequestSplashScreen(string type = null)
        {
            requestSplashScreenNative(type);
        }

        [DllImport("__Internal")]
        private static extern void requestSplashScreenNative(string type);


    #endregion


    #region Push notifications

        /// <summary>
        /// Disables the automatic register for push notifications.
        /// Should be called before SpilInit!
        /// </summary>
        public void DisableAutomaticRegisterForPushNotifications()
        {
            Debug.Log("UNITY: DisableAutomaticRegisterForPushNotifications");

            disableAutomaticRegisterForPushNotificationsNative();
        }

        [DllImport("__Internal")]
        private static extern void disableAutomaticRegisterForPushNotificationsNative();

        /// <summary>
        /// Registers for push notifications for iOS.
        /// Can be used then the automatic registration was disabled using: DisableAutomaticRegisterForPushNotifications();
        /// </summary>
        public void RegisterForPushNotifications()
        {
            Debug.Log("UNITY: RegisterForPushNotifications");

            registerForPushNotifications();
        }

        [DllImport("__Internal")]
        private static extern void registerForPushNotifications();

    #endregion

    #region event tracker

        [DllImport("__Internal")]
        private static extern void initEventTrackerWithOptions(string options);

        [DllImport("__Internal")]
        private static extern void trackEventNative(string eventName);

        [DllImport("__Internal")]
        private static extern void trackEventWithParamsNative(string eventName, string jsonStringParams);

    #endregion

    #region Social Login

        public override void UserLogin(string socialId, string socialProvider, string socialToken)
        {
            loginNative(socialId, socialProvider, socialToken);
        }

        [DllImport("__Internal")]
        private static extern void loginNative(string externalUserId, string externalProviderId, string externalToken);

        public override void UserLogout(bool global)
        {
            logoutNative(global);
        }

        [DllImport("__Internal")]
        private static extern void logoutNative(bool global);

        public override void UserPlayAsGuest()
        {
            userPlayAsGuestNative();
        }

        [DllImport("__Internal")]
        private static extern void userPlayAsGuestNative();

        public override void ShowUnauthorizedDialog(string title, string message, string loginText, string playAsGuestText)
        {
            showAuthorizedDialogNative(title, message, loginText, playAsGuestText);
        }

        [DllImport("__Internal")]
        private static extern void showAuthorizedDialogNative(string title, string message, string loginButtonText, string guestButtonText);

        public override bool IsLoggedIn()
        {
            return isLoggedInNative();
        }

        [DllImport("__Internal")]
        private static extern bool isLoggedInNative();

        public override void ResetData()
        {
            resetDataNative();
        }

        [DllImport("__Internal")]
        private static extern void resetDataNative();

        public override void ShowNativeDialog(string title, string message, string buttonText)
        {
            showDialogNative(title, message, buttonText);
        }

        [DllImport("__Internal")]
        private static extern void showDialogNative(string title, string message, string okButtonText);

    #endregion

    #region Userdata syncing

        public override string GetDeviceId()
        {
            return getDeviceIdNative();
        }
        [DllImport("__Internal")]
        private static extern string getDeviceIdNative();

        public override void RequestUserData()
        {
            requestUserDataNative();
        }
        [DllImport("__Internal")]
        private static extern void requestUserDataNative();

        public override void MergeUserData(string mergeData, string mergeType)
        {
            mergeUserDataNative(mergeData, mergeType);
        }
        [DllImport("__Internal")]
        private static extern void mergeUserDataNative(string mergedUserData, string mergeType);

        public override void ShowMergeConflictDialog(string title, string message, string localButtonText, string remoteButtonText, string mergeButtonText = null)
        {
            showMergeConflictDialogNative(title, message, localButtonText, remoteButtonText, mergeButtonText);
        }
        [DllImport("__Internal")]
        private static extern void showMergeConflictDialogNative(string title, string message, string localButtonText, string remoteButtonText, string mergeButtonText);

        public override void ShowSyncErrorDialog(string title, string message, string startMergeButtonText)
        {
            showSyncErrorDialogNative(title, message, startMergeButtonText);
        }
        [DllImport("__Internal")]
        private static extern void showSyncErrorDialogNative(string title, string message, string mergeButtonText);

        public override void ShowMergeFailedDialog(string title, string message, string retryButtonText, string mergeData, string mergeType)
        {
            showMergeFailedDialogNative(title, message, retryButtonText, mergeData, mergeType);
        }
        [DllImport("__Internal")]
        private static extern void showMergeFailedDialogNative(string title, string message, string retryButtonText, string mergeData, string mergeType);

    #endregion

    #region privacy policy

        internal override void CheckPrivacyPolicy()
        {
            checkPrivacyPolicyNative();
        }
        [DllImport("__Internal")]
        private static extern void checkPrivacyPolicyNative();

        public override void ShowPrivacyPolicySettings()
        {
            if (!Spil.CheckPrivacyPolicy) {
                Debug.Log("Privacy Policy not enabled. Will not show privacy policy settings screen");
                return;
            }
    
            if (Spil.UseUnityPrefab) {
                PrivacyPolicyHelper.PrivacyPolicyObject = (GameObject) GameObject.Instantiate(Resources.Load("Spilgames/PrivacyPolicy/PrivacyPolicyUnity" + Spil.MonoInstance.PrefabOrientation));
                PrivacyPolicyHelper.PrivacyPolicyObject.SetActive(true);
            
                PrivacyPolicyHelper.Instance.ShowSettingsScreen(1);
            }
            else
            {
                showPrivacyPolicySettings();
            }
        }
        [DllImport("__Internal")]
        private static extern void showPrivacyPolicySettings();
        
        public override void SavePrivValue(int priv)
        {
            savePrivValueNative(priv);
        }
        [DllImport("__Internal")]
        private static extern void savePrivValueNative(int priv);
    
        public override int GetPrivValue()
        {
            return getPrivValueNative();
        }
        [DllImport("__Internal")]
        private static extern int getPrivValueNative();
        
    #endregion
    }        
#endif
}