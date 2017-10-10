#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;
using SpilGames.Unity;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Helpers;
using System.Collections;
using System;
using SpilGames.Unity.Json;
using SpilGames.Unity.Base.UnityEditor.Responses;

namespace SpilGames.Unity.Base.UnityEditor {
    public class SpilEvent : MonoBehaviour {
        public string eventName;
        public JSONObject data = new JSONObject();
        public JSONObject customData = new JSONObject();
        private string uid = Spil.SpilUserIdEditor;

        private string bundleIdentifier;

        private string platform;

        public void Send() {
            this.StartCoroutine(SendCoroutine());
        }

        public IEnumerator SendCoroutine() {
            Spil spil = GameObject.FindObjectOfType<Spil>();
            platform = EditorUserBuildSettings.activeBuildTarget.ToString().Trim().ToLower();

#if UNITY_5_6_OR_NEWER
            bundleIdentifier = PlayerSettings.applicationIdentifier;
#elif UNITY_5_3_OR_NEWER
			bundleIdentifier = PlayerSettings.bundleIdentifier;
			#endif

            AddDefaultParameters();

            WWWForm requestForm = new WWWForm();
            requestForm.AddField("name", eventName);
            requestForm.AddField("data", data.ToString());
            requestForm.AddField("customData", !customData.IsNull ? customData.Print() : "{}");


            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var time = (long) (DateTime.Now.ToUniversalTime() - epoch).TotalMilliseconds;

            requestForm.AddField("ts", time.ToString());
            requestForm.AddField("queued", 0);

            if (spil.EditorDebugMode) {
                requestForm.AddField("debugMode", Convert.ToString(spil.EditorDebugMode).ToLower());
            }

            if (SpilUnityEditorImplementation.spilToken != null) {
                requestForm.AddField("spilToken", SpilUnityEditorImplementation.spilToken);
            }
            
            WWW request =
                new WWW(
                    "https://apptracker.spilgames.com/v1/native-events/event/" + platform + "/" + Spil.BundleIdEditor +
                    "/" + eventName, requestForm);
            yield return request;

            SpilLogging.Log("Sending event: " + "Name: " + eventName + " \nData: " + data + " \nCustom Data: " +
                            customData.Print() + " \nTimestamp: " + time);

            if (request.error != null && !request.error.Equals("")) {
                if (Spil.BundleIdEditor == null || Spil.BundleIdEditor.Equals("")) {
                    SpilLogging.Error(
                        "Spil Initialize might not have been called! Please make sure you call Spil.Initialize() at app start!");
                } else if (request.responseHeaders.Count > 0) {
                    foreach (KeyValuePair<string, string> entry in request.responseHeaders) {
                        if (entry.Key.Equals("STATUS") && entry.Value.Contains("401")) {
                            SocialLoginResponse.ProcessUnauthorizedResponse(request.text);
                        }
                    }
                } else {
                    SpilLogging.Error("Error getting data: " + request.error);
                    SpilLogging.Error("Error getting data: " + request.text);
                }
            } else {
                JSONObject serverResponse = new JSONObject(request.text);
                SpilLogging.Log("Data returned: \n" + serverResponse);
                ResponseEvent.Build(serverResponse);
            }
            GameObject.Destroy(this);
        }

        private void AddDefaultParameters() {
            data.AddField("uid", uid);
            data.AddField("locale", "en");
            data.AddField("appVersion", PlayerSettings.bundleVersion);
            data.AddField("apiVersion", SpilUnityImplementationBase.PluginVersion);
            data.AddField("osVersion", "1.0");
            data.AddField("os", platform);
            data.AddField("deviceModel", "Editor");
            data.AddField("timezoneOffset", "0");
            data.AddField("tto", "200");
            if (platform.Equals("android")) {
                data.AddField("packageName", bundleIdentifier);
            } else {
                data.AddField("bundleId", bundleIdentifier);
            }
            data.AddField("sessionId", "deadbeef");
            data.AddField("pluginName", Response.pluginName);
            data.AddField("pluginVersion", Response.pluginVersion);

            if (Response.provider == null || Response.externalId == null) return;
            JSONObject externalUserIdJson = new JSONObject();
            externalUserIdJson.AddField("provider", Response.provider);
            externalUserIdJson.AddField("userId", Response.externalId);

            data.AddField("externalUserId", externalUserIdJson);
        }
    }

    public class SpilLogging {
        public static void Log(string message) {
            Spil spil = GameObject.FindObjectOfType<Spil>();
            if (spil != null && spil.EditorLogging) {
                Debug.Log(message);
            }
        }

        public static void Error(string message) {
            Spil spil = GameObject.FindObjectOfType<Spil>();
            if (spil != null && spil.EditorLogging) {
                Debug.LogError(message);
            }
        }
    }
}
#endif