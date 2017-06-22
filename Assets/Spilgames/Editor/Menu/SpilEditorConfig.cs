﻿using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using SpilGames.Unity;
using System.Xml;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Json;

public class SpilEditorConfig : EditorWindow {
    private int tabSelected = 0;
    private static Spil spil;
    private Vector2 scrollPos;

    private static JSONObject configJSON;
    private static JSONObject android;
    private static JSONObject ios;

    private static string androidGameVersion = "";
    private static string iosGameVersion = "";

    private static string bundleIdentifier;

    [MenuItem("Spil SDK/Configuration", false, 0)]
    static void Init() {
#if UNITY_5_6_OR_NEWER
        bundleIdentifier = PlayerSettings.applicationIdentifier;
#elif UNITY_5_3_OR_NEWER
		bundleIdentifier = PlayerSettings.bundleIdentifier;
		#endif

        spil = GameObject.FindObjectOfType<Spil>();
        SpilEditorConfig window = (SpilEditorConfig) EditorWindow.GetWindow(typeof(SpilEditorConfig));
        window.autoRepaintOnSceneChange = true;
        window.titleContent.text = "Configuration";
        window.Show();

        androidGameVersion = PlayerSettings.bundleVersion;
        iosGameVersion = PlayerSettings.bundleVersion;
    }

