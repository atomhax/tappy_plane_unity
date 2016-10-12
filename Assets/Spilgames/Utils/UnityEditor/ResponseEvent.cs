#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

namespace SpilGames.Unity.Utils.UnityEditor
{
	public class ResponseEvent : SpilEvent
	{

		public string action;
		public string type;

		public static void Build(JSONObject response){

			ResponseEvent responseEvent = new ResponseEvent();

			responseEvent.eventName = response.GetField("name").ToString();

			if(response.HasField("type")){
				responseEvent.type = response.GetField("type").ToString();
			}

			if(response.HasField("action")){
				responseEvent.action = response.GetField("action").ToString();
			}

			if(response.HasField("data")){
				responseEvent.data = response.GetField("data");
			}

			switch(responseEvent.type)
			{
				case "advertisement":
					break;
				case "overlay":
					break;
				case "gameconfig":
					break;
				case "packages":
					break;
				case "notification":
					break;
				case "playerdata":
					break;
				case "gamedata":
					break;
				case "gamestate":
					
					break;
				case "reward":
					break;
			}
		}
	}
}
#endif