#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Json;
using SpilGames.Unity.Base.UnityEditor.Managers;

namespace SpilGames.Unity.Base.UnityEditor {
    public class ResponseEvent : SpilEvent {
        public string action;
        public string type;

        public static void Build(JSONObject response) {
            ResponseEvent responseEvent = Spil.MonoInstance.gameObject.AddComponent<ResponseEvent>();

            responseEvent.eventName = response.GetField("name").str;

            if (response.HasField("type")) {
                responseEvent.type = response.GetField("type").str;
            }

            if (response.HasField("action")) {
                responseEvent.action = response.GetField("action").str;
            }

            if (response.HasField("data")) {
                responseEvent.data = response.GetField("data");
            }

            if (responseEvent.type != null) {
                switch (responseEvent.type.ToLower().Trim()) {
                    case "advertisement":
                        AdvertisementResponse.ProcessAdvertisementResponse(responseEvent);
                        break;
                    case "overlay":
                        OverlayResponse.ProcessOverlayResponse(responseEvent);
                        break;
                    case "gameconfig":
                        ConfigResponse.ProcessConfigResponse(responseEvent);
                        break;
                    case "packages":
                        PackagesResponse.ProcessPackagesResponse(responseEvent);
                        break;
                    case "notification":
                        break;
                    case "gamedata":
                        GameDataResponse.ProcessGameDataResponse(responseEvent);
                        break;
                    case "promotions":
                        PromotionsResponse.ProcessPromotionResponse(responseEvent);
                        break;
                    case "userdata":
                        UserDataResponse.ProcessUserDataResponse(responseEvent);
                        break;
                    case "playerdata":
                        PlayerDataResponse.ProcessPlayerDataResponse(responseEvent);
                        break;
                    case "gamestate":
                        GameStateResponse.ProcessGameStateResponse(responseEvent);
                        break;
                    case "reward":
                        RewardResponse.ProcessRewardResponse(responseEvent);
                        break;
                    case "liveevent":
                        LiveEventResponse.ProcessLiveEventResponse(responseEvent);
                        break;
                    case "authentication":
                        SocialLoginResponse.ProcessSocialLoginResponse(responseEvent);
                        break;
                }
            }
            GameObject.Destroy(responseEvent);
        }
    }
    
    public class Response {
        public static string provider;
        public static string externalId;
        public static string pluginName;
        public static string pluginVersion;
    }
}
#endif