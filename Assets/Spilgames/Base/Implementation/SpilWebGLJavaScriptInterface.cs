using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SpilGames.Unity.Json;
using SpilGames.Unity.Base.Implementations;
#if UNITY_EDITOR || UNITY_WEBGL
using SpilGames.Unity.Base.UnityEditor.Managers;
#endif
using AOT;

namespace SpilGames.Unity.Base.Implementations
{
    public class SpilWebGLJavaScriptInterface : MonoBehaviour
    {
        #if UNITY_WEBGL

        // JavaScript methods called from Unity

        public string GetDeviceIdWebGL()
        {
            return GetDeviceIdJS();
        }
        
        [DllImport("__Internal")]
        public static extern string GetDeviceIdJS();

        static enumSplashScreenTypeWebGL splashScreenTypeWebGL = enumSplashScreenTypeWebGL.SPLASH_SCREEN;
        public static JSONObject iapDetailsWebGL = new JSONObject("{}");

        public enum enumSplashScreenTypeWebGL
        {
            DAILY_BONUS,
            SPLASH_SCREEN,
            TIERED_EVENT,
            LIVE_EVENT
        }

        public static void OpenUrlInNewWindowWebGL(string url)
        {
            OpenUrlInNewWindowJS(url);
        }

        [DllImport("__Internal")]
        public static extern void OpenUrlInNewWindowJS(string url);

        public static void OpenSplashScreenUrlWebGL(string url, JSONObject data, enumSplashScreenTypeWebGL splashScreenType)
        {
            SpilWebGLJavaScriptInterface.splashScreenTypeWebGL = splashScreenType;

            JSONObject dataToSend = new JSONObject("{}");

            if(data == null)
            {
                data = new JSONObject("{}");
            }

            dataToSend.AddField("eventData", data);

            dataToSend.AddField("iapDetails", toSnakeCaseWebGL(iapDetailsWebGL));
            dataToSend.AddField("liveEventDetails", new JSONObject("{}")); // TODO: Where to fetch this from?
            dataToSend.AddField("tieredEvents", toSnakeCaseWebGL(new JSONObject(JsonHelper.getJSONFromObject(TieredEventManager.GetTieredEventsOverview().tieredEvents.Values.ToList()))));

            JSONObject spilgameDataJson = new JSONObject();
            spilgameDataJson.AddField("items", toSnakeCaseWebGL(new JSONObject(JsonHelper.getJSONFromObject(Spil.GameData.Items))));
            spilgameDataJson.AddField("currencies", toSnakeCaseWebGL(new JSONObject(JsonHelper.getJSONFromObject(Spil.GameData.Currencies))));
            spilgameDataJson.AddField("bundles", toSnakeCaseWebGL(new JSONObject(JsonHelper.getJSONFromObject(Spil.GameData.Bundles))));
            spilgameDataJson.AddField("shop", toSnakeCaseWebGL(new JSONObject(JsonHelper.getJSONFromObject(Spil.GameData.Shop))));
            dataToSend.AddField("gameData", spilgameDataJson);

            JSONObject spilWalletJson = new JSONObject();
            spilWalletJson.AddField("currencies", toSnakeCaseWebGL(new JSONObject(JsonHelper.getJSONFromObject(Spil.PlayerData.Wallet.Currencies))));
            dataToSend.AddField("wallet", spilWalletJson);

            JSONObject spilInventoryJson = new JSONObject();
            spilInventoryJson.AddField("items", toSnakeCaseWebGL(new JSONObject(JsonHelper.getJSONFromObject(Spil.PlayerData.Inventory.Items))));
            dataToSend.AddField("inventory", spilInventoryJson);

            OpenSplashScreenUrlJS(url, dataToSend.ToString());
        }

        [DllImport("__Internal")]
        public static extern void OpenSplashScreenUrlJS(string url, string data);

        static JSONObject toSnakeCaseWebGL(JSONObject inputObject)
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
                        value = toSnakeCaseWebGL(value);
                        returnObject.AddField(keyNameSnakeCase, value);
                    }
                    else if (value.IsArray)
                    {
                        List<JSONObject> newValue = new List<JSONObject>();
                        for (int a = 0; a < value.list.Count; a++)
                        {
                            newValue.Add(toSnakeCaseWebGL(value.list[a]));
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
                    newValue.Add(toSnakeCaseWebGL(inputObject.list[a]));
                }
                returnObject = new JSONObject(newValue.ToArray());
            } else {
                returnObject = inputObject;
            }

