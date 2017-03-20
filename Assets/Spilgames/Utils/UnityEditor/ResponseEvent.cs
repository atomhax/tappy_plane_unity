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
						AdvertisementResponse.ProcessAdvertisementResponse(responseEvent);
						break;
					case "overlay":
						OverlayResponse.ProcessOverlayResponse(responseEvent);
						break;
					case "gameconfig":
						ConfigResponse.ProcessConfigResponse(responseEvent);
						break;
					case "packages":
						PackagesResponse.ProcessPackagesResponse (responseEvent);
						break;
					case "notification":
						break;
					case "playerdata":
						SpilUnityEditorImplementation.pData.ProcessPlayerDataResponse(responseEvent);
						break;
					case "gamedata":
						SpilUnityEditorImplementation.gData.ProcessGameObjectsResponse(responseEvent);
						break;
					case "gamestate":
						GameStateResponse.ProcessGameStateResponse (responseEvent);
						break;
					}
			}
			GameObject.Destroy(responseEvent);
		}
	}
}
#endif