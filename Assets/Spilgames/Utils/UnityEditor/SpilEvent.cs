﻿using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;
using SpilGames.Unity;
using SpilGames.Unity.Implementations;
using SpilGames.Unity.Utils;
using SpilGames.Unity.Helpers;
using System.Collections;

namespace SpilGames.Unity.Utils.UnityEditor
{
	public class SpilEvent : MonoBehaviour
	{
		public string eventName;
		public JSONObject data = new JSONObject ();
		public JSONObject customData = new JSONObject ();
		private int ts;
//		private string debug = "&debugMode=true";
		private string uid = Spil.SpilUserIdEditor;

		public void Send() {
			this.ts = (int)(Time.time * 1000);

			this.StartCoroutine (SendCoroutine ());
		}

		public IEnumerator SendCoroutine() {
			AddDefaultParameters();

			WWWForm requestForm = new WWWForm ();
			requestForm.AddField ("name", this.eventName);
			requestForm.AddField ("data", this.data.ToString ());
			if (!this.customData.IsNull) {
				requestForm.AddField ("customData", this.customData.ToString ());
			} else {
				requestForm.AddField ("customData", "{}");
			}


			requestForm.AddField ("ts", "1470057439857");
			requestForm.AddField ("queued", 0);
			requestForm.AddField ("debugMode", "true");

			WWW request = new WWW ("https://apptracker.spilgames.com/v1/native-events/event/android/" + Spil.BundleIdEditor + "/" + this.eventName, requestForm);
			yield return request;
//			while (!request.isDone);	
			if (request.error != null) {
				Debug.LogError ("Error getting data: " + request.error); 
				Debug.LogError ("Error getting data: " + request.text);
				ResponseEvent.Build(new JSONObject(request.text));
			} else { 
				JSONObject serverResponse = new JSONObject (request.text);
				Debug.LogError ("Data returned: " + serverResponse.ToString());
			}
		}

		private void AddDefaultParameters() {
			this.data.AddField("uid", this.uid);
			this.data.AddField("locale", "en");
			this.data.AddField("appVersion", "1.0");
			this.data.AddField("apiVersion", SpilUnityImplementationBase.PluginVersion);
			this.data.AddField("osVersion", "1.0");
			this.data.AddField("os", "android");
			this.data.AddField("osVersion", "6.0");
			this.data.AddField("deviceModel", "Editor");
			this.data.AddField("timezoneOffset", "0");
			this.data.AddField("tto", "200");
			this.data.AddField("packageName", Spil.BundleIdEditor);
			this.data.AddField ("sessionId", "deadbeef");
		}
	}
}