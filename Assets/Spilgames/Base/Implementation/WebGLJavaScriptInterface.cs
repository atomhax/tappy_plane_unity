using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SpilGames.Unity;
using SpilGames.Unity.Json;
using SpilGames.Unity.Base.UnityEditor.Managers;
using SpilGames.Unity.Base.Implementations;

namespace SpilGames.Unity.Base.Implementations
{
    public class WebGLJavaScriptInterface : MonoBehaviour
    {
        #if UNITY_WEBGL

        // JavaScript methods called from Unity

        static enumSplashScreenType splashScreenType = enumSplashScreenType.SPLASH_SCREEN;
        public static JSONObject iapDetails = new JSONObject("{}");

        public enum enumSplashScreenType
        {
            DAILY_BONUS,
            SPLASH_SCREEN,
            TIERED_EVENT,
            LIVE_EVENT
        }

        public static void OpenUrl(string url, JSONObject data, enumSplashScreenType splashScreenType)
        {
            WebGLJavaScriptInterface.splashScreenType = splashScreenType;

            JSONObject dataToSend = new JSONObject("{}");

            if(data == null)
            {
                data = new JSONObject("{}");
            }

            dataToSend.AddField("eventData", data);

            dataToSend.AddField("iapDetails", toSnakeCase(iapDetails));
            dataToSend.AddField("liveEventDetails", new JSONObject("{}")); // TODO: Where to fetch this from?
            dataToSend.AddField("tieredEvents", toSnakeCase(new JSONObject(JsonHelper.getJSONFromObject(TieredEventManager.GetTieredEventsOverview().tieredEvents.Values.ToList()))));

            JSONObject spilgameDataJson = new JSONObject();
            spilgameDataJson.AddField("items", toSnakeCase(new JSONObject(JsonHelper.getJSONFromObject(Spil.GameData.Items))));
            spilgameDataJson.AddField("currencies", toSnakeCase(new JSONObject(JsonHelper.getJSONFromObject(Spil.GameData.Currencies))));
            spilgameDataJson.AddField("bundles", toSnakeCase(new JSONObject(JsonHelper.getJSONFromObject(Spil.GameData.Bundles))));
            spilgameDataJson.AddField("shop", toSnakeCase(new JSONObject(JsonHelper.getJSONFromObject(Spil.GameData.Shop))));
            dataToSend.AddField("gameData", spilgameDataJson);

            JSONObject spilWalletJson = new JSONObject();
            spilWalletJson.AddField("currencies", toSnakeCase(new JSONObject(JsonHelper.getJSONFromObject(Spil.PlayerData.Wallet.Currencies))));
            dataToSend.AddField("wallet", spilWalletJson);

            JSONObject spilInventoryJson = new JSONObject();
            spilInventoryJson.AddField("items", toSnakeCase(new JSONObject(JsonHelper.getJSONFromObject(Spil.PlayerData.Inventory.Items))));
            dataToSend.AddField("inventory", spilInventoryJson);

            OpenUrlWithData(url, dataToSend.ToString());
        }

        static JSONObject toSnakeCase(JSONObject inputObject)
        {
            JSONObject returnObject = new JSONObject("{}");

            if(inputObject.IsObject)
            {
                List<string> keys = new List<string>(inputObject.keys);
                foreach (string key in keys)
                {
                    string keyNameSnakeCase = key.Substring(0, 1).ToLower() + (key.Length > 1 ? key.Substring(1) : "");
                    JSONObject value = inputObject.GetField(key);
                    if (value.IsObject)
                    {
                        value = toSnakeCase(value);
                        returnObject.AddField(keyNameSnakeCase, value);
                    }
                    else if (value.IsArray)
                    {
                        List<JSONObject> newValue = new List<JSONObject>();
                        for (int a = 0; a < value.list.Count; a++)
                        {
                            newValue.Add(toSnakeCase(value.list[a]));
                        }
                        returnObject.AddField(keyNameSnakeCase, new JSONObject(newValue.ToArray()));
                    } else {
                        returnObject.AddField(keyNameSnakeCase, value);
                    }
                }
            }
            else if(inputObject.IsArray)
            {
                List<JSONObject> newValue = new List<JSONObject>();
                for (int a = 0; a < inputObject.list.Count; a++)
                {
                    newValue.Add(toSnakeCase(inputObject.list[a]));
                }
                returnObject = new JSONObject(newValue.ToArray());
            } else {
                returnObject = inputObject;
            }

            return returnObject;
        }

        [DllImport("__Internal")]
        public static extern void OpenUrlWithData(string url, string data);

        public static void SendNativeMessage(string message, JSONObject data)
        {
            NativeMessage(message, toSnakeCase(data).ToString());
        }

        [DllImport("__Internal")]
        public static extern void NativeMessage(string message, string data);

        // Unity methods called from JavaScript

        public void CloseSplashScreen()
        {
            switch(splashScreenType)
            {
                case enumSplashScreenType.SPLASH_SCREEN:
                    Spil.Instance.fireSplashScreenClosed();
                    break;
                case enumSplashScreenType.DAILY_BONUS:
                    Spil.Instance.fireDailyBonusClosed();
                    break;
                case enumSplashScreenType.TIERED_EVENT:
                    Spil.Instance.fireTieredEventProgressClosed();
                    break;
            }
        }

        public void OpenGameShop()
        {
            Spil.Instance.fireSplashScreenOpenShop();
        }

        public void Collect() // Only for daily bonus with non-external reward
        {
            Spil.Instance.SendCustomEventInternal("collectDailyBonus", null);
        }

