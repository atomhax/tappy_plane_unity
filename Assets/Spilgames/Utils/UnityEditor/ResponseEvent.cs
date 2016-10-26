#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using SpilGames.Unity.Utils.UnityEditor.Responses;
using SpilGames.Unity.Implementations;

namespace SpilGames.Unity.Utils.UnityEditor
{
	public class ResponseEvent : SpilEvent
	{

		public string action;
		public string type;

		public static void Build (JSONObject response)
		{

			ResponseEvent responseEvent = Spil.MonoInstance.gameObject.AddComponent<ResponseEvent> ();

			responseEvent.eventName = response.GetField ("name").str;

			if (response.HasField ("type")) {
				responseEvent.type = response.GetField ("type").str;
			}

			if (response.HasField ("action")) {
				responseEvent.action = response.GetField ("action").str;
			}

			if (response.HasField ("data")) {
				responseEvent.data = response.GetField ("data");
			}

			if (responseEvent.type != null) {
				switch (responseEvent.type.ToLower ().Trim ()) {
					case "advertisement":
						AdvertisementData.ProcessAdvertisement(responseEvent);
						break;
					case "overlay":
						break;
					case "gameconfig":
						ConfigData.ProcessConfig(responseEvent);
						break;
					case "packages":
						PackagesData.ProcessPackagesData (responseEvent);
						break;
					case "notification":
						break;
					case "playerdata":
						SpilUnityEditorImplementation.pData.ProcessPlayerData(responseEvent);
						break;
					case "gamedata":
						SpilUnityEditorImplementation.gData.ProcessGameObjects(responseEvent);
						break;
					case "gamestate":
						GameStateData.ProcessGameState (responseEvent);
						break;
					case "reward":
						break;
					}
			}

		}
	}
}
#endif