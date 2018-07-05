#if UNITY_EDITOR || UNITY_WEBGL
using System.IO;
using UnityEngine;
//using System.Collections;
//using SpilGames.Unity.Base.Implementations;
//using SpilGames.Unity.Json;

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
#if UNITY_WEBGL
            string config = GameObject.FindObjectOfType<Spil>().defaultGameConfigAsset.text;
#else
            string config = File.ReadAllText(Application.streamingAssetsPath + "/defaultGameConfig.json");
#endif

            return config;
        }
    }
    
    public class ConfigResponse : Response {
        public static void ProcessConfigResponse(ResponseEvent response) {
            if (response.data == null) return;
            ConfigManager.GameConfigData = response.data.Print();
			Spil.Instance.fireConfigUpdated();
        }

    }
}
#endif