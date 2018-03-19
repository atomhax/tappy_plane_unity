#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Json;

namespace SpilGames.Unity.Base.UnityEditor.Managers {
    public class ConfigManager {
        public static string GameConfigData;
        
        public static string getConfigAll() {
            return GameConfigData ?? loadGameConfigFromAssets();
        }

        public static string getConfigValue(string key) {
            JSONObject config = new JSONObject(GameConfigData);

            return config.HasField(key) ? config.GetField(key).Print(false) : null;
        }

        private static string loadGameConfigFromAssets() {
            string config = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/defaultGameConfig.json");

            return config;
        }
    }
    
    public class ConfigResponse : Response {
        public static void ProcessConfigResponse(ResponseEvent response) {
            if (response.data == null) return;
            ConfigManager.GameConfigData = response.data.Print();
            SpilUnityImplementationBase.fireConfigUpdatedEvent();
        }

    }
}
#endif