using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;
using SpilGames.Unity;
using SpilGames.Unity.Implementations;
using SpilGames.Unity.Utils;
using SpilGames.Unity.Helpers;

namespace SpilGames.Unity.Utils.UnityEditor
{

	public class SpilEvent : MonoBehaviour
	{

		public string eventName;
		private JSONObject data;
		public JSONObject customData;
		private string queued = "0";
		private long ts = (Time.time * 1000).ToString();
		private string debug = "&debugMode=true";
		private string uid = Spil.SpilUserIdEditor;

		public static void TrackEvent(SpilEvent eventToTrack){
			WWWForm requestForm = new WWWForm ();

			requestForm.AddField ("name", eventToTrack.eventName);
			requestForm.AddField ("data", data.ToString ());
			requestForm.AddField ("customData", customData.ToString ());
			requestForm.AddField ("ts", eventToTrack.ts);
			requestForm.AddField ("queued", 0);

			WWW request = new WWW ("https://apptracker.spilgames.com/v1/native-events/event/android/" + Spil.BundleIdEditor + "/" + eventToTrack.eventName, requestForm);
			while (!request.isDone)
				;
			if (request.error != null) {
				Debug.LogError ("Error getting data: " + request.error);  
			} else { 
				JSONObject serverResponse = new JSONObject (request.text);
				Debug.Log ("Data returned: " + serverResponse.ToString());
			}
		}

		private Event AddDefaultParameters(SpilEvent eventToTrack){
			eventToTrack.data.AddField("uid", uid);
			eventToTrack.data.AddField("locale", "en");
			eventToTrack.data.AddField("appVersion", "1.0");
			eventToTrack.data.AddField("apiVersion", SpilUnityImplementationBase.PluginVersion);
			eventToTrack.data.AddField("osVersion", "1.0");
			eventToTrack.data.AddField("os", "android");
			eventToTrack.data.AddField("osVersion", "6.0");
			eventToTrack.data.AddField("deviceModel", "Editor");
			eventToTrack.data.AddField("timezoneOffset", "0");
			eventToTrack.data.AddField("packageName", Spil.BundleIdEditor);
			eventToTrack.data.AddField ("sessionId", "deadbeef");
		}

	}
}