            return returnObject;
        }

        public static void GiveWebGLPlayerFocus()
        {
            GivePlayerFocusJS();
        }

        [DllImport("__Internal")]
        public static extern void GivePlayerFocusJS();

        public static void SendNativeMessageWebGL(string message, JSONObject data)
        {
            NativeMessageJS(message, toSnakeCaseWebGL(data).ToString());
        }

        [DllImport("__Internal")]
        public static extern void NativeMessageJS(string message, string data);

        // Unity methods called from JavaScript

        public void CloseSplashScreenWebGL()
        {
            switch(splashScreenTypeWebGL)
            {
                case enumSplashScreenTypeWebGL.SPLASH_SCREEN:
                    Spil.Instance.fireSplashScreenClosed();
                    break;
                case enumSplashScreenTypeWebGL.DAILY_BONUS:
                    Spil.Instance.fireDailyBonusClosed();
                    break;
                case enumSplashScreenTypeWebGL.TIERED_EVENT:
                    Spil.Instance.fireTieredEventProgressClosed();
                    break;
            }
        }

        public void OpenGameShopWebGL()
        {
            Spil.Instance.fireSplashScreenOpenShop();
        }

        public void CollectWebGL() // Only for daily bonus with non-external reward
        {
            Spil.Instance.SendCustomEventInternal("collectDailyBonus", null);
        }

        public void ReceiveRewardWebGL(string paramsJson) // Only for daily bonus with external reward
        {
            Spil.Instance.fireDailyBonusReward(paramsJson);
        }

        public void SendEventWebGL(string paramsJson)
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

        public void IapPurchaseRequestWebGL(string paramsJson)
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

        public void BuyBundleWebGL(string paramsJson)
        {
            JSONObject paramsJsonObject = new JSONObject(paramsJson);
            Spil.Instance.BuyBundle((int)paramsJsonObject.GetField("bundleId").n, paramsJsonObject.HasField("reason") ? paramsJsonObject.GetField("reason").str : null, paramsJsonObject.HasField("location") ? paramsJsonObject.GetField("location").str : null, paramsJsonObject.HasField("reasonDetails") ? paramsJsonObject.GetField("reasonDetails").str : null);
        }

        public void OpenBundleWebGL(string paramsJson)
        {
            JSONObject paramsJsonObject = new JSONObject(paramsJson);
            SpilWebGLUnityImplementation.pData.OpenBundle((int)paramsJsonObject.GetField("bundleId").n, (int)paramsJsonObject.GetField("amount").n, paramsJsonObject.GetField("reason").str, paramsJsonObject.GetField("reasonDetails").str, paramsJsonObject.GetField("location").str, null);
        }

        public void OpenGachaWebGL(string paramsJson)
        {
            JSONObject paramsJsonObject = new JSONObject(paramsJson);
            Spil.Instance.OpenGacha((int)paramsJsonObject.GetField("gachaId").n, paramsJsonObject.HasField("reason") ? paramsJsonObject.GetField("reason").str : null, paramsJsonObject.HasField("location") ? paramsJsonObject.GetField("location").str : null, paramsJsonObject.HasField("reasonDetails") ? paramsJsonObject.GetField("reasonDetails").str : null);
        }

        public void AddItemWebGL(string paramsJson)
        {
            JSONObject paramsJsonObject = new JSONObject(paramsJson);

            if ((int)paramsJsonObject.GetField("amount").i > 0)
            {
                Spil.Instance.AddItemToInventory((int)paramsJsonObject.GetField("itemId").i, (int)paramsJsonObject.GetField("amount").i, paramsJsonObject.GetField("reason").str, paramsJsonObject.GetField("reasonDetails").str, paramsJsonObject.GetField("location").str, null);
            } else {
                Spil.Instance.SubtractItemFromInventory((int)paramsJsonObject.GetField("itemId").i, (int)paramsJsonObject.GetField("amount").i, paramsJsonObject.GetField("reason").str, paramsJsonObject.GetField("reasonDetails").str, paramsJsonObject.GetField("location").str, null);
            }
        }

        public void AddCurrencyWebGL(string paramsJson)
        {
            JSONObject paramsJsonObject = new JSONObject(paramsJson);

            if ((int)paramsJsonObject.GetField("amount").i > 0)
            {
                Spil.Instance.AddCurrencyToWallet((int)paramsJsonObject.GetField("currencyId").i, (int)paramsJsonObject.GetField("amount").i, paramsJsonObject.GetField("reason").str, paramsJsonObject.GetField("reasonDetails").str, paramsJsonObject.GetField("location").str, null);
            } else {
                Spil.Instance.SubtractCurrencyFromWallet((int)paramsJsonObject.GetField("currencyId").i, (int)paramsJsonObject.GetField("amount").i, paramsJsonObject.GetField("reason").str, paramsJsonObject.GetField("reasonDetails").str, paramsJsonObject.GetField("location").str, null);
            }
        }

        public void SendDataToGameWebGL(string paramsJson)
        {
            Spil.Instance.fireSplashScreenData(paramsJson);
        }

        // Live events 
        // TODO: Implement and test live events!
         
        public void OpenLiveEventNextStageWebGL()
        {
            //LiveEventManager.ProcessAdvanceToNextStage(); // TODO: Implement this?
        }

        public void ApplyLiveEventItemsWebGL(string paramsJson)
        {
            LiveEventManager.ProcessApplyItems(new JSONObject(paramsJson));
        }

        // Tiered events

        public void ClaimTierRewardWebGL(string paramsJson)
        {
            JSONObject paramsJsonObject = new JSONObject(paramsJson);
            TieredEventManager.ClaimTierReward((int)paramsJsonObject.GetField("tieredEventId").i, (int)paramsJsonObject.GetField("tierId").i);
        }

        // AppLixir

        public delegate void SimpleCallback(int val);

        [DllImport("__Internal")]
        public static extern void ShowAppLixirVideoWebGL(int devId, int gameId, int zoneId, int fallback, SimpleCallback onCompleted);

        [MonoPInvokeCallback(typeof(SimpleCallback))]

        private static void ApplixirCompletedHandler(int result)
        {
            Debug.Log("ApplixirCompletedHandler GOT VIDEO RESULT CALLBACK: " + result);

            PlayVideoResult pvr = (PlayVideoResult)result;

            if (callbackAppLixirWebGL != null)
            {
                callbackAppLixirWebGL(pvr);
            }
        }

        public enum PlayVideoResult
        {
            NONE = 0,
            AD_WATCHED = 1, // an ad was presented and ran for more than 5 Seconds
            NETWORK_ERROR = 2, // no connectivity available
            AD_BLOCKER = 3, // an ad blocker was detected
            AD_INTERRUPTED = 4, // ad was ended prior to 5 seconds (abnormal end)
            ADS_UNAVAILABLE = 5, // no ads were returned to the player
            AD_FALLBACK = 6, // fallback mode displayed a banner in response to ads-unavailable.Will only occur if "fallback:1" is set in the options.
            CORS_ERROR = 7,
            NO_ZONEID = 8,
            AD_STARTED = 9,
            SYS_CLOSING = 10
        }

        private static Action<PlayVideoResult> callbackAppLixirWebGL = null;

        /// <summary>
        /// Calls out to the applixir service to show a video ad.
        /// Result is returned via the resultListerens event.
        /// </summary>
        public static void PlayAppLixirVideoWebGL(Action<PlayVideoResult> callback)
        {
            SpilWebGLJavaScriptInterface.callbackAppLixirWebGL = callback;
            ShowAppLixirVideoWebGL(devId, gameId, zoneId, (fallback ? 1 : 0), ApplixirCompletedHandler);
        }

        private static int devId;
        private static int gameId;
        private static int zoneId;
        private static bool fallback;

        public static void initAppLixirWebGL(int devId, int gameId, int zoneId, bool fallback)
        {
            SpilWebGLJavaScriptInterface.devId = devId;
            SpilWebGLJavaScriptInterface.gameId = gameId;
            SpilWebGLJavaScriptInterface.zoneId = zoneId;
            SpilWebGLJavaScriptInterface.fallback = fallback;
        }

        /*
        public static void onPlayVideoResultString(string result)
        {
            Debug.Log("onPlayVideoResultString GOT VIDEO RESULT CALLBACK: " + result);
            PlayVideoResult pvr = PlayVideoResult.ADS_UNAVAILABLE;

            if (!string.IsNullOrEmpty(result))
            {
                result = result.ToLower().Trim();
                switch (result)
                {
                    case "ad-watched":
                        pvr = PlayVideoResult.AD_WATCHED;
                    break;
                    case "network-error":
                        pvr = PlayVideoResult.NETWORK_ERROR;
                    break;
                    case "ad-blocker":
                        pvr = PlayVideoResult.AD_BLOCKER;
                    break;
                    case "ad-interrupted":
                        pvr = PlayVideoResult.AD_INTERRUPTED;
                    break;
                    case "ads-unavailable":
                        pvr = PlayVideoResult.ADS_UNAVAILABLE;
                    break;
                    case "ad-fallback":
                        pvr = PlayVideoResult.AD_FALLBACK;
                    break;
                    case "cors - error":
                        pvr = PlayVideoResult.CORS_ERROR;
                    break;
                    case "no_zoneId":
                        pvr = PlayVideoResult.NO_ZONEID;
                    break;
                    case "ad - started":
                        pvr = PlayVideoResult.AD_STARTED;
                    break;
                    case "sys - closing":
                        pvr = PlayVideoResult.SYS_CLOSING;
                    break;
                    default:
                        pvr = PlayVideoResult.ADS_UNAVAILABLE;
                    break;
                }
            }

            if (callback != null)
            {
                callback(pvr);
            }
        }
        */

        #endif
    }
}