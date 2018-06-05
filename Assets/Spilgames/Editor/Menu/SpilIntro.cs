﻿using System.IO;
using SpilGames.Unity.Base.Implementations;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SpilIntro : EditorWindow {
    private int tabSelected = 0;
    
    private Vector2 scrollPos;
    private Texture2D logo = null;
    
    private static GUIStyle centeredStyle;
    
    static SpilIntro() {
        string spilIntroductionShown = PlayerPrefs.GetString("spilIntroduction", "0");
        if (spilIntroductionShown.Equals("0") || !spilIntroductionShown.Equals(SpilUnityImplementationBase.PluginVersion)) {
            PlayerPrefs.SetString("spilIntroduction", SpilUnityImplementationBase.PluginVersion);
            Init();
        }
    }

    void OnEnable() {
        Vector2 size = new Vector2(position.width, 256);
        
        logo = new Texture2D((int) size.x, (int) size.y, TextureFormat.RGB24,false);
        logo.LoadImage(File.ReadAllBytes(Application.dataPath + "/Resources/Spilgames/PrivacyPolicy/Images/spillogo.png"));
    }

    [MenuItem("Spil SDK/Introduction", false, 0)]
    static void Init() {
        SpilIntro window = (SpilIntro) EditorWindow.GetWindow(typeof(SpilIntro));
        window.autoRepaintOnSceneChange = true;
        window.titleContent.text = "Introduction";
        window.Show();
    }

    void OnGUI() {
        GUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));
        GUILayout.BeginHorizontal();
        if (GUILayout.Toggle(tabSelected == 0, "Introduction", EditorStyles.toolbarButton)) {
            tabSelected = 0;
        }
//
//        if (GUILayout.Toggle(tabSelected == 1, "What's New", EditorStyles.toolbarButton)) {
//            tabSelected = 1;
//        }
        
        GUILayout.EndHorizontal();
        
        switch (tabSelected) {
            case 0:
                DrawIntroduction();
                break;
            case 1:
                DrawWhatsNew();
                break;
        }
        
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void DrawIntroduction() {
        if (centeredStyle == null) {
            centeredStyle = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, wordWrap = true, fontStyle = FontStyle.Bold};
        }
        
        GUILayout.Label("");
        GUILayout.Label(logo, centeredStyle);
        
        GUILayout.Label("");
        GUILayout.Label("Welcome to the Spil SDK Version " + SpilUnityImplementationBase.PluginVersion, centeredStyle);
        
        GUILayout.Label("");
        GUILayout.Label("If this is the first time you are using the Spil SDK please keep in mind the following steps:", centeredStyle);
        GUILayout.Label("");
        GUILayout.Label("    1. Make sure to configure the game bundle id, that it matches the one that has been provided by the Spil representative and that it matches the value defined in SLOT.", EditorStyles.wordWrappedLabel);
        GUILayout.Label("    2. Create a GameObject in the first scene of your game, name that GameObject SpilSDK and assign the Spil (Script) component to it.", EditorStyles.wordWrappedLabel);
        GUILayout.Label("    3. Input the Android Project Id (which you received from the Spil representative) into your SpilSDK GameObject. This can be done either by accessing the Android Configuration menu (Top Bar->Spil SDK->Configuration->Android Tab) and pasting the id there or selecting the GameObject, inspecting it and pasting the id in the Android Settings/Project Id section. An example of a Project Id: 492493592102.", EditorStyles.wordWrappedLabel);
        GUILayout.Label("    4. Create the default configuration files by going to the Spil SDK menu (Top Bar), selecting the Configuration menu, going into the General tab and clicking Create Default Configuration Files button. Make sure that the version code is set correctly and matches the version codes defines in SLOT.", EditorStyles.wordWrappedLabel);
        GUILayout.Label("    5. By default the Spil SDK will have debug mode turned off when using it in the editor. This setting will also influence what default configuration will be created (debug/testing(SLOT) or production). If you want to enable debug mode, you can do so by enabling it in the Spil SDK GameObject, in the Editor Settings section.", EditorStyles.wordWrappedLabel);
        GUILayout.Label("    6. Pay attention to the Unity console as the Spil SDK provides useful information. You can also turn off Spil Logging by disabling the setting in the Spil SDK GameObject.", EditorStyles.wordWrappedLabel);
        GUILayout.Label("    7. Start using the Spil SDK by calling 'Spil.Instance.' or 'Spil.PlayerData.' depending on your needs. Links to all the features of the SDK can be found in the Spil SDK menu (Top Bar), selecting the Documentation menu.", EditorStyles.wordWrappedLabel);
        GUILayout.Label("");
        GUILayout.Label("When setting up your game to create an Android build, make sure you have done the following steps:", centeredStyle);
        GUILayout.Label("");
        GUILayout.Label("    1. Used the correct AndroidManifest.xml definition. This means that the Android application is the SpilSDKApplication (com.spilgames.spilsdk.activities.SpilSDKApplication) and the main Android activity is the SpilUnityActivity (com.spilgames.spilsdk.activities.SpilUnityActivity). We advise that you pay attention to the supplied AndroidManifest.xml contained in the Plugin package..", EditorStyles.wordWrappedLabel);
        GUILayout.Label("    2. Make sure to copy the Google Play Services libraries in the correct folder and that there are no multiple version of the Google Play Services.", EditorStyles.wordWrappedLabel);
        GUILayout.Label("    3. Before building for Android, go to the Spil SDK menu (Top Bar), select the Configuration menu, go into the Android tab and click the Verify Android Setup button. Pay attention to the console for any potential issues.", EditorStyles.wordWrappedLabel);
        GUILayout.Label("");
        GUILayout.Label("If you have any other question feel free to contact the Spil SDK developers in the Slack channel assigned to your game.", centeredStyle);
        GUILayout.Label("");
        GUILayout.Label("Have fun and create great games!", centeredStyle);
        GUILayout.Label("");
    }

    private void DrawWhatsNew() {
        if (centeredStyle == null) {
            centeredStyle = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, wordWrap = true, fontStyle = FontStyle.Bold};
        }
        
        GUILayout.Label("");
        GUILayout.Label(logo, centeredStyle);
        
        GUILayout.Label("");
    }
}