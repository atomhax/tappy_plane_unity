#if UNITY_EDITOR || UNITY_WEBGL
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

        public static bool AdMobEnabled = false;
        public static bool ChartboostEnabled = false;

        void Update() {
            AdInfo.text = adInfoText;
        }
        
        public static void PlayVideo() {
#if UNITY_WEBGL
            AdOverlay = (GameObject) Instantiate(UnityEngine.Resources.Load("Assets/Spilgames/Editor/Prefabs/AdOverlay.prefab"));
#else 
            AdOverlay = (GameObject) Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Spilgames/Editor/Prefabs/AdOverlay.prefab"));
#endif
            AdOverlay.SetActive(true);
            provider = "Fyber";
            adType = "rewardVideo";
            adInfoText = provider + " " + adType + " is playing!";
			Spil.Instance.fireAdStart();
        }

        public static void PlayInterstitial(string selectedProvider) {
#if UNITY_WEBGL
            AdOverlay = (GameObject) Instantiate(UnityEngine.Resources.Load("Assets/Spilgames/Editor/Prefabs/AdOverlay.prefab"));
#else 
            AdOverlay = (GameObject) Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Spilgames/Editor/Prefabs/AdOverlay.prefab"));
#endif
            AdOverlay.SetActive(true);
            provider = selectedProvider;
            adType = "interstitial";
            adInfoText = provider + " " + adType + " is playing!";
			Spil.Instance.fireAdStart();
        }

        public static void PlayMoreApps() {
#if UNITY_WEBGL
            AdOverlay = (GameObject) Instantiate(UnityEngine.Resources.Load("Assets/Spilgames/Editor/Prefabs/AdOverlay.prefab"));
#else 
            AdOverlay = (GameObject) Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Spilgames/Editor/Prefabs/AdOverlay.prefab"));
#endif
            AdOverlay.SetActive(true);
            provider = "Spil";
            adType = "moreApps";
            adInfoText = provider + " " + adType + " is playing!";
			Spil.Instance.fireAdStart();
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

			Spil.Instance.fireAdFinished(JsonHelper.getJSONFromObject(adFinished));
            provider = null;
            adType = null;
            Destroy(AdOverlay);
        }

        public void DismissAd() {
            AdFinished adFinished = new AdFinished();

            adFinished.network = provider;
            adFinished.type = adType;
            adFinished.reason = "dismiss";

			Spil.Instance.fireAdFinished(JsonHelper.getJSONFromObject(adFinished));
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

                if (provider.HasField("AdMob")) {
                    AdvertisementManager.AdMobEnabled = true;
                    SpilLogging.Log("AdMob Enabled");
                }

                if (provider.HasField("Chartboost") || provider.HasField("ChartBoost")) {
                    AdvertisementManager.ChartboostEnabled = true;
                    SpilLogging.Log("Chartboost Enabled");
                }
            }
            else if (response.action.Equals("show")) {
                int priv = Spil.Instance.GetPrivValue();

                if (priv < 2 && priv > -1) { 
                    return;
                }
                
                string provider = response.data.GetField("provider").str;
                string adType = response.data.GetField("adType").str;

                int probability = Random.Range(0, 100);
                bool available = probability > 20;

                if (provider.ToLower().Trim().Equals("admob")) {
                    if (available) {
                        SpilLogging.Log("AdMob Show");
                        Spil.Instance.fireAdAvailable(adType);

                        if (adType.Equals("interstitial")) {
                            AdvertisementManager.PlayInterstitial("AdMob"); 
                        } 
                    }
                    else {
                        SpilLogging.Log("AdMob Not Available");
						Spil.Instance.fireAdNotAvailable(adType);
                    }
                } else if (provider.ToLower().Trim().Equals("chartboost")) {
                    if (available) {
                        SpilLogging.Log("Chartboost Show");
						Spil.Instance.fireAdAvailable(adType);
                        AdvertisementManager.PlayInterstitial("Chartboost");
                    }
                    else {
                        SpilLogging.Log("Chartboost Not Available");
						Spil.Instance.fireAdNotAvailable(adType);
                    }
                }
            }
        }
    }
}
#endif