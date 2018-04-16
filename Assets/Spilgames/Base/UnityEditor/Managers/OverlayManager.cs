#if UNITY_EDITOR
using System.Collections.Generic;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Helpers.PlayerData;
using SpilGames.Unity.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SpilGames.Unity.Base.UnityEditor.Managers {
    public class OverlayManager : MonoBehaviour {
        public static GameObject SplashScreen;
        public static GameObject DailyBonus;

        private static Spil.DailyBonusRewardTypeEnum rewardType;

        void Awake() {
            rewardType = Spil.MonoInstance.DailyBonusRewardType;
        }

        public static void ShowSplashScreen(JSONObject data, string url) {
            SpilLogging.Log("Opening URL: " + url + " With data: " + data.Print(false));

            SpilUnityImplementationBase.fireSplashScreenOpen();

            SplashScreen = (GameObject) Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Spilgames/Editor/Prefabs/SplashScreen.prefab"));
            SplashScreen.SetActive(true);
        }

        public static void ShowDailyBonus(JSONObject data, string url) {
            SpilLogging.Log("Opening URL: " + url + " With data: " + data.Print(false));

            SpilUnityImplementationBase.fireDailyBonusOpen();

            DailyBonus = (GameObject) Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Spilgames/Editor/Prefabs/DailyBonus.prefab"));
            DailyBonus.SetActive(true);
        }

        public void CloseSplashScreen() {
            SpilUnityImplementationBase.fireSplashScreenClosed();

            Destroy(SplashScreen);
        }

        public void OpenShop() {
            SpilUnityImplementationBase.fireSplashScreenOpenShop();

            Destroy(SplashScreen);
        }

        public void IAPPurchaseRequest() {
            if (Spil.IapPurchaseRequest == null || Spil.IapPurchaseRequest.Equals("")) {
                SpilLogging.Error("Iap Purchase Request SKU Id not set! Please configure value in the Spil SDK object!");
            } else {
                SpilUnityImplementationBase.fireIAPRequestPurchase(Spil.IapPurchaseRequest);
            }

            Destroy(SplashScreen);
        }

        public void CloseDailyBonus() {
            SpilUnityImplementationBase.fireDailyBonusClosed();

            Destroy(DailyBonus);
        }

        public void CollectReward() {
            if (rewardType == Spil.DailyBonusRewardTypeEnum.EXTERNAL) {
                List<Reward> rewards = new List<Reward>();

                Reward reward = new Reward();
                reward.externalId = Spil.DailyBonusExternalId;
                reward.amount = Spil.DailyBonusAmount;

                rewards.Add(reward);

                string rewardsJSON = JsonHelper.getJSONFromObject(rewards);

                JSONObject json = new JSONObject();
                json.AddField("data", rewardsJSON);

                SpilUnityImplementationBase.fireDailyBonusReward(json.Print(false));
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

            SpilUnityImplementationBase.fireDailyBonusClosed();
            
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
                url = response.data.GetField("url").Print(false);
            }

            if (response.action.ToLower().Trim().Equals("show")) {
                if (response.eventName.ToLower().Equals("splashscreen")) {
                    OverlayManager.ShowSplashScreen(response.data, url);
                }
                else if (response.eventName.ToLower().Equals("dailybonus")) {
                    OverlayManager.ShowDailyBonus(response.data, url);
                }
            }
            else if (response.action.ToLower().Trim().Equals("notavailable")) {
                if (response.eventName.ToLower().Equals("splashscreen")) {
                    SpilUnityImplementationBase.fireSplashScreenNotAvailable();
                }
                else if (response.eventName.ToLower().Equals("dailybonus")) {
                    SpilUnityImplementationBase.fireDailyBonusNotAvailable();
                }
            }
        }
    }
}
#endif