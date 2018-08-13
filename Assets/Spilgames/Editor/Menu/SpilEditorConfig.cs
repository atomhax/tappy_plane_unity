using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SpilGames.Unity;
using System.Xml;
using System.Xml.Serialization;
using JetBrains.Annotations;
using SpilGames.Unity.Base.Implementations;
using SpilGames.Unity.Base.UnityEditor;
using SpilGames.Unity.Json;
using UnityEngine.WSA;
using Application = UnityEngine.Application;

public class SpilEditorConfig : EditorWindow {
    private int tabSelected = 0;
    private static Spil spil;
    private Vector2 scrollPos;

    private Texture2D logo = null;
    private static GUIStyle centeredStyle;
    
    private static JSONObject configJSON;
    private static JSONObject android;
    private static JSONObject ios;

    private static string androidGameVersion = "";
    private static string iosGameVersion = "";

    public static string androidPackageName;

    int androidImplementationSelection = 0;
    static string[] options = new string[] {" Gradle", " AAR Files"};
    
    public static string GetAndroidPackageName() {
        #if UNITY_5_6_OR_NEWER
        return PlayerSettings.applicationIdentifier;
        #elif UNITY_5_3_OR_NEWER
		    return PlayerSettings.bundleIdentifier;
#endif
    }

    public static string iosBundelId;

    public static string GetIOSBundleId() {
        #if UNITY_5_6_OR_NEWER
        return PlayerSettings.applicationIdentifier;
        #elif UNITY_5_3_OR_NEWER
		        return PlayerSettings.bundleIdentifier;
#endif
    }

    public static bool retrievalError;

    [MenuItem("Spil SDK/Configuration", false, 1)]
    static void Init() {
        spil = GameObject.FindObjectOfType<Spil>();
        SpilEditorConfig window = (SpilEditorConfig) EditorWindow.GetWindow(typeof(SpilEditorConfig));
        window.autoRepaintOnSceneChange = true;
        window.titleContent.text = "Configuration";
        window.minSize = new Vector2(1000, 700);
        window.Show();

        androidGameVersion = PlayerSettings.bundleVersion;
        androidPackageName = GetAndroidPackageName();
        iosGameVersion = PlayerSettings.bundleVersion;
        iosBundelId = GetIOSBundleId();
        
    }

    void OnEnable() {
        Vector2 size = new Vector2(position.width, 256);

        logo = new Texture2D((int) size.x, (int) size.y, TextureFormat.RGB24, false);
        logo.LoadImage(File.ReadAllBytes(Application.dataPath + "/Resources/Spilgames/PrivacyPolicy/Images/spillogo.png"));

        androidImplementationSelection = EditorPrefs.GetInt("spilSdkAndroidImplementation", 0);
    }

    private void OnDisable() {
        EditorPrefs.SetInt("spilSdkAndroidImplementation", androidImplementationSelection);
    }

