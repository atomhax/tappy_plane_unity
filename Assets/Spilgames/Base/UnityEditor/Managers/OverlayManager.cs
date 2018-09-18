#if UNITY_EDITOR || UNITY_WEBGL
using System.Collections;
using System.Collections.Generic;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Helpers.PlayerData;
using SpilGames.Unity.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SpilGames.Unity.Base.UnityEditor.Managers {
    public class OverlayManager : MonoBehaviour {
        public static GameObject SplashScreen;
        public static GameObject DailyBonus;
        
        public static SpilDailyBonus spilDailyBonusConfig;
        public static GameObject SpilSplashScreen;
        public static GameObject TempSpilSplashScreen;
        
        private static Spil.DailyBonusRewardTypeEnum rewardType;

        void Awake() {
            rewardType = Spil.MonoInstance.DailyBonusRewardType;
        }

        public static void processDailyBonusResponse(string dailyBonusConfig) {
            spilDailyBonusConfig = JsonHelper.getObjectFromJson<SpilDailyBonus>(dailyBonusConfig);
            
            Spil.Instance.fireDailyBonusAvailable(spilDailyBonusConfig.type);
        }
        
        public static void ShowSplashScreen(JSONObject data, string url) {
			Spil.Instance.fireSplashScreenOpen();

            #if UNITY_WEBGL && !UNITY_EDITOR
            WebGLJavaScriptInterface.OpenUrl(url, data, WebGLJavaScriptInterface.enumSplashScreenType.SPLASH_SCREEN);
            #else 
            SplashScreen = (GameObject) Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Spilgames/Editor/Prefabs/SplashScreen.prefab"));
            SplashScreen.SetActive(true);
            #endif
        }

        public static void ShowDailyBonus(JSONObject data, string url) {
//            Spil.MonoInstance.StartCoroutine(SetupSplashScreen(data, url));
        }

        public static IEnumerator SetupSplashScreen(JSONObject data, string url) {
            yield return Spil.MonoInstance.StartCoroutine(DownloadSplashScreenAssets(url));
            
            
            Image[] images = TempSpilSplashScreen.GetComponentsInChildren<Image>();
            foreach (Image image in images) {
                if (data.HasField(image.name)) {
                    yield return Spil.MonoInstance.StartCoroutine(DownloadSplashScreenImages(data.GetField(image.name).str, image, !image.name.Contains("background")));
                }
            }

            Text[] texts = TempSpilSplashScreen.GetComponentsInChildren<Text>();
            foreach (Text text in texts) {
                if (data.HasField(text.name)) {
                    text.text = data.GetField(text.name).str;
                }
            }
            
            TempSpilSplashScreen.SetActive(true);
        }
        
        public static IEnumerator DownloadSplashScreenAssets(string url) {
            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url);
            yield return request.SendWebRequest();

            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
            bundle.LoadAllAssets();
            SpilSplashScreen = bundle.LoadAsset<GameObject>("SplashScreen");
            TempSpilSplashScreen = (GameObject) Instantiate(SpilSplashScreen); 
        }

        public static IEnumerator DownloadSplashScreenImages(string url, Image image, bool preserveAspect) {
            Texture2D tex = new Texture2D((int) image.sprite.rect.width, (int) image.sprite.rect.height);
            WWW www = new WWW(url);
            yield return www;
            www.LoadImageIntoTexture(tex);

            image.type = Image.Type.Simple;
            image.preserveAspect = preserveAspect;

            image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2());
            

        }
        
        public static void ShowDailyBonus() {
            SpilLogging.Log("Opening URL: " + spilDailyBonusConfig.url + " With data: " + JsonHelper.getJSONFromObject(spilDailyBonusConfig));

			Spil.Instance.fireDailyBonusOpen();

            #if UNITY_WEBGL && !UNITY_EDITOR
            WebGLJavaScriptInterface.OpenUrl(url, data, WebGLJavaScriptInterface.enumSplashScreenType.DAILY_BONUS);
            #else 
            DailyBonus = (GameObject) Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Spilgames/Editor/Prefabs/DailyBonus.prefab"));
            DailyBonus.SetActive(true);
            #endif 
        }

        public static void CloseSplashScreen() {
            Spil.Instance.fireSplashScreenClosed();
            Destroy(SplashScreen);
        }

        public void CloseDailyBonus() {
            Spil.Instance.fireDailyBonusClosed();
            
            Destroy(DailyBonus);
        }
        
        public void OpenShop() {
			Spil.Instance.fireSplashScreenOpenShop();
            Destroy(SplashScreen);
        }

        public static void IAPPurchaseRequest(string skuId = null) {
            if(skuId != null) {
                Spil.IapPurchaseRequest = skuId;
            }
            if (Spil.IapPurchaseRequest == null || Spil.IapPurchaseRequest.Equals("")) {
                SpilLogging.Error("Iap Purchase Request SKU Id not set! Please configure value in the Spil SDK object!");
            } else {
				Spil.Instance.fireIAPRequestPurchase(Spil.IapPurchaseRequest);
            }

            Destroy(SplashScreen);
        }

        public static void CollectReward() {
            if (rewardType == Spil.DailyBonusRewardTypeEnum.EXTERNAL) {
                List<Reward> rewards = new List<Reward>();

                Reward reward = new Reward();
                reward.externalId = Spil.DailyBonusExternalId;
                reward.amount = Spil.DailyBonusAmount;

                rewards.Add(reward);

                string rewardsJSON = JsonHelper.getJSONFromObject(rewards);

                JSONObject json = new JSONObject();
                json.AddField("data", rewardsJSON);

				Spil.Instance.fireDailyBonusReward(json.Print(false));
            } else {
                int id = Spil.DailyBonusId;
                int amount = Spil.DailyBonusAmount;

                if (id == 0 || amount == 0) {
                    SpilLogging.Error("Daily Bonus Rewards not configured for Editor!");
                }

                if (rewardType == Spil.DailyBonusRewardTypeEnum.CURRENCY) {
                    SpilUnityEditorImplementation.pData.WalletOperation("add", id, amount, PlayerDataUpdateReasons.DailyBonus, null, "DailyBonus", null);
                } else if (rewardType == Spil.DailyBonusRewardTypeEnum.ITEM) {
                    SpilUnityEditorImplementation.pData.InventoryOperation("add", id, amount, PlayerDataUpdateReasons.DailyBonus, null, "DailyBonus", null);
                }
            }

			Spil.Instance.fireDailyBonusClosed();
            
            Destroy(DailyBonus);
        }
    }

    public class Reward {
        public int id;
        public string externalId;
        public string type;
        public int amount;
    }
    
    public class OverlayResponse : Response {
        public static void ProcessOverlayResponse(ResponseEvent response) {
            string url = null;

            if (response.data.HasField("url")) {
                url = response.data.GetField("url").str;
            }

            if (response.action.ToLower().Trim().Equals("show")) {
                if (response.eventName.ToLower().Equals("splashscreen")) {
                    OverlayManager.ShowSplashScreen(response.data.GetField("data"), url);
                }
                else if (response.eventName.ToLower().Equals("dailybonus")) {
                    OverlayManager.processDailyBonusResponse(response.data.Print());
                }
            }
            else if (response.action.ToLower().Trim().Equals("notavailable")) {
                if (response.eventName.ToLower().Equals("splashscreen")) {
					Spil.Instance.fireSplashScreenNotAvailable();
                }
                else if (response.eventName.ToLower().Equals("dailybonus")) {
                    OverlayManager.spilDailyBonusConfig = new SpilDailyBonus();
					Spil.Instance.fireDailyBonusNotAvailable();
                }
            }
        }
    }
}
#endif