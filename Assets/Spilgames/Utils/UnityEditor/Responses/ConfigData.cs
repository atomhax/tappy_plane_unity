#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using SpilGames.Unity.Implementations;

namespace SpilGames.Unity.Utils.UnityEditor.Responses
{

	public class ConfigData : Data
	{

		public static string GameConfigData;

		public static void ProcessConfig(ResponseEvent response){
			if(response.data != null){
				GameConfigData = response.data.Print(false);
				SpilUnityImplementationBase.fireConfigUpdatedEvent();
			}
		}

		public static string getConfigAll(){
			if(GameConfigData == null){
				return loadGameConfigFromAssets();
			}

			return GameConfigData;
		}

		public static string getConfigValue(string key){
			JSONObject config = new JSONObject(GameConfigData);

			if(config.HasField("key")){
				return config.GetField("key").Print(false);
			}

			return null;

		}

		private static string loadGameConfigFromAssets(){
			string config = System.IO.File.ReadAllText (Application.streamingAssetsPath + "/defaultGameConfig.json");

			return config;
		}


	
	}

}
#endif