#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Helpers;
using System.IO;
using SpilGames.Unity.Base.SDK;
using SpilGames.Unity.Json;

namespace SpilGames.Unity.Base.UnityEditor.Responses {
    public class GameDataResponse : Response {
        public List<SpilCurrencyData> currencies;
        public List<SpilItemData> items;
        public List<SpilBundleData> bundles;
        public List<SpilShopTabData> shop;
        public List<SpilShopPromotionData> promotions;

        private bool updatedFromServer;

        public void ProcessGameDataResponse(ResponseEvent response) {
            if (response.data.HasField("currencies")) {
                if (currencies == null) {
                    currencies = new List<SpilCurrencyData>();
                }

                if (currencies.Count > 0) {
                    currencies.Clear();
                }

                JSONObject currenciesJSON = response.data.GetField("currencies");

                for (int i = 0; i < currenciesJSON.Count; i++) {
                    SpilCurrencyData currency =
                        JsonHelper.getObjectFromJson<SpilCurrencyData>(currenciesJSON.list[i].Print(false));
                    currencies.Add(currency);
                }
            }

            if (response.data.HasField("items")) {
                if (items == null) {
                    items = new List<SpilItemData>();
                }

                if (items.Count > 0) {
                    items.Clear();
                }

                JSONObject itemsJSON = response.data.GetField("items");

                for (int i = 0; i < itemsJSON.Count; i++) {
                    SpilItemData item = JsonHelper.getObjectFromJson<SpilItemData>(itemsJSON.list[i].Print(false));
                    items.Add(item);
                }
            }

            if (response.data.HasField("bundles")) {
                if (bundles == null) {
                    bundles = new List<SpilBundleData>();
                }

                if (bundles.Count > 0) {
                    bundles.Clear();
                }

                JSONObject bundlesJSON = response.data.GetField("bundles");

                for (int i = 0; i < bundlesJSON.Count; i++) {
                    SpilBundleData bundle =
                        JsonHelper.getObjectFromJson<SpilBundleData>(bundlesJSON.list[i].Print(false));
                    bundles.Add(bundle);
                }
            }

            if (response.data.HasField("shop")) {
                if (shop == null) {
                    shop = new List<SpilShopTabData>();
                }

                if (shop.Count > 0) {
                    shop.Clear();
                }

                JSONObject shopJSON = response.data.GetField("shop");

                for (int i = 0; i < shopJSON.Count; i++) {
                    SpilShopTabData tab = JsonHelper.getObjectFromJson<SpilShopTabData>(shopJSON.list[i].Print(false));
                    shop.Add(tab);
                }
            }

            if (response.data.HasField("promotions")) {
                if (promotions == null) {
                    promotions = new List<SpilShopPromotionData>();
                }

                if (promotions.Count > 0) {
                    promotions.Clear();
                }

                JSONObject promotionsJSON = response.data.GetField("promotions");

                for (int i = 0; i < promotionsJSON.Count; i++) {
                    SpilShopPromotionData bundle =
                        JsonHelper.getObjectFromJson<SpilShopPromotionData>(promotionsJSON.list[i].Print(false));
                    promotions.Add(bundle);
                }
            }

            updatedFromServer = true;

            Spil.GameData.RefreshData(Spil.Instance);
            SpilUnityImplementationBase.fireSpilGameDataAvailable();
        }

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

        public GameDataResponse InitialiseGameObjects() {
            string gameData = GetGameObjects();

            return JsonHelper.getObjectFromJson<GameDataResponse>(gameData);
        }
    }
}
#endif