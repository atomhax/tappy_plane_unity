#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using SpilGames.Unity.Base.Implementations;
using System.Collections.Generic;
using SpilGames.Unity.Helpers;
using SpilGames.Unity.Json;
using SpilGames.Unity.Helpers.PlayerData;

namespace SpilGames.Unity.Base.UnityEditor.Responses {
    public class OverlayResponse : Response {
        private static Spil.DailyBonusRewardTypeEnum rewardType = Spil.MonoInstance.DailyBonusRewardType;

        public static void ProcessOverlayResponse(ResponseEvent response) {
            string url = null;

            if (response.data.HasField("url")) {
                url = response.data.GetField("url").Print(false);
            }

            if (response.action.ToLower().Trim().Equals("show")) {
                if (response.eventName.ToLower().Equals("splashscreen")) {
                    ShowSplashScreen(response.data, url);
                }
                else if (response.eventName.ToLower().Equals("dailybonus")) {
                    ShowDailyBonus(response.data, url);
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


        private static void ShowSplashScreen(JSONObject data, string url) {
            SpilLogging.Log("Opening URL: " + url + " With data: " + data.Print(false));

            SpilUnityImplementationBase.fireSplashScreenOpen();

            GameObject overlayObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            WebOverlay overlay = overlayObject.AddComponent<WebOverlay>();
            overlay.overlayType = "splashScreen";
        }

        private static void ShowDailyBonus(JSONObject data, string url) {
            SpilLogging.Log("Opening URL: " + url + " With data: " + data.Print(false));

            SpilUnityImplementationBase.fireDailyBonusOpen();

            GameObject overlayObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            WebOverlay overlay = overlayObject.AddComponent<WebOverlay>();
            overlay.overlayType = "dailyBonus";
        }

        public class WebOverlay : MonoBehaviour {
            public string overlayType;

            void OnGUI() {
                if (overlayType.Equals("splashScreen")) {
                    if (GUI.Button(new Rect(10, 10, (Screen.width - 20), (Screen.height - 20) / 3), "Close")) {
                        SpilUnityImplementationBase.fireSplashScreenClosed();

                        GameObject.Destroy(this.gameObject);
                    }

                    if (GUI.Button(new Rect(10, (Screen.height / 3 + 10), (Screen.width - 20), (Screen.height - 20) / 3),
                        "Open Shop")) {
                        SpilUnityImplementationBase.fireSplashScreenOpenShop();

                        GameObject.Destroy(this.gameObject);
                    }
                    
                    if (GUI.Button(new Rect(10, (2 * Screen.height / 3 + 10), (Screen.width - 20), (Screen.height - 20) / 3),
                        "IAP Purchase Request")) {
                        if (Spil.IapPurchaseRequest == null || Spil.IapPurchaseRequest.Equals("")) {
                            SpilLogging.Error("Iap Purchase Request SKU Id not set! Please configure value in the Spil SDK object!");
                        } else {
                            SpilUnityImplementationBase.fireIAPRequestPurchase(Spil.IapPurchaseRequest);  
                        }

                        GameObject.Destroy(this.gameObject);
                    }
                }
                else if (overlayType.Equals("dailyBonus")) {
                    if (GUI.Button(new Rect(10, 10, (Screen.width - 20), (Screen.height - 20) / 2), "Close")) {
                        SpilUnityImplementationBase.fireDailyBonusClosed();

                        GameObject.Destroy(this.gameObject);
                    }

                    if (GUI.Button(new Rect(10, Screen.height / 2, (Screen.width - 20), (Screen.height - 20) / 2),
                        "Collect Reward")) {
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
                        }
                        else {
                            int id = Spil.DailyBonusId;
                            int amount = Spil.DailyBonusAmount;

                            if (id == 0 || amount == 0) {
                                SpilLogging.Error("Daily Bonus Rewards not configured for Editor!");
                            }

                            if (rewardType == Spil.DailyBonusRewardTypeEnum.CURRENCY) {
                                SpilUnityEditorImplementation.pData.WalletOperation("add", id, amount, PlayerDataUpdateReasons.DailyBonus, null, "DailyBonus", null);
                            }
                            else if (rewardType == Spil.DailyBonusRewardTypeEnum.ITEM) {
                                SpilUnityEditorImplementation.pData.InventoryOperation("add", id, amount, PlayerDataUpdateReasons.DailyBonus, null, "DailyBonus", null);
                            }
                        }

                        GameObject.Destroy(this.gameObject);
                    }
                }
            }
        }

        public class Reward {
            public int id;
            public string externalId;
            public string type;
            public int amount;
        }
    }
}
#endif