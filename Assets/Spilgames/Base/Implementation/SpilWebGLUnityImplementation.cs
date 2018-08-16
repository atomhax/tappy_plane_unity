using System;
using System.Collections.Generic;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Helpers.GameData;
using SpilGames.Unity.Json;
using UnityEngine;

namespace SpilGames.Unity.Base.Implementations
{
#if UNITY_WEBGL
    public class SpilWebGLUnityImplementation : SpilUnityEditorImplementation
    {
        /*public static PlayerDataManager pData;
        public static GameDataManager gData;
        private static bool trackedInstall = false;
        public static bool unauthorized = false;
        public static string spilToken;
        
        public override void AddCurrencyToWallet(int currencyId, int amount, string reason, string location, string reasonDetails = null, string transactionId = null)
        {

        }

        public override void AddItemToInventory(int itemId, int amount, string reason, string location, string reasonDetails = null, string transactionId = null)
        {

        }

        public override void BuyBundle(int bundleId, string reason, string location, string reasonDetails = null, string transactionId = null)
        {

        }

        public override void ClaimToken(string token, string rewardType)
        {

        }

        public override void ClearDiskCache()
        {

        }

        public override void ClosedParentalGate(bool pass)
        {

        }

        public override string GetAllAssetBundles()
        {
            return null;
        }

        public override string GetAllPromotions()
        {
            return null;
        }

        public override List<TieredEvent> GetAllTieredEvents()
        {
            return null;
        }

        public override string GetBundlePromotion(int bundleId)
        {
            return null;
        }

        public override string GetConfigAll()
        {
            return null;
        }

        public override string GetConfigValue(string key)
        {
            return null;
        }*/

        public override string GetDeviceId()
        {
            return WebGLJavaScriptInterface.GetDeviceIdJS();
        }

