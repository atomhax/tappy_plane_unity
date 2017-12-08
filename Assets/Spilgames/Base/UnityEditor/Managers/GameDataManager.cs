#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Helpers;
using System.IO;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Json;

namespace SpilGames.Unity.Base.UnityEditor.Managers {
    public class GameDataManager {
        public List<SpilCurrencyData> currencies;
        public List<SpilItemData> items;
        public List<SpilBundleData> bundles;
        public List<SpilShopTabData> shop;
        public List<SpilShopPromotionData> promotions;

        public static bool updatedFromServer;
        
        public string GetGameObjects() {
            if (!updatedFromServer) {
                try {
                    string gameData = File.ReadAllText(Application.streamingAssetsPath + "/defaultGameData.json");
                    return gameData;
                }
                catch (FileNotFoundException e) {
                    SpilLogging.Log("defaultGameData.json not found. Creating a placeholder!" + e.ToString());
                    string placeholder =
                        "{\"currencies\": [],\"promotions\": [],\"shop\": [],\"bundles\": [],\"items\": []}";
                    return placeholder;
                }
            }
            return JsonHelper.getJSONFromObject(this);
        }

        public GameDataManager InitialiseGameObjects() {
            string gameData = GetGameObjects();

            return JsonHelper.getObjectFromJson<GameDataManager>(gameData);
        }
    }
    
    public class GameDataResponse : Response {
        public static void ProcessGameDataResponse(ResponseEvent response) {
            if (response.data.HasField("currencies")) {
                if (SpilUnityEditorImplementation.gData.currencies == null) {
                    SpilUnityEditorImplementation.gData.currencies = new List<SpilCurrencyData>();
                }

                if (SpilUnityEditorImplementation.gData.currencies.Count > 0) {
                    SpilUnityEditorImplementation.gData.currencies.Clear();
                }

                JSONObject currenciesJSON = response.data.GetField("currencies");

                for (int i = 0; i < currenciesJSON.Count; i++) {
                    SpilCurrencyData currency =
                        JsonHelper.getObjectFromJson<SpilCurrencyData>(currenciesJSON.list[i].Print(false));
                    SpilUnityEditorImplementation.gData.currencies.Add(currency);
                }
            }

            if (response.data.HasField("items")) {
                if (SpilUnityEditorImplementation.gData.items == null) {
                    SpilUnityEditorImplementation.gData.items = new List<SpilItemData>();
                }

                if (SpilUnityEditorImplementation.gData.items.Count > 0) {
                    SpilUnityEditorImplementation.gData.items.Clear();
                }

                JSONObject itemsJSON = response.data.GetField("items");

                for (int i = 0; i < itemsJSON.Count; i++) {
                    SpilItemData item = JsonHelper.getObjectFromJson<SpilItemData>(itemsJSON.list[i].Print(false));
                    SpilUnityEditorImplementation.gData.items.Add(item);
                }
            }

            if (response.data.HasField("bundles")) {
                if (SpilUnityEditorImplementation.gData.bundles == null) {
                    SpilUnityEditorImplementation.gData.bundles = new List<SpilBundleData>();
                }

                if (SpilUnityEditorImplementation.gData.bundles.Count > 0) {
                    SpilUnityEditorImplementation.gData.bundles.Clear();
                }

                JSONObject bundlesJSON = response.data.GetField("bundles");

                for (int i = 0; i < bundlesJSON.Count; i++) {
                    SpilBundleData bundle =
                        JsonHelper.getObjectFromJson<SpilBundleData>(bundlesJSON.list[i].Print(false));
                    SpilUnityEditorImplementation.gData.bundles.Add(bundle);
                }
            }

            if (response.data.HasField("promotions")) {
                if (SpilUnityEditorImplementation.gData.promotions == null) {
                    SpilUnityEditorImplementation.gData.promotions = new List<SpilShopPromotionData>();
                }

                if (SpilUnityEditorImplementation.gData.promotions.Count > 0) {
                    SpilUnityEditorImplementation.gData.promotions.Clear();
                }

                JSONObject promotionsJSON = response.data.GetField("promotions");

                for (int i = 0; i < promotionsJSON.Count; i++) {
                    SpilShopPromotionData bundle =
                        JsonHelper.getObjectFromJson<SpilShopPromotionData>(promotionsJSON.list[i].Print(false));
                    SpilUnityEditorImplementation.gData.promotions.Add(bundle);
                }
            }
            
            if (response.data.HasField("shop")) {
                if (SpilUnityEditorImplementation.gData.shop == null) {
                    SpilUnityEditorImplementation.gData.shop = new List<SpilShopTabData>();
                }

                if (SpilUnityEditorImplementation.gData.shop.Count > 0) {
                    SpilUnityEditorImplementation.gData.shop.Clear();
                }

                JSONObject shopJSON = response.data.GetField("shop");

                for (int i = 0; i < shopJSON.Count; i++) {
                    SpilShopTabData tab = JsonHelper.getObjectFromJson<SpilShopTabData>(shopJSON.list[i].Print(false));

                    if (SpilUnityEditorImplementation.gData.promotions != null) {
                        foreach (SpilShopEntryData entryData in tab.entries) {
                            foreach (SpilShopPromotionData shopPromotionData in SpilUnityEditorImplementation.gData.promotions) {
                                if (entryData.bundleId == shopPromotionData.bundleId) {
                                    tab.hasActivePromotions = true;
                                }
                            }
                        }
                    }                 
                    
                    SpilUnityEditorImplementation.gData.shop.Add(tab);
                }
            }

            GameDataManager.updatedFromServer = true;

            Spil.GameData.RefreshData(Spil.Instance);
            SpilUnityImplementationBase.fireSpilGameDataAvailable();
        }

    }

}
#endif