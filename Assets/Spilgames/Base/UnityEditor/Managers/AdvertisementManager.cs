#if UNITY_EDITOR
using System;
using UnityEngine;
using System.Collections;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Helpers;
using SpilGames.Unity.Json;
using UnityEditor;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SpilGames.Unity.Base.UnityEditor.Managers {
    public class AdvertisementManager : MonoBehaviour {
        public static GameObject AdOverlay;
        public Text AdInfo;
        public static string adInfoText;

        private static String provider;
        private static String adType;

        public static bool DFPEnabled = false;
        public static bool FyberEnabled = false;
        public static bool ChartboostEnabled = false;

        void Update() {
            AdInfo.text = adInfoText;
        }
        
        public static void PlayVideo() {
            AdOverlay = (GameObject) Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Spilgames/Editor/Prefabs/AdOverlay.prefab"));
            AdOverlay.SetActive(true);
            provider = "Fyber";
            adType = "rewardVideo";
            adInfoText = provider + " " + adType + " is playing!";
            SpilUnityImplementationBase.fireAdStartedEvent();
        }

        public static void PlayInterstitial(string selectedProvider) {
            AdOverlay = (GameObject) Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Spilgames/Editor/Prefabs/AdOverlay.prefab"));
            AdOverlay.SetActive(true);
            provider = selectedProvider;
            adType = "interstitial";
            adInfoText = provider + " " + adType + " is playing!";
            SpilUnityImplementationBase.fireAdStartedEvent();
        }

        public static void PlayMoreApps() {
            AdOverlay = (GameObject) Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Spilgames/Editor/Prefabs/AdOverlay.prefab"));
            AdOverlay.SetActive(true);
            provider = "Spil";
            adType = "moreApps";
            adInfoText = provider + " " + adType + " is playing!";
            SpilUnityImplementationBase.fireAdStartedEvent();
        }

        public void CloseAd() {
            AdFinished adFinished = new AdFinished();

            adFinished.network = provider;
            adFinished.type = adType;
            adFinished.reason = "close";

            if (adType.ToLower().Trim().Equals("rewardvideo")) {
                adFinished.reward = new AdReward();

                if (Spil.CurrencyName != null) {
                    adFinished.reward.currencyName = Spil.CurrencyName;
                }

                if (Spil.CurrencyId != null) {
                    adFinished.reward.currencyId = Spil.CurrencyId;
                }

                if (Spil.Reward != 0) {
                    adFinished.reward.reward = Spil.Reward;
                }
            }

            SpilUnityImplementationBase.fireAdFinishedEvent(JsonHelper.getJSONFromObject(adFinished));
            provider = null;
            adType = null;
            Destroy(AdOverlay);
        }

        public void DismissAd() {
            AdFinished adFinished = new AdFinished();

            adFinished.network = provider;
            adFinished.type = adType;
            adFinished.reason = "dismiss";

            SpilUnityImplementationBase.fireAdFinishedEvent(JsonHelper.getJSONFromObject(adFinished));
            provider = null;
            adType = null;
            Destroy(AdOverlay);
        }
    }

    public class AdFinished {
        public string network;
        public string type;
        public string reason;
        public AdReward reward;
    }

    public class AdReward {
        public string currencyName;
        public string currencyId;
        public int reward;
    }
    
    public class AdvertisementResponse : Response {
        public static void ProcessAdvertisementResponse(ResponseEvent response) {
            if (response.action.Equals("init")) {
                JSONObject provider = response.data.GetField("providers");

                if (provider.HasField("DFP")) {
                    AdvertisementManager.DFPEnabled = true;
                    SpilLogging.Log("DFP Enabled");
                }

                if (provider.HasField("Fyber")) {
                    AdvertisementManager.FyberEnabled = true;
                    SpilLogging.Log("Fyber Enabled");
                }

                if (provider.HasField("Chartboost") || provider.HasField("ChartBoost")) {
                    AdvertisementManager.ChartboostEnabled = true;
                    SpilLogging.Log("Chartboost Enabled");
                }
            }
            else if (response.action.Equals("show")) {
                string provider = response.data.GetField("provider").str;
                string adType = response.data.GetField("adType").str;

                int probability = Random.Range(0, 100);
                bool available = probability > 20;

                if (provider.ToLower().Trim().Equals("dfp")) {
                    if (available) {
                        SpilLogging.Log("DFP Show");
                        SpilUnityImplementationBase.fireAdAvailableEvent(adType);
                        AdvertisementManager.PlayInterstitial("DFP");
                    }
                    else {
                        SpilLogging.Log("DFP Not Available");
                        SpilUnityImplementationBase.fireAdNotAvailableEvent(adType);
                    }
                }
                else if (provider.ToLower().Trim().Equals("fyber")) {
                    if (available) {
                        SpilLogging.Log("Fyber Show");
                        SpilUnityImplementationBase.fireAdAvailableEvent(adType);
                    }
                    else {
                        SpilLogging.Log("Fyber Not Available");
                        SpilUnityImplementationBase.fireAdNotAvailableEvent(adType);
                    }
                }
                else if (provider.ToLower().Trim().Equals("chartboost")) {
                    if (available) {
                        SpilLogging.Log("Chartboost Show");
                        SpilUnityImplementationBase.fireAdAvailableEvent(adType);
                        AdvertisementManager.PlayInterstitial("Chartboost");
                    }
                    else {
                        SpilLogging.Log("Chartboost Not Available");
                        SpilUnityImplementationBase.fireAdNotAvailableEvent(adType);
                    }
                }
            }
        }
    }
}
#endif