    void OnGUI() {
        GUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));
        
        if (centeredStyle == null) {
            centeredStyle = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, wordWrap = true, fontStyle = FontStyle.Bold};
        }
        
        GUILayout.Label("");
        GUILayout.Label(logo, centeredStyle);
        GUILayout.Label("");
        
        GUILayout.BeginHorizontal();
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

        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void DrawGeneral() {
        GUILayout.Label("");
        GUILayout.Label(
            "WARNING: Please make sure to set your bundle ID in your player settings before getting your game data.\nPlease also note that this tool will only work if your build target is set to iOS or Android",
            EditorStyles.wordWrappedLabel);
        GUILayout.Label("");

        GUILayout.Label("Android Game Version", EditorStyles.boldLabel);
        androidGameVersion = GUILayout.TextField(androidGameVersion);

        GUILayout.Label("Android Package Name", EditorStyles.boldLabel);
        androidPackageName = GUILayout.TextField(androidPackageName);

        GUILayout.Label("iOS Game Version", EditorStyles.boldLabel);
        iosGameVersion = GUILayout.TextField(iosGameVersion);

        GUILayout.Label("iOS Bundle Id", EditorStyles.boldLabel);
        iosBundelId = GUILayout.TextField(iosBundelId);

        GUILayout.Label("");
        GUILayout.Label("The button below will create the following configuration files:",
            EditorStyles.wordWrappedLabel);
        GUILayout.Label(" • defaultGameConfig.json", EditorStyles.boldLabel);
        GUILayout.Label(" • defaultPlayerData.json", EditorStyles.boldLabel);
        GUILayout.Label(" • defaultGameData.json", EditorStyles.boldLabel);
        GUILayout.Label("These files can be located in the StreamingAssets folder", EditorStyles.wordWrappedLabel);

        GUILayout.Label("");
        if (GUILayout.Button("Create Default Configuration Files")) {
            CreateDefaultConfigFiles();
        }
    }
    
    private void DrawIOS() {
        string iosFolder = "Assets/Plugins/iOS/";
        
        GUILayout.Label("This tab contains configuration information specific to iOS", EditorStyles.boldLabel);
        GUILayout.Label("");

        var styleRed = new GUIStyle(EditorStyles.label);
        styleRed.normal.textColor = Color.red;
        styleRed.fontStyle = FontStyle.Bold;
        
        string[] iosVersionFile = File.ReadAllLines(iosFolder + "Spil.framework/Headers/Spil.h");
        
        foreach (string line in iosVersionFile) {
            
            if (line.Contains("#define SPIL_SDK_VERSION @") && !line.Contains(SpilUnityImplementationBase.iOSVersion)) {
                string version = "";
                foreach (Match m in Regex.Matches(line, "\".*?\"")) {
                    version = m.Value.Replace("\"", "");
                }
                GUILayout.Label("Spil iOS Framework version does not match Spil Unity Plugin.", styleRed);
                GUILayout.Label("Spil iOS Framework version: " + version, styleRed);
                GUILayout.Label("Spil Unity Plugin version: " + SpilUnityImplementationBase.iOSVersion, styleRed);
                GUILayout.Label("");
            }
        }

        string customUserId = "<< Spil component not found in scene! >>";
        bool exportDefaultEntitlements = EditorPrefs.GetBool("exportDefaultEntitlements");
        bool useICloudKV = EditorPrefs.GetBool("useICloudKV");

        if (spil != null) {
            customUserId = spil.CustomBundleId;
        }

        GUILayout.Label("Custom bundle id: (Useful when the bundle id used to build is\n" +
                        "not the same as the one connecting to the Spil Games backend)");
        customUserId = GUILayout.TextField(customUserId);

        GUILayout.Space(4);
        GUILayout.Label("Entitlements:", EditorStyles.boldLabel);
        exportDefaultEntitlements = GUILayout.Toggle(exportDefaultEntitlements, "Export Spil Games entitlements");
        useICloudKV = GUILayout.Toggle(useICloudKV, "Use iCloud key-value store");

        EditorPrefs.SetBool("exportDefaultEntitlements", exportDefaultEntitlements);
        EditorPrefs.SetBool("useICloudKV", useICloudKV);

        if (spil != null) {
            spil.CustomBundleId = customUserId;
        }
    }

    private void DrawAndroid() {
        string androidFolder = "Assets/Plugins/Android/";
        string spilFolder = "Assets/Spilgames/Plugins/NativeLibraries/Android/";
        string spilAndroidInstalationFolder = "Assets/Spilgames/Plugins/Android/";
        string spilSDK = "spilsdk-" + SpilUnityImplementationBase.AndroidVersion + ".aar";
        string spilSDKAdjust = "spilsdk-adjust-" + SpilUnityImplementationBase.AndroidVersion + ".aar";
        string spilSDKChartboost = "spilsdk-chartboost-" + SpilUnityImplementationBase.AndroidVersion + ".aar";
        string spilSDKAdMob = "spilsdk-admob-" + SpilUnityImplementationBase.AndroidVersion + ".aar";
        string spilSDKFirebase = "spilsdk-firebase-" + SpilUnityImplementationBase.AndroidVersion + ".aar";

        string[] gradleLines = new string[0];

        bool hasLatestGradleFile = false;
        bool gradleSpilSDKCheck = false;
        bool gradleAdjustSpilSDKCheck = false;
        bool gradleChartboostSpilSDKCheck = false;
        bool gradleAdMobSpilSDKCheck = false;
        bool gradleFirebaseSpilSDKCheck = false;
        
        GUILayout.Label("This tab contains configuration information specific to Android", EditorStyles.boldLabel);
        GUILayout.Label("");
        
        if (!File.Exists(androidFolder + "mainTemplate.gradle")) {
            GUILayout.Label("Implementation type: AAR Files", EditorStyles.boldLabel);
            androidImplementationSelection = 1;
            #if UNITY_2017_1_OR_NEWER
            if (GUILayout.Button("Enable Gradle building using Spil Gradle Template")) {
                File.Copy(spilFolder + "Gradle/mainTemplate.gradle", androidFolder + "mainTemplate.gradle");
                androidImplementationSelection = 0;

                foreach (string fileName in Directory.GetFiles(androidFolder)) {
                    if (fileName.Contains("spilsdk") && fileName.Contains(".aar") && !fileName.Contains("resources")) {
                        File.Delete(fileName);
                    }
                }
            }
            #endif
        }
        else {
            androidImplementationSelection = GUILayout.SelectionGrid(androidImplementationSelection, options, options.Length, EditorStyles.radioButton);
            gradleLines = File.ReadAllLines(androidFolder + "mainTemplate.gradle");
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
                
                if (line.Contains("Unity SpilSDK") && line.Contains(SpilUnityImplementationBase.PluginVersion)) {
                    hasLatestGradleFile = true;
                }
            }
        }
        GUILayout.Label("");
        
        var styleGreen = new GUIStyle(EditorStyles.label);
        Color green = new Color();
        ColorUtility.TryParseHtmlString("#00E676", out green);
        styleGreen.normal.textColor = green;

        var styleRed = new GUIStyle(EditorStyles.label);
        styleRed.normal.textColor = Color.red;

        string spilSDKCheck = "";
        string spilSDKAdjustCheck = "";
        string spilSDKChartboostCheck = "";
        string spilSDKAdMobCheck = "";
        string spilSDKFirebaseCheck = "";

        if (File.Exists(androidFolder + "mainTemplate.gradle") && !hasLatestGradleFile) {
            GUILayout.Label("Not using latest version of the Spil SDK Gradle file!" + spilSDKCheck, styleRed);
            if (GUILayout.Button("Update Gradle File")) {
                File.Delete(androidFolder + "mainTemplate.gradle");
                File.Copy(spilFolder + "Gradle/mainTemplate.gradle", androidFolder + "mainTemplate.gradle");
            }
        }
        
        GUILayout.Label("Installed Modules:", EditorStyles.boldLabel);
        if (!File.Exists(androidFolder + spilSDK) && !gradleSpilSDKCheck) {
            spilSDKCheck = " - False";
            GUILayout.Label(" • SDK Main Module:" + spilSDKCheck, styleRed);
            if (GUILayout.Button("Enable SDK Main Module")) {
                switch (androidImplementationSelection) {
                        case 0:
                            for (int i = 0; i < gradleLines.Length; i++) {
                                if (gradleLines[i].Contains("spilsdk:" + SpilUnityImplementationBase.AndroidVersion) && gradleLines[i].Contains("//")) {
                                    gradleLines[i] = gradleLines[i].Replace("//", "");
                                }
                            }
                            
                            File.WriteAllLines(androidFolder + "mainTemplate.gradle", gradleLines);
                            break;
                        case 1:
                            foreach (string fileName in Directory.GetFiles(androidFolder)) {
                                if (fileName.Contains("spilsdk") && fileName.Contains(".aar") && !fileName.Contains("resources")) {
                                    File.Delete(fileName);
                                }
                            }
                            File.Copy(spilFolder + "Main SDK/spilsdk-" + SpilUnityImplementationBase.AndroidVersion +".aar", androidFolder + "spilsdk-" + SpilUnityImplementationBase.AndroidVersion +".aar");
                            
                            break;
                }
            }
        }
        else {
            spilSDKCheck = " - True";
            GUILayout.Label(" • SDK Main Module:" + spilSDKCheck, styleGreen);
            if (GUILayout.Button("Update SDK Main Module")) {
                switch (androidImplementationSelection) {
                    case 0:
                        for (int i = 0; i < gradleLines.Length; i++) {
                            if (gradleLines[i].Contains("spilsdk:" + SpilUnityImplementationBase.AndroidVersion) && gradleLines[i].Contains("//")) {
                                gradleLines[i] = gradleLines[i].Replace("//", "");
                            }
                        }
                            
                        File.WriteAllLines(androidFolder + "mainTemplate.gradle", gradleLines);
                        
                        foreach (string fileName in Directory.GetFiles(androidFolder)) {
                            if (fileName.Contains("spilsdk") && fileName.Contains(".aar") && !fileName.Contains("resources")) {
                                File.Delete(fileName);
                            }
                        }
                        
                        break;
                    case 1:
                        foreach (string fileName in Directory.GetFiles(androidFolder)) {
                            if (fileName.Contains("spilsdk") && fileName.Contains(".aar") && !fileName.Contains("resources")) {
                                File.Delete(fileName);
                            }
                        }
                        File.Copy(spilFolder + "Main SDK/spilsdk-" + SpilUnityImplementationBase.AndroidVersion +".aar", androidFolder + "spilsdk-" + SpilUnityImplementationBase.AndroidVersion +".aar");
                            
                        for (int i = 0; i < gradleLines.Length; i++) {
                            if (gradleLines[i].Contains("spilsdk") && !gradleLines[i].Contains("//")) {
                                gradleLines[i] = gradleLines[i].Replace("compile", "//compile");
                            }
                        }
                        File.WriteAllLines(androidFolder + "mainTemplate.gradle", gradleLines);
                        
                        break;
                }
            }
        }
        GUILayout.Label("");
        
        if (!File.Exists(androidFolder + spilSDKAdjust) && !gradleAdjustSpilSDKCheck) {
            spilSDKAdjustCheck = " - False";
            GUILayout.Label(" • Adjust (Analytics):" + spilSDKAdjustCheck, styleRed);
            if (GUILayout.Button("Enable Adjust (Analytics) Module")) {
                switch (androidImplementationSelection) {
                    case 0:
                        for (int i = 0; i < gradleLines.Length; i++) {
                            if (gradleLines[i].Contains("spilsdk-adjust:" + SpilUnityImplementationBase.AndroidVersion) && gradleLines[i].Contains("//")) {
                                gradleLines[i] = gradleLines[i].Replace("//", "");
                            }
                        }
                            
                        File.WriteAllLines(androidFolder + "mainTemplate.gradle", gradleLines);
                        break;
                    case 1:
                        foreach (string fileName in Directory.GetFiles(androidFolder)) {
                            if (fileName.Contains("spilsdk-adjust") && fileName.Contains(".aar") && !fileName.Contains("resources")) {
                                File.Delete(fileName);
                            }
                        }
                        File.Copy(spilFolder + "Adjust/spilsdk-adjust-" + SpilUnityImplementationBase.AndroidVersion +".aar", androidFolder + "spilsdk-adjust-" + SpilUnityImplementationBase.AndroidVersion +".aar");
                            
                        break;
                }
            }
        }
        else {
            spilSDKAdjustCheck = " - True";
            GUILayout.Label(" • Adjust (Analytics):" + spilSDKAdjustCheck, styleGreen);
        }
        GUILayout.Label("");

        if (!File.Exists(androidFolder + spilSDKChartboost) && !gradleChartboostSpilSDKCheck) {
            spilSDKChartboostCheck = " - False";
            GUILayout.Label(" • Chartboost (Advertising):" + spilSDKChartboostCheck, styleRed);
            if (GUILayout.Button("Enable Chartboost (Advertising) Module")) {
                switch (androidImplementationSelection) {
                    case 0:
                        for (int i = 0; i < gradleLines.Length; i++) {
                            if (gradleLines[i].Contains("spilsdk-chartboost:" + SpilUnityImplementationBase.AndroidVersion) && gradleLines[i].Contains("//")) {
                                gradleLines[i] = gradleLines[i].Replace("//", "");
                            }
                        }
                            
                        File.WriteAllLines(androidFolder + "mainTemplate.gradle", gradleLines);
                        break;
                    case 1:
                        foreach (string fileName in Directory.GetFiles(androidFolder)) {
                            if (fileName.Contains("spilsdk-chartboost") && fileName.Contains(".aar") && !fileName.Contains("resources")) {
                                File.Delete(fileName);
                            }
                        }
                        File.Copy(spilFolder + "Advertisement/Chartboost/spilsdk-chartboost-" + SpilUnityImplementationBase.AndroidVersion +".aar", androidFolder + "spilsdk-chartboost-" + SpilUnityImplementationBase.AndroidVersion +".aar");
                            
                        break;
                }
            }
        }
        else {
            spilSDKChartboostCheck = " - True";
            GUILayout.Label(" • Chartboost (Advertising):" + spilSDKChartboostCheck, styleGreen);
        }
        GUILayout.Label("");
        
        if (!File.Exists(androidFolder + spilSDKAdMob) && !gradleAdMobSpilSDKCheck) {
            spilSDKAdMobCheck = " - False";
            GUILayout.Label(" • AdMob (Advertising):" + spilSDKAdMobCheck, styleRed);
            if (GUILayout.Button("Enable AdMob (Advertising) Module")) {
                switch (androidImplementationSelection) {
                    case 0:
                        for (int i = 0; i < gradleLines.Length; i++) {
                            if (gradleLines[i].Contains("spilsdk-admob:" + SpilUnityImplementationBase.AndroidVersion) && gradleLines[i].Contains("//")) {
                                gradleLines[i] = gradleLines[i].Replace("//", "");
                            }
                        }
                            
                        File.WriteAllLines(androidFolder + "mainTemplate.gradle", gradleLines);
                        break;
                    case 1:
                        foreach (string fileName in Directory.GetFiles(androidFolder)) {
                            if (fileName.Contains("spilsdk-admob") && fileName.Contains(".aar") && !fileName.Contains("resources")) {
                                File.Delete(fileName);
                            }
                        }
                        File.Copy(spilFolder + "Advertisement/AdMob/spilsdk-admob-" + SpilUnityImplementationBase.AndroidVersion +".aar", androidFolder + "spilsdk-admob-" + SpilUnityImplementationBase.AndroidVersion +".aar");
                            
                        break;
                }
            }
        }
        else {
            spilSDKAdMobCheck = " - True";
            GUILayout.Label(" • AdMob (Advertising):" + spilSDKAdMobCheck, styleGreen);
        }
        GUILayout.Label("");
        
        if (!File.Exists(androidFolder + spilSDKFirebase) && !gradleFirebaseSpilSDKCheck) {
            spilSDKFirebaseCheck = " - False";
            GUILayout.Label(" • Firebase (Analytics & Deep Linking):" + spilSDKFirebaseCheck, styleRed);
            if (GUILayout.Button("Enable Firebase (Analytics & Deep Linking) Module")) {
                switch (androidImplementationSelection) {
                    case 0:
                        for (int i = 0; i < gradleLines.Length; i++) {
                            if (gradleLines[i].Contains("spilsdk-firebase:" + SpilUnityImplementationBase.AndroidVersion) && gradleLines[i].Contains("//")) {
                                gradleLines[i] = gradleLines[i].Replace("//", "");
                            }
                        }
                            
                        File.WriteAllLines(androidFolder + "mainTemplate.gradle", gradleLines);
                        break;
                    case 1:
                        foreach (string fileName in Directory.GetFiles(androidFolder)) {
                            if (fileName.Contains("spilsdk-firebase") && fileName.Contains(".aar") && !fileName.Contains("resources")) {
                                File.Delete(fileName);
                            }
                        }
                        File.Copy(spilFolder + "Firebase/spilsdk-firebase-" + SpilUnityImplementationBase.AndroidVersion +".aar", androidFolder + "spilsdk-firebase-" + SpilUnityImplementationBase.AndroidVersion +".aar");
                            
                        break;
                }
            }
        }
        else {
            spilSDKFirebaseCheck = " - True";
            GUILayout.Label(" • Firebase (Analytics & Deep Linking):" + spilSDKFirebaseCheck, styleGreen);
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

        GUILayout.Label(
            "The label above tells if the Spil SDK will automatically request all the dangerous permissions at the start of your game. If you want to disable it please check your \"AndroidManifest.xml\" file.",
            EditorStyles.wordWrappedLabel);

        GUILayout.Label("");

        if (GUILayout.Button("Verify Android Setup")) {
            VerifyAndroidSetup();
        }
    }

    void DrawEditor() {
        GUILayout.Label("This tab contains configuration information specific to the Unity Editor",
            EditorStyles.boldLabel);
        GUILayout.Label("");

        bool enableLogging = false;
        if (spil != null) {
            enableLogging = spil.SpilLoggingEnabled;
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
            GUILayout.Label(
                "Spil SDK Editor Logging is Disabled! If you want to enable it toggle it from the Spil Game Object!",
                styleRed);
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
        if (androidPackageName == "" || iosBundelId == "") {
            throw new UnityException("Bundle ID is Blank");
        }

        SpilLogging.Log("Getting Default Files for Android: " + androidPackageName + ", And iOS: " + iosBundelId);
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

            playerData.GetField("wallet").RemoveField("offset");
            playerData.GetField("wallet").AddField("offset", 0);

            playerData.GetField("inventory").RemoveField("offset");
            playerData.GetField("inventory").AddField("offset", 0);

            File.WriteAllText(streamingAssetsPath + "/defaultPlayerData.json", playerData.Print(false));
        }
        
        GetTrackingFile();
        AddGoogleAppId();

        if (retrievalError) {
            SpilLogging.Error("Error retrieving default files! Please check the logs!");
        }
        else {
            SpilLogging.Log("Default files succesfully retrieved!");
        }

        retrievalError = false;
    }

    void GetTrackingFile() {
        SpilLogging.Log("Getting Game Specific Tracking File");
        
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var time = (long) (DateTime.Now.ToUniversalTime() - epoch).TotalMilliseconds;
        
        WWW request = new WWW("https://splashscreens.cdn.spilcloud.com/native_game_tracking/" + androidPackageName + ".cs?ts=" + time.ToString());
        while (!request.isDone) ;
        if (request.error != null && !request.error.Equals("")) {
            SpilLogging.Log("No Game Specific Tracking File available!");
        }
        else {
            string fileName = "";
            string version = "";
            using (StringReader reader = new StringReader(request.text)) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    if (line.Contains("FileName")) {
                        fileName = line.Split('"').Where((s, i) => i % 2 == 1).ToList()[0];
                    }
                    
                    if (line.Contains("Version")) {
                        version = line.Split('"').Where((s, i) => i % 2 == 1).ToList()[0];
                    }
                }
            }

            if (!File.Exists(Application.dataPath + "/Spilgames/Base/Tracking/" + fileName + ".cs")) {
                File.WriteAllText(Application.dataPath + "/Spilgames/Base/Tracking/" + fileName + ".cs", request.text);
            } else {
                File.Delete(Application.dataPath + "/Spilgames/Base/Tracking/" + fileName + ".cs");
                File.WriteAllText(Application.dataPath + "/Spilgames/Base/Tracking/" + fileName + ".cs", request.text);
            }
        }
    }
    
    string GetData(string type) {
        string gameData = "";

        if (type.Equals("requestConfig")) {
            JSONObject combined = new JSONObject();
            JSONObject tempAndroid = new JSONObject();
            JSONObject tempIOS = new JSONObject();

            WWWForm form = GetFormData("android");
            form.AddField("name", type);
            WWW request =
                new WWW(
                    "https://apptracker.spilgames.com/v1/native-events/event/android/" + androidPackageName + "/" +
                    type,
                    form);
            while (!request.isDone)
                ;
            if (request.error != null && !request.error.Equals("")) {
                SpilLogging.Error("Error getting game data: " + request.error + ". Request: " + request.url +
                                  ". App version: " + androidGameVersion);
                combined.AddField("androidSdkConfig", "");
                retrievalError = true;
            }
            else {
                tempAndroid = new JSONObject(request.text).GetField("data");
            }

            WWWForm form2 = GetFormData("ios");
            form2.AddField("name", type);
            WWW request2 =
                new WWW("https://apptracker.spilgames.com/v1/native-events/event/ios/" + iosBundelId + "/" + type,
                    form2);
            while (!request2.isDone)
                ;
            if (request2.error != null && !request2.error.Equals("")) {
                SpilLogging.Error("Error getting game data: " + request2.error + ". Request: " + request2.url +
                                  ". App version: " + iosGameVersion);
                combined.AddField("iosSdkConfig", "");
                retrievalError = true;
            }
            else {
                tempIOS = new JSONObject(request2.text).GetField("data");
            }

            if (EditorUserBuildSettings.activeBuildTarget.ToString().Trim().ToLower().Equals("android")) {
                combined = tempAndroid;

                if (combined.HasField("iosSdkConfig")) {
                    combined.RemoveField("iosSdkConfig");
                }

                combined.AddField("iosSdkConfig",
                    tempIOS.GetField("iosSdkConfig"));
            }
            else if (EditorUserBuildSettings.activeBuildTarget.ToString().Trim().ToLower().Equals("ios")) {
                combined = tempIOS;

                if (combined.HasField("androidSdkConfig")) {
                    combined.RemoveField("androidSdkConfig");
                }

                combined.AddField("androidSdkConfig",
                    tempAndroid.GetField("androidSdkConfig"));
            }

            gameData = combined.Print(false);
        }
        else {
            WWWForm form = GetFormData(EditorUserBuildSettings.activeBuildTarget.ToString().Trim().ToLower());
            form.AddField("name", type);

            string bundleIdentifier = null;
            #if UNITY_ANDROID
            bundleIdentifier = androidPackageName;
            #elif UNITY_IPHONE || UNITY_TVOS
            bundleIdentifier = iosBundelId;
#endif

            WWW request =
                new WWW(
                    "https://apptracker.spilgames.com/v1/native-events/event/" +
                    EditorUserBuildSettings.activeBuildTarget.ToString().Trim().ToLower() + "/" + bundleIdentifier +
                    "/" + type, form);
            while (!request.isDone)
                ;
            if (request.error != null && !request.error.Equals("")) {
                SpilLogging.Error("Error getting data: " + request.error + " " + request.text);
                retrievalError = true;
            }
            else {
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
            dummyData.AddField("packageName", androidPackageName);
        }
        else {
            dummyData.AddField("bundleId", iosBundelId);
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
        if (spil != null && spil.EditorDebugMode) {
            form.AddField("debugMode", Convert.ToString(spil.EditorDebugMode).ToLower());
        }

        return form;
    }

    void VerifyAndroidSetup() {
        bool isEverythingCorrect = true;

        string androidFolder = "Assets/Plugins/Android/";

        if (Directory.Exists(androidFolder + "GooglePlayServices")) {
            SpilLogging.Error(
                "The contents of the GooglePlayServices folder should be copied into 'Assets/Plugins/Android/' and afterwards the folder should be removed");
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
            SpilLogging.Error("Your app's AndroidManifest.xml file does not contain \"<application>\" node.");
            SpilLogging.Error("Unable to verify if the values were correct");

            return;
        }

        if (!applicationNode.Attributes["android:name"].Value.Equals("com.spilgames.spilsdk.activities.SpilSDKApplication")) {
            SpilLogging.Error(
                "The application name from your \"AndroidManifest.xml\" file is set incorrectly. Please set it to either \"com.spilgames.spilsdk.activities.SpilSDKApplication\" if you want for the Spil SDK to function correctly");
            isEverythingCorrect = false;
        }

        if (applicationNode.Attributes["android:name"].Value.Equals("com.spilgames.spilsdk.activities.SpilSDKApplicationWithFabric")) {
            SpilLogging.Error(
                "The application name found in your \"AndroidManifest.xml\" file is set incorrectly. The application name \"com.spilgames.spilsdk.activities.SpilSDKApplicationWithFabric\" has been removed from the Spil SDK. Please use the standard \"com.spilgames.spilsdk.activities.SpilSDKApplication\". The Fabric (Crashlytics functionality has been moved to this application name.");
            isEverythingCorrect = false;
        }

        bool isUnityRequestPermissionDisabled = false;

        foreach (XmlNode node in applicationNode.ChildNodes) {
            if (node.Name.Equals("activity")) {
                foreach (XmlNode subNode in node.ChildNodes) {
                    if (subNode.Name.Equals("intent-filter")) {
                        foreach (XmlNode bottomNode in subNode.ChildNodes) {
                            if (bottomNode.Name.Equals("action") && bottomNode.Attributes["android:name"].Value
                                    .Equals("android.intent.action.MAIN")) {
                                if (!(node.Attributes["android:name"].Value
                                        .Equals("com.spilgames.spilsdk.activities.SpilUnityActivity")) &&
                                    !(node.Attributes["android:name"].Value
                                        .Equals("com.spilgames.spilsdk.activities.SpilUnityActivityWithPrime")) &&
                                    !(node.Attributes["android:name"].Value
                                        .Equals("com.spilgames.spilsdk.activities.SpilUnityActivityWithAN"))) {
                                    SpilLogging.Error(
                                        "The Main Activity name from your \"AndroidManifest.xml\" file is set incorrectly. Please set it to either \"com.spilgames.spilsdk.activities.SpilUnityActivity\", \"com.spilgames.spilsdk.activities.SpilUnityActivityWithPrime\" (if you are using Prime31 Plugin) or \"com.spilgames.spilsdk.activities.SpilUnityActivityWithAN\" (if you are using Android Native Plugin) if you want for the Spil SDK to function correctly");
                                    isEverythingCorrect = false;
                                }
                            }
                        }
                    }
                }
            }

            if (node.Name.Equals("meta-data")) {
                if (node.Attributes["android:name"].Value.Equals("unityplayer.SkipPermissionsDialog") &&
                    node.Attributes["android:value"].Value.Equals("true")) {
                    isUnityRequestPermissionDisabled = true;
                    isEverythingCorrect = false;
                }
            }
        }

        if (!isUnityRequestPermissionDisabled) {
            SpilLogging.Error(
                "You did not disable the automatic Unity permission request. Please add the following line to your \"applicaiton\" tag: \"<meta-data android:name=\"unityplayer.SkipPermissionsDialog\" android:value=\"true\" />\". This is required in order to not have any problems with the Spil SDK's permission system. Keep in mind that all your dangerous permissions will be handled by the Spil SDK automatically");
        }

        if (!isEverythingCorrect) {
            SpilLogging.Log("Verification Complete! The Spil SDK for Android is configured correctly!");
        }
        else {
            SpilLogging.Error("Verification Complete! Something was not configured correctly!! Please check the logs");
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
            SpilLogging.Error("Your app's AndroidManifest.xml file does not contain \"<application>\" node.");
            SpilLogging.Error("Unable to verify if the values were correct");

            return true;
        }

        foreach (XmlNode node in applicationNode.ChildNodes) {
            if (node.Name.Equals("meta-data")) {
                if (node.Attributes["android:name"].Value.Equals("spil.permissions.DisableAutoRequest") &&
                    node.Attributes["android:value"].Value.Equals("true")) {
                    return false;
                }
            }
        }

        return true;
    }

    public static void AddGoogleAppId() {
        string androidFolder = "Assets/Plugins/Android/";
        string spilSDKFirebase = "spilsdk-firebase-" + SpilUnityImplementationBase.AndroidVersion + ".aar";
        string gameConfigPath = Application.streamingAssetsPath + "/defaultGameConfig.json";

        string savePath = androidFolder + "res/values/";

        if (!File.Exists(gameConfigPath) || !File.Exists(androidFolder + spilSDKFirebase)) {
            return;
        }

        if (!Directory.Exists(androidFolder + "res")) {
            Directory.CreateDirectory(androidFolder + "res");
        }
        
        if (!Directory.Exists(androidFolder + "res/values")) {
            Directory.CreateDirectory(androidFolder + "res/values");
        }

        JSONObject localConfig = new JSONObject(File.ReadAllText(gameConfigPath)).GetField("androidSdkConfig");
        string firebaseAppId = localConfig.GetField("firebase").GetField("applicationId").str;

        if (File.Exists(savePath + "strings.xml")) {
            XmlDocument stringsFileXML = new XmlDocument();
            stringsFileXML.Load(savePath + "strings.xml");

            XmlElement xmlRoot = stringsFileXML.DocumentElement;

            XmlNodeList nodes = stringsFileXML.GetElementsByTagName("string");
            for (int i = 0; i < nodes.Count; i++) {
                if (nodes[i].Attributes["name"].Value == "google_app_id") {
                    xmlRoot.RemoveChild(nodes[i]);
                }
            }
            
            XmlNode node = stringsFileXML.CreateElement("string");
            XmlAttribute attribute = stringsFileXML.CreateAttribute("name");
            attribute.Value = "google_app_id";
            node.Attributes.Append(attribute);
            node.InnerText = firebaseAppId;

            xmlRoot.AppendChild(node);
            
            stringsFileXML.Save(savePath + "strings.xml");
        } else {
            string fileOutput = "<?xml version='1.0' encoding='utf-8'?><resources><string name='google_app_id'>" + firebaseAppId + "</string></resources>";

            File.WriteAllText(savePath + "strings.xml", fileOutput);
        }
    }
}