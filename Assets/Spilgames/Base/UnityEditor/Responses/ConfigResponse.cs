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
			if (response.data == null) return;
			GameConfigData = response.data.Print();
			SpilUnityImplementationBase.fireConfigUpdatedEvent();
		}

		public static string getConfigAll()
		{
			return GameConfigData ?? loadGameConfigFromAssets();
		}

		public static string getConfigValue(string key)
		{
			JSONObject config = new JSONObject(GameConfigData);

			return config.HasField(key) ? config.GetField(key).Print(false) : null;
		}

		private static string loadGameConfigFromAssets()
		{
			string config = System.IO.File.ReadAllText (Application.streamingAssetsPath + "/defaultGameConfig.json");

			return config;
		}
	}

}
#endif