        /*public override string GetImagePath(string url)
        {
            return null;
        }

        public override string GetInvetoryFromSdk()
        {
            return null;
        }

        public override string GetLiveEventConfig()
        {
            return null;
        }

        public override long GetLiveEventEndDate()
        {
            return 0l;
        }

        public override long GetLiveEventStartDate()
        {
            return 0l;
        }

        public override void GetOtherUsersGameState(string provider, string userIdsJsonArray)
        {

        }

        public override string GetPackagePromotion(string packageId)
        {
            return null;
        }

        public override string GetPrivateGameState()
        {
            return null;
        }

        public override int GetPrivValue()
        {
            return -1;
        }

        public override string GetPublicGameState()
        {
            return null;
        }

        public override string GetSpilGameDataFromSdk()
        {
            return null;
        }

        public override string GetSpilUserId()
        {
            return null;
        }

        public override TieredEventProgress GetTieredEventProgress(int tieredEventId)
        {
            return null;
        }

        public override string GetUserId()
        {
            return null;
        }

        public override string GetUserProvider()
        {
            return null;
        }

        public override string GetWalletFromSdk()
        {
            return null;
        }

        public override bool IsLoggedIn()
        {
            return false;
        }

        public override void MergeUserData(string mergeData, string mergeType)
        {

        }

        public override void OpenGacha(int gachaId, string reason, string location, string reasonDetails = null)
        {

        }

        public override void OpenLiveEvent()
        {

        }

        public override void PlayMoreApps()
        {

        }

        public override void PlayVideo(string location = null, string rewardType = null)
        {

        }

        public override void PreloadItemAndBundleImages()
        {

        }

        public override void RequestAssetBundles()
        {

        }

        public override void RequestDailyBonus()
        {

        }

        public override void RequestImage(string url, int id, string imageType)
        {

        }

        public override void RequestLiveEvent()
        {

        }

        public override void RequestMoreApps()
        {

        }

        public override void RequestPackages()
        {

        }

        public override void RequestPromotions()
        {

        }

        public override void RequestRewardVideo(string location = null, string rewardType = null)
        {

        }

        public override void RequestServerTime()
        {

        }

        public override void RequestSplashScreen(string type)
        {

        }

        public override void RequestTieredEvents()
        {

        }

        public override void RequestUserData()
        {

        }

        public override void ResetData()
        {

        }

        public override void ResetInventory()
        {

        }

        public override void ResetPlayerData()
        {

        }

        public override void ResetWallet()
        {

        }

        public override void SavePrivValue(int priv)
        {

        }

        public override void SetCustomBundleId(string bundleId)
        {

        }

        public override void SetPluginInformation(string PluginName, string PluginVersion)
        {

        }

        public override void SetPrivateGameState(string privateData)
        {

        }

        public override void SetPublicGameState(string publicData)
        {

        }

        public override void SetUserId(string providerId, string userId)
        {

        }

        public override void ShowHelpCenterWebview(string url)
        {

        }

        public override void ShowMergeConflictDialog(string title, string message, string localButtonText, string remoteButtonText, string mergeButtonText = null)
        {

        }

        public override void ShowMergeFailedDialog(string title, string message, string retryButtonText, string mergeData, string mergeType)
        {

        }

        public override void ShowNativeDialog(string title, string message, string buttonText)
        {

        }

        public override void ShowPrivacyPolicySettings()
        {

        }

        public override void ShowPromotionScreen(int promotionId)
        {

        }

        public override void ShowSyncErrorDialog(string title, string message, string startMergeButtonText)
        {

        }

        public override void ShowTieredEventProgress(int tieredEventId)
        {

        }

        public override void ShowUnauthorizedDialog(string title, string message, string loginText, string playAsGuestText)
        {

        }

        public override void SubtractCurrencyFromWallet(int currencyId, int amount, string reason, string location, string reasonDetails = null, string transactionId = null)
        {

        }

        public override void SubtractItemFromInventory(int itemId, int amount, string reason, string location, string reasonDetails = null, string transactionId = null)
        {

        }

        public override void TestRequestAd(string providerName, string adType, bool parentalGate)
        {

        }

        public override void UpdatePlayerData()
        {

        }

        public override void UserLogin(string socialId, string socialProvider, string socialToken)
        {

        }

        public override void UserLogout(bool global)
        {

        }

        public override void UserPlayAsGuest()
        {

        }

        protected override string GetAllPackages()
        {
            return null;
        }

        protected override string GetPackage(string key)
        {
            return null;
        }

        internal override void CheckPrivacyPolicy()
        {
            
        }

        internal override void SendCustomEventInternal(string eventName, Dictionary<string, object> dict)
        {
            SpilLogging.Log("SendCustomEvent " + eventName);
            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = eventName;

            if (dict != null) {
                spilEvent.customData = JsonHelper.DictToJSONObject(dict);
                if (dict.ContainsKey("trackingOnly")) {
                    spilEvent.customData.RemoveField("trackingOnly");
                    spilEvent.customData.AddField("trackingOnly", true);
                    dict.Remove("trackingOnly");
                }
            }

            spilEvent.Send();
        }

        internal override void SpilInit(bool withPrivacyPolicy)
        {
            gData = new GameDataManager();
            pData = new PlayerDataManager();
            
            TrackEditorInstall();
            
            RequestConfig();
            RequestGameData();
            RequestUserData();
            AdvertisementInit();
            RequestPackages();
            RequestPromotions();
            RequestAssetBundles();
        }
        
        private void TrackEditorInstall() {
            if (trackedInstall) {
                return;
            }
            
            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "install";

            spilEvent.Send();

            trackedInstall = true;
        }
        
        private void RequestConfig() {
            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "requestConfig";
            spilEvent.Send();
        }
        
        private void RequestGameData() {
            gData = gData.InitialiseGameObjects();

            Spil.GameData = new SpilGameDataHelper(this);

            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "requestGameData";

            spilEvent.Send();
        }

        internal void AdvertisementInit() {
            SpilEvent spilEvent = Spil.MonoInstance.gameObject.AddComponent<SpilEvent>();
            spilEvent.eventName = "advertisementInit";

            spilEvent.Send();
        }*/
    }
#endif
        }