using System;
using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor;
using System.IO;
using SpilGames.Unity.Base.Implementations;
using System.Xml;
using UnityEngine.Analytics;


public class SpilAndroidBuildPostProcess : MonoBehaviour {
    private static string androidFolder = "Assets/Plugins/Android/";

#if UNITY_5_6_OR_NEWER
    private static string bundleIdentifier = PlayerSettings.applicationIdentifier;
#elif UNITY_5_3_OR_NEWER
	private static string bundleIdentifier = PlayerSettings.bundleIdentifier;
	#endif

    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuildProject) {
        if (target == BuildTarget.Android) {
            Debug.Log("Starting verification step for Android Spil SDK");

            //Check if Google Play Services are copied correctly
            VerifyGoglePlayServices();

            //Check if Spil SDK aars are present
            VerifySpilSDKFiles();

            //Check if AndroidMaifest.xml has the correct values
            VerifyManifest();
            
            //Check if the Spil SDK is up-to-date
            CheckLatestPluginVersion();
            
            Analytics.enabled = false;
            Analytics.deviceStatsEnabled = false;
            
            Debug.Log("Verification step for Android Spil SDK completed");
        }
    }

    public static void VerifyGoglePlayServices() {
        if (Directory.Exists(androidFolder + "GooglePlayServices")) {
            Debug.LogError("The contents of the GooglePlayServices folder should be copied into 'Assets/Plugins/Android/' and afterwards the folder should be removed");
        }
    }

    public static void VerifySpilSDKFiles() {
        string spilSDK = "spilsdk-" + SpilUnityImplementationBase.AndroidVersion + ".aar";
        string spilSDKAdjust = "spilsdk-adjust-" + SpilUnityImplementationBase.AndroidVersion + ".aar";
        string spilSDKChartboost = "spilsdk-chartboost-" + SpilUnityImplementationBase.AndroidVersion + ".aar";
        string spilSDKAdMob = "spilsdk-admob-" + SpilUnityImplementationBase.AndroidVersion + ".aar";
        string spilSDKFirebase = "spilsdk-firebase-" + SpilUnityImplementationBase.AndroidVersion + ".aar";

        String[] gradleLines = File.ReadAllLines(androidFolder + "mainTemplate.gradle");
        bool gradleSpilSDKCheck = false;
        bool gradleAdjustSpilSDKCheck = false;
        bool gradleChartboostSpilSDKCheck = false;
        bool gradleAdMobSpilSDKCheck = false;
        bool gradleFirebaseSpilSDKCheck = false;
        
        foreach (string line in gradleLines) {
            if (line.Contains("spilsdk:" + SpilUnityImplementationBase.AndroidVersion) && !line.Contains("//")) {
                gradleSpilSDKCheck = true;
            }
            
            if (line.Contains("spilsdk-adjust:" + SpilUnityImplementationBase.AndroidVersion) && !line.Contains("//")) {
                gradleAdjustSpilSDKCheck = true;
            }
            
            if (line.Contains("spilsdk-chartboost:" + SpilUnityImplementationBase.AndroidVersion) && !line.Contains("//")) {
                gradleChartboostSpilSDKCheck = true;
            }
            
            if (line.Contains("spilsdk-admob:" + SpilUnityImplementationBase.AndroidVersion) && !line.Contains("//")) {
                gradleAdMobSpilSDKCheck = true;
            }
            
            if (line.Contains("spilsdk-firebase:" + SpilUnityImplementationBase.AndroidVersion) && !line.Contains("//")) {
                gradleFirebaseSpilSDKCheck = true;
            }
        }
        
        if (!File.Exists(androidFolder + spilSDK) && !gradleSpilSDKCheck) {
            Debug.LogError("The Spil SDK aar file is missing from your 'Assets/Plugins/Android/' or the gradle file is missing the declaration. If you want to use the Spil SDK please make sure to include the file to that location or include the mainTemplate.gradle file included with the Spil SDK package.");
        }

        if (!File.Exists(androidFolder + spilSDKAdjust) && !gradleAdjustSpilSDKCheck) {
            Debug.Log("The Spil SDK Adjust aar file is missing from your 'Assets/Plugins/Android/' or the gradle file is missing the declaration. If you want to use the Spil SDK Adjust please make sure to included the file to that location or enable the feature from the configuration menu.");
        }

        if (!File.Exists(androidFolder + spilSDKChartboost) && !gradleChartboostSpilSDKCheck) {
            Debug.Log("The Spil SDK Chartboost aar file is missing from your 'Assets/Plugins/Android/' or the gradle file is missing the declaration. If you want to use the Spil SDK Chartboost please make sure to included to that location or enable the feature from the configuration menu.");
        }

        if (!File.Exists(androidFolder + spilSDKAdMob) && !gradleAdMobSpilSDKCheck) {
            Debug.Log("The Spil SDK AdMob aar file is missing from your 'Assets/Plugins/Android/' or the gradle file is missing the declaration. If you want to use the Spil SDK AdMob please make sure to included the file to that location or enable the feature from the configuration menu.");
        }
        
        if (!File.Exists(androidFolder + spilSDKFirebase) && !gradleFirebaseSpilSDKCheck) {
            Debug.Log("The Spil SDK Firebase aar file is missing from your 'Assets/Plugins/Android/' or the gradle file is missing the declaration. If you want to use the Spil SDK Firebase please make sure to included the file to that location or enable the feature from the configuration menu.");
        }
        
        CheckSlotGameConifg();
        CheckSlotGameData();
    }

    public static void VerifyManifest() {
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

        if (!(applicationNode.Attributes["android:name"].Value.Equals("com.spilgames.spilsdk.activities.SpilSDKApplication"))) {
            Debug.LogError(
                "The application name from your \"AndroidManifest.xml\" file is set incorrectly. Please set it to either \"com.spilgames.spilsdk.activities.SpilSDKApplication\" if you want for the Spil SDK to function correctly");
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
                                }
                            }
                        }
                    }
                }
            }
            if (node.Name.Equals("meta-data")) {
                if (node.Attributes["android:name"].Value.Equals("unityplayer.SkipPermissionsDialog") && node.Attributes["android:value"].Value.Equals("true")) {
                    isUnityRequestPermissionDisabled = true;
                }
            }
        }

        if (!isUnityRequestPermissionDisabled) {
            Debug.LogError(
                "You did not disable the automatic Unity permission request. Please add the following line to your \"applicaiton\" tag: \"<meta-data android:name=\"unityplayer.SkipPermissionsDialog\" android:value=\"true\" />\". This is required in order to not have any problems with the Spil SDK's permission system. Keep in mind that all your dangerous permissions will be handled by the Spil SDK automatically");
        }
    }
    
    public static void CheckSlotGameConifg() {
        JSONObject slotConfigJSON = new JSONObject(GetData("requestConfig"));
        JSONObject localConfigJSON = new JSONObject(System.IO.File.ReadAllText(Application.streamingAssetsPath + "/defaultGameConfig.json"));
        if (slotConfigJSON.HasField("androidSdkConfig") && localConfigJSON.HasField("androidSdkConfig")) {
            string slotConfig = slotConfigJSON.GetField("androidSdkConfig").Print(false);
            string localConfig = localConfigJSON.GetField("androidSdkConfig").Print(false);

            if (!localConfig.Equals(slotConfig)) {
                UnityEngine.Debug.Log("The local \"defaultGameConfig.json\" file is out-of-sync with the SLOT configuration. Please make sure to update your file with the latest changes from SLOT before building!");
                Debug.Log("The check done at this stage only verifies if you have data correctly for the Live environment and not the Debug/Testing one!");
            } else {
                Debug.Log("Local Game Config is synchronised with SLOT");
            }
        }
    }

    public static void CheckSlotGameData() {
        JSONObject slotGameDataJSON = new JSONObject(GetData("requestGameData"));
        JSONObject localGameDataJSON = new JSONObject(System.IO.File.ReadAllText(Application.streamingAssetsPath + "/defaultGameData.json"));
        if (!localGameDataJSON.Print().Equals(slotGameDataJSON.Print())) {
            UnityEngine.Debug.Log("The local \"defaultGameData.json\" file is out-of-sync with the SLOT configuration. Please make sure to update your file with the latest changes from SLOT before building!");
            Debug.Log("The check done at this stage only verifies if you have data correctly for the Live environment and not the Debug/Testing one!");
        }
        else {
            Debug.Log("Local Game Data is synchronised with SLOT");
        }
    }

    public static string GetData(string type) {
        string data = "";
        WWWForm form = GetFormData();
        form.AddField("name", type);
        WWW request = new WWW("https://apptracker.spilgames.com/v1/native-events/event/android/" + bundleIdentifier + "/" + type, form);
        while (!request.isDone)
            ;
        if (request.error != null && !request.error.Equals("")) {
            SpilLogging.Error("Error getting game data: " + request.error);
        } else {
            JSONObject serverResponse = new JSONObject(request.text);
            data = serverResponse.GetField("data").ToString();
        }
        return data;
    }

    static WWWForm GetFormData() {
        JSONObject dummyData = new JSONObject();
        dummyData.AddField("uid", "deadbeef");
        dummyData.AddField("locale", "en");
        dummyData.AddField("appVersion", PlayerSettings.bundleVersion);
        dummyData.AddField("apiVersion", SpilUnityImplementationBase.PluginVersion);
        dummyData.AddField("os", "Android");
        dummyData.AddField("osVersion", "1");
        dummyData.AddField("deviceModel", "Editor");
        dummyData.AddField("packageName", bundleIdentifier);
        dummyData.AddField("tto", "0");
        dummyData.AddField("sessionId", "deadbeef");
        dummyData.AddField("timezoneOffset", "0");
        WWWForm form = new WWWForm();
        form.AddField("data", dummyData.ToString());
        form.AddField("ts", "1470057439857");
        form.AddField("queued", 0);
        return form;
    }

    public static void CheckLatestPluginVersion() {
        WWW request = new WWW("https://api.github.com/repos/spilgames/spil_event_unity_plugin/releases/latest");

        while (!request.isDone) ;

        if (request.error == null || request.error.Equals("")) {
            JSONObject response = new JSONObject(request.text);

            string GitHubReleaseTag = response.GetField("tag_name").Print(false);

            if (!GitHubReleaseTag.Contains(SpilUnityImplementationBase.PluginVersion)) {
                Debug.Log("A new version of the Spil SDK is available! You can download the new version here: https://github.com/spilgames/spil_event_unity_plugin/releases ");
            }
        }
    }
}