    void OnGUI() {
        GUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Toggle(tabSelected == 0, "General", EditorStyles.toolbarButton)) {
                    tabSelected = 0;
                }
                if (GUILayout.Toggle(tabSelected == 1, "iOS", EditorStyles.toolbarButton)) {
                    tabSelected = 1;
                }
                if (GUILayout.Toggle(tabSelected == 2, "Android", EditorStyles.toolbarButton)) {
                    tabSelected = 2;
                }
                if (GUILayout.Toggle(tabSelected == 3, "Editor", EditorStyles.toolbarButton)) {
                    tabSelected = 3;
                }
            }
            GUILayout.EndHorizontal();

            switch (tabSelected) {
                case 0:
                    DrawGeneral();
                    break;
                case 1:
                    DrawIOS();
                    break;
                case 2:
                    DrawAndroid();
                    break;
                case 3:
                    DrawEditor();
                    break;
            }
        }

        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void DrawGeneral() {
        GUILayout.Label("");
        GUILayout.Label("WARNING: Please make sure to set your bundle ID in your player settings before getting your game data.\nPlease also note that this tool will only work if your build target is set to iOS or Android", EditorStyles.wordWrappedLabel);
        GUILayout.Label("");

        GUILayout.Label("Android Game Version", EditorStyles.boldLabel);
        androidGameVersion = GUILayout.TextField(androidGameVersion);

        GUILayout.Label("iOS Game Version", EditorStyles.boldLabel);
        iosGameVersion = GUILayout.TextField(iosGameVersion);

        GUILayout.Label("");
        GUILayout.Label("The button below will create the following configuration files:", EditorStyles.wordWrappedLabel);
        GUILayout.Label(" • defaultGameConfig.json", EditorStyles.boldLabel);
        GUILayout.Label(" • defaultPlayerData.json", EditorStyles.boldLabel);
        GUILayout.Label(" • defaultGameData.json", EditorStyles.boldLabel);
        GUILayout.Label("These files can be located in the StreamingAssets folder", EditorStyles.wordWrappedLabel);

        GUILayout.Label("");
        if (GUILayout.Button("Create Default Configuration Files")) {
            CreateDefaultConfigFiles();
        }


        if (File.Exists(Application.streamingAssetsPath + "/defaultGameConfig.json")) {
            GUILayout.Label("");
            GUILayout.Label("DefaultGameConfig Values:", EditorStyles.boldLabel);

            if (configJSON == null) {
                configJSON = new JSONObject(File.ReadAllText(Application.streamingAssetsPath + "/defaultGameConfig.json"));
            }

            GUILayout.Label("");
            if (configJSON.HasField("androidSdkConfig")) {
                if (android == null) {
                    android = new JSONObject(configJSON.GetField("androidSdkConfig").Print(false));
                }

                GUILayout.Label("Android", EditorStyles.boldLabel);
                string androidConfig = "";

                for (int i = 0; i < android.Count; i++) {
                    androidConfig = androidConfig + android.keys[i] + ": " + android.list[i].Print(false) + "\n";
                }
                GUILayout.Label(androidConfig, EditorStyles.wordWrappedLabel);
            }
            GUILayout.Label("");
            if (configJSON.HasField("iosSdkConfig")) {
                if (ios == null) {
                    ios = new JSONObject(configJSON.GetField("iosSdkConfig").Print(false));
                }

                GUILayout.Label("iOS", EditorStyles.boldLabel);
                string iosConfig = "";

                for (int i = 0; i < ios.Count; i++) {
                    iosConfig = iosConfig + ios.keys[i] + ": " + ios.list[i].Print(false) + "\n";
                }
                GUILayout.Label(iosConfig, EditorStyles.wordWrappedLabel);
            }
        }
    }

    private void DrawIOS() {
        GUILayout.Label("This tab contains configuration information specific to iOS", EditorStyles.boldLabel);
        GUILayout.Label("");

        string customUserId = "<< Spil component not found in scene! >>";
        bool exportDefaultEntitlements = EditorPrefs.GetBool("exportDefaultEntitlements");
        bool useICloudContainer = EditorPrefs.GetBool("useICloudContainer");
        bool useICloudKV = EditorPrefs.GetBool("useICloudKV");
        bool usePushNotifications = EditorPrefs.GetBool("usePushNotifications");


        if (spil != null) {
            customUserId = spil.CustomBundleId;
        }

        GUILayout.Label("Custom bundle id: (Useful when the bundle id used to build is\n" +
                        "not the same as the one connecting to the Spil Games backend)");
        customUserId = GUILayout.TextField(customUserId);

        GUILayout.Space(4);
        GUILayout.Label("Entitlements:", EditorStyles.boldLabel);
        exportDefaultEntitlements = GUILayout.Toggle(exportDefaultEntitlements, "Export Spil Games entitlements");
        useICloudContainer = GUILayout.Toggle(useICloudContainer, "Use iCloud project container");
        useICloudKV = GUILayout.Toggle(useICloudKV, "Use iCloud key-value store");
        usePushNotifications = GUILayout.Toggle(usePushNotifications, "Use push notifications");

        EditorPrefs.SetBool("exportDefaultEntitlements", exportDefaultEntitlements);
        EditorPrefs.SetBool("useICloudContainer", useICloudContainer);
        EditorPrefs.SetBool("useICloudKV", useICloudKV);
        EditorPrefs.SetBool("usePushNotifications", usePushNotifications);

        if (spil != null) {
            spil.CustomBundleId = customUserId;
        }
    }

    private void DrawAndroid() {
        GUILayout.Label("This tab contains configuration information specific to Android", EditorStyles.boldLabel);
        GUILayout.Label("");

        GUILayout.Label("Installed Modules:", EditorStyles.boldLabel);

        string androidFolder = "Assets/Plugins/Android/";
        string spilSDK = "spilsdk-" + SpilUnityImplementationBase.AndroidVersion + ".aar";
        string spilSDKAdjust = "spilsdk-adjust-" + SpilUnityImplementationBase.AndroidVersion + ".aar";
        string spilSDKChartboost = "spilsdk-chartboost-" + SpilUnityImplementationBase.AndroidVersion + ".aar";
        string spilSDKDFP = "spilsdk-dfp-" + SpilUnityImplementationBase.AndroidVersion + ".aar";
        string spilSDKFyber = "spilsdk-fyber-" + SpilUnityImplementationBase.AndroidVersion + ".aar";
        string spilSDKZendesk = "spilsdk-zendesk-" + SpilUnityImplementationBase.AndroidVersion + ".aar";

        var styleGreen = new GUIStyle(EditorStyles.label);
        Color green = new Color();
        ColorUtility.TryParseHtmlString("#006400", out green);
        styleGreen.normal.textColor = green;

        var styleRed = new GUIStyle(EditorStyles.label);
        styleRed.normal.textColor = Color.red;

        string spilSDKCheck = "";
        string spilSDKAdjustCheck = "";
        string spilSDKChartboostCheck = "";
        string spilSDKDFPCheck = "";
        string spilSDKFyberCheck = "";
        string spilSDKZendeskCheck = "";

        if (!File.Exists(androidFolder + spilSDK)) {
            spilSDKCheck = " - False";
            GUILayout.Label(" • SDK Main Module:" + spilSDKCheck, styleRed);
        }
        else {
            spilSDKCheck = " - True";
            GUILayout.Label(" • SDK Main Module:" + spilSDKCheck, styleGreen);
        }

        if (!File.Exists(androidFolder + spilSDKAdjust)) {
            spilSDKAdjustCheck = " - False";
            GUILayout.Label(" • Adjust (Analytics):" + spilSDKAdjustCheck, styleRed);
        }
        else {
            spilSDKAdjustCheck = " - True";
            GUILayout.Label(" • Adjust (Analytics):" + spilSDKAdjustCheck, styleGreen);
        }

        if (!File.Exists(androidFolder + spilSDKChartboost)) {
            spilSDKChartboostCheck = " - False";
            GUILayout.Label(" • Chartboost (Advertising):" + spilSDKChartboostCheck, styleRed);
        }
        else {
            spilSDKChartboostCheck = " - True";
            GUILayout.Label(" • Chartboost (Advertising):" + spilSDKChartboostCheck, styleGreen);
        }

        if (!File.Exists(androidFolder + spilSDKDFP)) {
            spilSDKDFPCheck = " - False";
            GUILayout.Label(" • DFP (Advertising):" + spilSDKDFPCheck, styleRed);
        }
        else {
            spilSDKDFPCheck = " - True";
            GUILayout.Label(" • DFP (Advertising):" + spilSDKDFPCheck, styleGreen);
        }

        if (!File.Exists(androidFolder + spilSDKFyber)) {
            spilSDKFyberCheck = " - False";
            GUILayout.Label(" • Fyber (Advertising):" + spilSDKFyberCheck, styleRed);
        }
        else {
            spilSDKFyberCheck = " - True";
            GUILayout.Label(" • Fyber (Advertising):" + spilSDKFyberCheck, styleGreen);
        }

        if (!File.Exists(androidFolder + spilSDKZendesk)) {
            spilSDKZendeskCheck = " - False";
            GUILayout.Label(" • Zendesk (Customer Support):" + spilSDKZendeskCheck, styleRed);
        }
        else {
            spilSDKZendeskCheck = " - True";
            GUILayout.Label(" • Zendesk (Customer Support):" + spilSDKZendeskCheck, styleGreen);
        }


        GUILayout.Label("");

        GUILayout.Label("Android Project Id:", EditorStyles.boldLabel);

        string projectId = "<< Spil component not found in scene! >>";
        if (spil != null) {
            projectId = spil.ProjectId;
        }

        projectId = GUILayout.TextField(projectId);

        if (spil != null) {
            spil.ProjectId = projectId;
        }

        GUILayout.Label("");

        GUILayout.Label("Permissions", EditorStyles.boldLabel);

        if (CheckAutomaticPermissionRequest()) {
            GUILayout.Label("Spil SDK Automatic Runtime Requesting of dangerous permissions - True", styleGreen);
        }
        else {
            GUILayout.Label("Spil SDK Automatic Runtime Requesting of dangerous permissions - False", styleRed);
        }

        GUILayout.Label("The label above tells if the Spil SDK will automatically request all the dangerous permissions at the start of your game. If you want to disable it please check your \"AndroidManifest.xml\" file.", EditorStyles.wordWrappedLabel);

        GUILayout.Label("");

        if (GUILayout.Button("Verify Android Setup")) {
            VerifyAndroidSetup();
        }
    }

    void DrawEditor() {
        GUILayout.Label("This tab contains configuration information specific to the Unity Editor", EditorStyles.boldLabel);
        GUILayout.Label("");

        bool enableLogging = false;
        if (spil != null) {
            enableLogging = spil.EditorLogging;
        }
        var styleRed = new GUIStyle(EditorStyles.label);
        styleRed.normal.textColor = Color.red;

        var styleGreen = new GUIStyle(EditorStyles.label);
        Color green = new Color();
        ColorUtility.TryParseHtmlString("#006400", out green);
        styleGreen.normal.textColor = green;

        if (enableLogging) {
            GUILayout.Label("Spil SDK Editor Logging is Enabled!", styleGreen);
        }
        else {
            GUILayout.Label("Spil SDK Editor Logging is Disabled! If you want to enable it toggle it from the Spil Game Object!", styleRed);
        }

        GUILayout.Label("");

        GUILayout.Label("Spil User Id:", EditorStyles.boldLabel);

        string editorUserId = "<< Spil component not found in scene! >>";
        if (spil != null) {
            editorUserId = spil.spilUserIdEditor;
        }

        editorUserId = GUILayout.TextField(editorUserId);

        if (spil != null) {
            spil.spilUserIdEditor = editorUserId;
        }

        GUILayout.Label("Bundle Id:", EditorStyles.boldLabel);

        string bundleIdEditor = "<< Spil component not found in scene! >>";
        if (spil != null) {
            bundleIdEditor = spil.bundleIdEditor;
        }

        bundleIdEditor = GUILayout.TextField(bundleIdEditor);

        if (spil != null) {
            spil.bundleIdEditor = bundleIdEditor;
        }

        GUILayout.Label("");
        if (GUILayout.Button("Detailed Editor Configuration")) {
            Selection.activeGameObject = spil.gameObject;
        }
    }

    void CreateDefaultConfigFiles() {
        if (bundleIdentifier == "") {
            throw new UnityException("Bundle ID is Blank");
        }
        Debug.Log("Getting Default Game Data for: " + bundleIdentifier);
        string streamingAssetsPath = Application.dataPath + "/StreamingAssets";
        if (!File.Exists(streamingAssetsPath)) {
            Directory.CreateDirectory(streamingAssetsPath);
        }
        if (!File.Exists(streamingAssetsPath + "/defaultGameData.json")) {
            File.WriteAllText(streamingAssetsPath + "/defaultGameData.json", GetData("requestGameData"));
        }
        else {
            File.Delete(streamingAssetsPath + "/defaultGameData.json");
            File.WriteAllText(streamingAssetsPath + "/defaultGameData.json", GetData("requestGameData"));
        }

        if (!File.Exists(streamingAssetsPath + "/defaultGameConfig.json")) {
            string configResponse = GetData("requestConfig");
            File.WriteAllText(streamingAssetsPath + "/defaultGameConfig.json", configResponse);
            configJSON = new JSONObject(configResponse);

            android = null;
            ios = null;
        }
        else {
            File.Delete(streamingAssetsPath + "/defaultGameConfig.json");
            string configResponse = GetData("requestConfig");
            File.WriteAllText(streamingAssetsPath + "/defaultGameConfig.json", configResponse);
            configJSON = new JSONObject(configResponse);

            android = null;
            ios = null;
        }

        if (!File.Exists(streamingAssetsPath + "/defaultPlayerData.json")) {
            JSONObject gameData = new JSONObject(GetData("requestGameData"));
            JSONObject playerData = new JSONObject(GetData("requestPlayerData"));

            for (int i = 0; i < gameData.GetField("currencies").Count; i++) {
                JSONObject currency = new JSONObject();
                currency.AddField("id", gameData.GetField("currencies").list[i].GetField("id"));
                currency.AddField("currentBalance", 0);
                currency.AddField("delta", 0);

                playerData.GetField("wallet").GetField("currencies").Add(currency);
            }

            playerData.GetField("wallet").RemoveField("offset");
            playerData.GetField("wallet").AddField("offset", 0);

            playerData.GetField("inventory").RemoveField("offset");
            playerData.GetField("inventory").AddField("offset", 0);

            File.WriteAllText(streamingAssetsPath + "/defaultPlayerData.json", playerData.Print(false));
        }
        else {
            File.Delete(streamingAssetsPath + "/defaultPlayerData.json");

            JSONObject gameData = new JSONObject(GetData("requestGameData"));
            JSONObject playerData = new JSONObject(GetData("requestPlayerData"));

            for (int i = 0; i < gameData.GetField("currencies").Count; i++) {
                JSONObject currency = new JSONObject();
                currency.AddField("id", gameData.GetField("currencies").list[i].GetField("id"));
                currency.AddField("currentBalance", 0);
                currency.AddField("delta", 0);

                playerData.GetField("wallet").GetField("currencies").Add(currency);
            }

            playerData.GetField("wallet").AddField("offset", 0);

            playerData.GetField("inventory").AddField("offset", 0);

            File.WriteAllText(streamingAssetsPath + "/defaultPlayerData.json", playerData.Print(false));
        }
    }

    string GetData(string type) {
        string gameData = "";

        if (type.Equals("requestConfig")) {
            JSONObject combined = null;

            WWWForm form = GetFormData("android");
            form.AddField("name", type);
            WWW request = new WWW("https://apptracker.spilgames.com/v1/native-events/event/android/" + bundleIdentifier + "/" + type, form);
            while (!request.isDone)
                ;
            if (request.error != null) {
                Debug.LogError("Error getting game data: " + request.error);
                combined.AddField("androidSdkConfig", "");
            }
            else {
                combined = new JSONObject(request.text).GetField("data");
            }

            WWWForm form2 = GetFormData("ios");
            form2.AddField("name", type);
            WWW request2 = new WWW("https://apptracker.spilgames.com/v1/native-events/event/ios/" + bundleIdentifier + "/" + type, form2);
            while (!request2.isDone)
                ;
            if (request2.error != null) {
                Debug.LogError("Error getting game data: " + request2.error);
                combined.AddField("iosSdkConfig", "");
            }
            else {
                if (combined.HasField("iosSdkConfig")) {
                    combined.RemoveField("iosSdkConfig");
                }
                combined.AddField("iosSdkConfig", new JSONObject(request2.text).GetField("data").GetField("iosSdkConfig"));
            }
            gameData = combined.Print(false);
        }
        else {
            WWWForm form = GetFormData(EditorUserBuildSettings.activeBuildTarget.ToString().Trim().ToLower());
            form.AddField("name", type);
            WWW request = new WWW("https://apptracker.spilgames.com/v1/native-events/event/" + EditorUserBuildSettings.activeBuildTarget.ToString().Trim().ToLower() + "/" + bundleIdentifier + "/" + type, form);
            while (!request.isDone)
                ;
            if (request.error != null) {
                Debug.LogError("Error getting game data: " + request.error + " " + request.text);
            }
            else {
                Debug.Log(type + " Data returned: " + request.text);
                JSONObject serverResponce = new JSONObject(request.text);
                gameData = serverResponce.GetField("data").ToString();
            }
        }


        return gameData;
    }

    WWWForm GetFormData(string platform) {
        JSONObject dummyData = new JSONObject();
        dummyData.AddField("uid", "deadbeef");
        dummyData.AddField("locale", "en");

        if (platform.Equals("android")) {
            dummyData.AddField("appVersion", androidGameVersion);
        }
        else {
            dummyData.AddField("appVersion", iosGameVersion);
        }

        dummyData.AddField("apiVersion", SpilUnityImplementationBase.PluginVersion);
        dummyData.AddField("os", platform);
        dummyData.AddField("osVersion", "1");
        dummyData.AddField("deviceModel", "Editor");

        if (platform.Equals("android")) {
            dummyData.AddField("packageName", bundleIdentifier);
        }
        else {
            dummyData.AddField("bundleId", bundleIdentifier);
        }

        dummyData.AddField("tto", "0");
        dummyData.AddField("sessionId", "deadbeef");
        dummyData.AddField("timezoneOffset", "0");

        JSONObject dummyCustomData = new JSONObject();
        JSONObject dummyWallet = new JSONObject();
        dummyWallet.AddField("offset", 0);
        JSONObject dummyInventory = new JSONObject();
        dummyInventory.AddField("offset", 0);
        dummyCustomData.AddField("wallet", dummyWallet);
        dummyCustomData.AddField("inventory", dummyInventory);

        WWWForm form = new WWWForm();
        form.AddField("data", dummyData.ToString());
        form.AddField("customData", dummyCustomData.ToString());
        form.AddField("ts", "1470057439857");
        form.AddField("queued", 0);
        if (spil.EditorDebugMode) {
            form.AddField("debugMode", Convert.ToString(spil.EditorDebugMode).ToLower());
        }
        return form;
    }

    void VerifyAndroidSetup() {
        bool isEverythingCorrect = true;

        string androidFolder = "Assets/Plugins/Android/";

        if (Directory.Exists(androidFolder + "GooglePlayServices")) {
            Debug.LogError("The contents of the GooglePlayServices folder should be copied into 'Assets/Plugins/Android/' and afterwards the folder should be removed");
            isEverythingCorrect = false;
        }

        string appManifestPath = androidFolder + "AndroidManifest.xml";

        // Let's open the app's AndroidManifest.xml file.
        XmlDocument manifestFile = new XmlDocument();
        manifestFile.Load(appManifestPath);

        XmlElement manifestRoot = manifestFile.DocumentElement;
        XmlNode applicationNode = null;

        foreach (XmlNode node in manifestRoot.ChildNodes) {
            if (node.Name == "application") {
                applicationNode = node;
                break;
            }
        }

        // If there's no applicatio node, something is really wrong with your AndroidManifest.xml.
        if (applicationNode == null) {
            Debug.LogError("Your app's AndroidManifest.xml file does not contain \"<application>\" node.");
            Debug.LogError("Unable to verify if the values were correct");

            return;
        }

        if (!(applicationNode.Attributes["android:name"].Value.Equals("com.spilgames.spilsdk.activities.SpilSDKApplication")) && !(applicationNode.Attributes["android:name"].Value.Equals("com.spilgames.spilsdk.activities.SpilSDKApplicationWithFabric"))) {
            Debug.LogError(
                "The application name from your \"AndroidManifest.xml\" file is set incorrectly. Please set it to either \"com.spilgames.spilsdk.activities.SpilSDKApplication\" or \"com.spilgames.spilsdk.activities.SpilSDKApplicationWithFabric\" (if you are using Crashlytics) if you want for the Spil SDK to function correctly");
            isEverythingCorrect = false;
        }

        bool isUnityRequestPermissionDisabled = false;

        foreach (XmlNode node in applicationNode.ChildNodes) {
            if (node.Name.Equals("activity")) {
                foreach (XmlNode subNode in node.ChildNodes) {
                    if (subNode.Name.Equals("intent-filter")) {
                        foreach (XmlNode bottomNode in subNode.ChildNodes) {
                            if (bottomNode.Name.Equals("action") && bottomNode.Attributes["android:name"].Value.Equals("android.intent.action.MAIN")) {
                                if (!(node.Attributes["android:name"].Value.Equals("com.spilgames.spilsdk.activities.SpilUnityActivity")) && !(node.Attributes["android:name"].Value.Equals("com.spilgames.spilsdk.activities.SpilUnityActivityWithPrime")) &&
                                    !(node.Attributes["android:name"].Value.Equals("com.spilgames.spilsdk.activities.SpilUnityActivityWithAN"))) {
                                    Debug.LogError(
                                        "The Main Activity name from your \"AndroidManifest.xml\" file is set incorrectly. Please set it to either \"com.spilgames.spilsdk.activities.SpilUnityActivity\", \"com.spilgames.spilsdk.activities.SpilUnityActivityWithPrime\" (if you are using Prime31 Plugin) or \"com.spilgames.spilsdk.activities.SpilUnityActivityWithAN\" (if you are using Android Native Plugin) if you want for the Spil SDK to function correctly");
                                    isEverythingCorrect = false;
                                }
                            }
                        }
                    }
                }
            }

            if (node.Name.Equals("meta-data")) {
                if (node.Attributes["android:name"].Value.Equals("unityplayer.SkipPermissionsDialog") && node.Attributes["android:value"].Value.Equals("true")) {
                    isUnityRequestPermissionDisabled = true;
                    isEverythingCorrect = false;
                }
            }
        }

        if (!isUnityRequestPermissionDisabled) {
            Debug.LogError(
                "You did not disable the automatic Unity permission request. Please add the following line to your \"applicaiton\" tag: \"<meta-data android:name=\"unityplayer.SkipPermissionsDialog\" android:value=\"true\" />\". This is required in order to not have any problems with the Spil SDK's permission system. Keep in mind that all your dangerous permissions will be handled by the Spil SDK automatically");
        }

        if (!isEverythingCorrect) {
            Debug.Log("Verification Complete! The Spil SDK for Android is configured correctly!");
        }
        else {
            Debug.LogError("Verification Complete! Something was not configured correctly!! Please check the logs");
        }
    }

    bool CheckAutomaticPermissionRequest() {
        string androidFolder = "Assets/Plugins/Android/";
        string appManifestPath = androidFolder + "AndroidManifest.xml";

        // Let's open the app's AndroidManifest.xml file.
        XmlDocument manifestFile = new XmlDocument();
        manifestFile.Load(appManifestPath);

        XmlElement manifestRoot = manifestFile.DocumentElement;
        XmlNode applicationNode = null;

        foreach (XmlNode node in manifestRoot.ChildNodes) {
            if (node.Name == "application") {
                applicationNode = node;
                break;
            }
        }

        // If there's no applicatio node, something is really wrong with your AndroidManifest.xml.
        if (applicationNode == null) {
            Debug.LogError("Your app's AndroidManifest.xml file does not contain \"<application>\" node.");
            Debug.LogError("Unable to verify if the values were correct");

            return true;
        }

        foreach (XmlNode node in applicationNode.ChildNodes) {
            if (node.Name.Equals("meta-data")) {
                if (node.Attributes["android:name"].Value.Equals("spil.permissions.DisableAutoRequest") && node.Attributes["android:value"].Value.Equals("true")) {
                    return false;
                }
            }
        }

        return true;
    }
}