        public void ReceiveReward(string paramsJson) // Only for daily bonus with external reward
        {
            Spil.Instance.fireDailyBonusReward(paramsJson);
        }

        public void SendEvent(string paramsJson)
        {
            JSONObject paramsJsonObject = new JSONObject(paramsJson);
            JSONObject payload = new JSONObject(paramsJsonObject.HasField("payload") ? paramsJsonObject.GetField("payload").str : null);
            Dictionary<string, object> paramsDict = new Dictionary<string, object>();
            if (payload != null)
            {
                foreach (KeyValuePair<string, string> a in payload.ToDictionary())
                {
                    paramsDict.Add(a.Key, a.Value);
                }
            }

            Spil.Instance.SendCustomEventInternal(paramsJsonObject.GetField("eventName").str, paramsDict);
        }

        public void IapPurchaseRequest(string paramsJson)
        {
            JSONObject paramsJsonObject = new JSONObject(paramsJson);
            string skuId = paramsJsonObject.GetField("skuId").str;

            if (skuId != null)
            {
                Spil.IapPurchaseRequest = skuId;
            }
            if (Spil.IapPurchaseRequest == null || Spil.IapPurchaseRequest.Equals(""))
            {
                SpilLogging.Error("Iap Purchase Request SKU Id not set! Please configure value in the Spil SDK object!");
            } else {
                Spil.Instance.fireIAPRequestPurchase(Spil.IapPurchaseRequest);
            }
        }

        public void BuyBundle(string paramsJson)
        {
            JSONObject paramsJsonObject = new JSONObject(paramsJson);
            Spil.Instance.BuyBundle((int)paramsJsonObject.GetField("bundleId").n, paramsJsonObject.HasField("reason") ? paramsJsonObject.GetField("reason").str : null, paramsJsonObject.HasField("location") ? paramsJsonObject.GetField("location").str : null, paramsJsonObject.HasField("reasonDetails") ? paramsJsonObject.GetField("reasonDetails").str : null);
        }

        public void OpenBundle(string paramsJson)
        {
            JSONObject paramsJsonObject = new JSONObject(paramsJson);
            SpilWebGLUnityImplementation.pData.OpenBundle((int)paramsJsonObject.GetField("bundleId").n, (int)paramsJsonObject.GetField("amount").n, paramsJsonObject.GetField("reason").str, paramsJsonObject.GetField("location").str, paramsJsonObject.GetField("reasonDetails").str);
        }

        public void OpenGacha(string paramsJson)
        {
            JSONObject paramsJsonObject = new JSONObject(paramsJson);
            Spil.Instance.OpenGacha((int)paramsJsonObject.GetField("gachaId").n, paramsJsonObject.HasField("reason") ? paramsJsonObject.GetField("reason").str : null, paramsJsonObject.HasField("location") ? paramsJsonObject.GetField("location").str : null, paramsJsonObject.HasField("reasonDetails") ? paramsJsonObject.GetField("reasonDetails").str : null);
        }

        public void AddItem(string paramsJson)
        {
            JSONObject paramsJsonObject = new JSONObject(paramsJson);

            if ((int)paramsJsonObject.GetField("amount").i > 0)
            {
                Spil.Instance.AddItemToInventory((int)paramsJsonObject.GetField("itemId").i, (int)paramsJsonObject.GetField("amount").i, paramsJsonObject.GetField("reason").str, paramsJsonObject.GetField("reasonDetails").str, paramsJsonObject.GetField("location").str, null);
            } else {
                Spil.Instance.SubtractItemFromInventory((int)paramsJsonObject.GetField("itemId").i, (int)paramsJsonObject.GetField("amount").i, paramsJsonObject.GetField("reason").str, paramsJsonObject.GetField("reasonDetails").str, paramsJsonObject.GetField("location").str, null);
            }
        }

        public void AddCurrency(string paramsJson)
        {
            JSONObject paramsJsonObject = new JSONObject(paramsJson);

            if ((int)paramsJsonObject.GetField("amount").i > 0)
            {
                Spil.Instance.AddCurrencyToWallet((int)paramsJsonObject.GetField("currencyId").i, (int)paramsJsonObject.GetField("amount").i, paramsJsonObject.GetField("reason").str, paramsJsonObject.GetField("reasonDetails").str, paramsJsonObject.GetField("location").str, null);
            } else {
                Spil.Instance.SubtractCurrencyFromWallet((int)paramsJsonObject.GetField("currencyId").i, (int)paramsJsonObject.GetField("amount").i, paramsJsonObject.GetField("reason").str, paramsJsonObject.GetField("reasonDetails").str, paramsJsonObject.GetField("location").str, null);
            }
        }

        public void SendDataToGame(string paramsJson)
        {
            Spil.Instance.fireSplashScreenData(paramsJson);
        }

        // Live events 
        // TODO: Implement and test live events!
         
        public void OpenLiveEventNextStage()
        {
            //LiveEventManager.ProcessAdvanceToNextStage(); // TODO: Implement this?
        }

        public void ApplyLiveEventItems(string paramsJson)
        {
            LiveEventManager.ProcessApplyItems(new JSONObject(paramsJson));
        }

        // Tiered events

        public void ClaimTierReward(string paramsJson)
        {
            JSONObject paramsJsonObject = new JSONObject(paramsJson);
            TieredEventManager.ClaimTierReward((int)paramsJsonObject.GetField("tieredEventId").i, (int)paramsJsonObject.GetField("tierId").i);
        }

        #endif
    }
}