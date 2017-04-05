#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;
using SpilGames.Unity;
using SpilGames.Unity.Implementations;
using SpilGames.Unity.Utils;
using SpilGames.Unity.Helpers;
using System.Collections;
using SpilGames.Unity.Utils.UnityEditor.Responses;
using System;

namespace SpilGames.Unity.Utils.UnityEditor
{
	public class SpilEvent : MonoBehaviour
	{
		public string eventName;
		public JSONObject data = new JSONObject ();
		public JSONObject customData = new JSONObject ();
		private string uid = Spil.SpilUserIdEditor;

		private string platform;

		#if UNITY_5_6_OR_NEWER
		private string bundleIdentifier = PlayerSettings.applicationIdentifier;
		#elif UNITY_5_3_OR_NEWER
		private string bundleIdentifier = PlayerSettings.bundleIdentifier;
		#endif

		public void Send() {
			this.StartCoroutine (SendCoroutine ());
		}

		public IEnumerator SendCoroutine() {

			platform = EditorUserBuildSettings.activeBuildTarget.ToString().Trim().ToLower();

			AddDefaultParameters();

			WWWForm requestForm = new WWWForm ();
			requestForm.AddField ("name", this.eventName);
			requestForm.AddField ("data", this.data.ToString ());
			if (!this.customData.IsNull) {
				requestForm.AddField ("customData", this.customData.Print(false));
			} else {
				requestForm.AddField ("customData", "{}");
			}


			var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
             		var time = (long)(DateTime.Now.ToUniversalTime() - epoch).TotalMilliseconds;

             		requestForm.AddField ("ts", time.ToString());
			requestForm.AddField ("queued", 0);
			requestForm.AddField ("debugMode", "true");

			WWW request = new WWW ("https://apptracker.spilgames.com/v1/native-events/event/" + platform + "/" + Spil.BundleIdEditor + "/" + this.eventName, requestForm);
			yield return request;
//			while (!request.isDone);	
			if (request.error != null) {
				Debug.LogError ("Error getting data: " + request.error); 
				Debug.LogError ("Error getting data: " + request.text);
			} else { 
				JSONObject serverResponse = new JSONObject (request.text);
				Debug.Log ("Data returned: " + serverResponse.ToString());
				ResponseEvent.Build(serverResponse);
			}
			GameObject.Destroy(this);
		}

		private void AddDefaultParameters() {
			this.data.AddField("uid", this.uid);
			this.data.AddField("locale", "en");
			this.data.AddField("appVersion", PlayerSettings.bundleVersion);
			this.data.AddField("apiVersion", SpilUnityImplementationBase.PluginVersion);
			this.data.AddField("osVersion", "1.0");
			this.data.AddField("os", platform);
			this.data.AddField("deviceModel", "Editor");
			this.data.AddField("timezoneOffset", "0");
			this.data.AddField("tto", "200");
			if(platform.Equals("android")){
				this.data.AddField ("packageName", bundleIdentifier);
			} else {
				this.data.AddField ("bundleId", bundleIdentifier);
			}
			this.data.AddField ("sessionId", "deadbeef");
			this.data.AddField("pluginName", Response.pluginName);
			this.data.AddField("pluginVersion", Response.pluginVersion);

			if(Response.provider != null && Response.externalId != null){
				JSONObject externalUserIdJson = new JSONObject();
				externalUserIdJson.AddField("provider", Response.provider);
				externalUserIdJson.AddField("userId", Response.externalId);

				this.data.AddField("externalUserId", externalUserIdJson);
			}
		}
	}
}
#endif