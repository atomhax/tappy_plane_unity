#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Json;

namespace SpilGames.Unity.Base.UnityEditor.Responses
{

	public class ConfigResponse : Response
	{

		public static string GameConfigData;

		public static void ProcessConfigResponse(ResponseEvent response){
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

			if(config.HasField(key)){
				return config.GetField(key).Print(false);
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