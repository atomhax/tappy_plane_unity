using UnityEngine;
using System.Collections;
using SpilGames.Unity.Implementations;
using SpilGames.Unity.Utils.UnityEditor;

namespace SpilGames.Unity.Utils.UnityEditor.Responses
{

	public class ConfigData : Data
	{
		#if UNITY_EDITOR

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

		#endif
	
	}

}


