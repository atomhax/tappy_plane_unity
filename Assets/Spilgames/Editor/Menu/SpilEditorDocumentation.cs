﻿using UnityEngine;
using System.IO;
using UnityEditor;

public class SpilEditorDocumentation : EditorWindow {
    private int tabSelected = 0;
    private Texture2D logo = null;
    private static GUIStyle centeredStyle;

    [MenuItem("Spil SDK/Documentation", false, 2)]
    static void Init() {
        SpilEditorDocumentation window = (SpilEditorDocumentation) EditorWindow.GetWindow(typeof(SpilEditorDocumentation));
        window.autoRepaintOnSceneChange = true;
        window.titleContent.text = "Documentation";
        window.minSize = new Vector2(1000, 700);
        window.Show();
    }

    void OnEnable() {
        Vector2 size = new Vector2(position.width, 256);

        logo = new Texture2D((int) size.x, (int) size.y, TextureFormat.RGB24, false);
        logo.LoadImage(File.ReadAllBytes(Application.dataPath + "/Resources/Spilgames/PrivacyPolicy/Images/spillogo.png"));
    }

    void OnGUI() {
        EditorGUILayout.BeginVertical();
        if (centeredStyle == null) {
            centeredStyle = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, wordWrap = true, fontStyle = FontStyle.Bold};
        }
        
        GUILayout.Label("");
        GUILayout.Label(logo, centeredStyle);
        
        EditorGUILayout.EndVertical();
        
        GUILayout.BeginArea(new Rect(10, 100, 200, position.height));
        GUILayout.BeginVertical();
        if (GUILayout.Toggle(tabSelected == 0, "General", EditorStyles.toolbarButton)) {
            tabSelected = 0;
        }

        if (GUILayout.Toggle(tabSelected == 1, "Event Tracking", EditorStyles.toolbarButton)) {
            tabSelected = 1;
        }

        if (GUILayout.Toggle(tabSelected == 2, "Game Balancing", EditorStyles.toolbarButton)) {
            tabSelected = 2;
        }

        if (GUILayout.Toggle(tabSelected == 3, "Smart Advertisements", EditorStyles.toolbarButton)) {
            tabSelected = 3;
        }

        if (GUILayout.Toggle(tabSelected == 4, "Push Notifications", EditorStyles.toolbarButton)) {
            tabSelected = 4;
        }

        if (GUILayout.Toggle(tabSelected == 5, "User Identification", EditorStyles.toolbarButton)) {
            tabSelected = 5;
        }

        if (GUILayout.Toggle(tabSelected == 6, "Shop, Wallet & Inventory Control", EditorStyles.toolbarButton)) {
            tabSelected = 6;
        }

        if (GUILayout.Toggle(tabSelected == 7, "Working with the Social Login Feature", EditorStyles.toolbarButton)) {
            tabSelected = 7;
        }

        if (GUILayout.Toggle(tabSelected == 8, "Managing In-Game Purchases", EditorStyles.toolbarButton)) {
            tabSelected = 8;
        }

        if (GUILayout.Toggle(tabSelected == 9, "Promotions", EditorStyles.toolbarButton)) {
            tabSelected = 9;
        }

        if (GUILayout.Toggle(tabSelected == 10, "Implementing Customer Support", EditorStyles.toolbarButton)) {
            tabSelected = 10;
        }

        if (GUILayout.Toggle(tabSelected == 11, "Working with Game States", EditorStyles.toolbarButton)) {
            tabSelected = 11;
        }

        if (GUILayout.Toggle(tabSelected == 12, "Asset Bundles", EditorStyles.toolbarButton)) {
            tabSelected = 12;
        }
        
        if (GUILayout.Toggle(tabSelected == 13, "Splash and Daily Bonus Screens", EditorStyles.toolbarButton)) {
            tabSelected = 13;
        }

        if (GUILayout.Toggle(tabSelected == 14, "Tiered Events", EditorStyles.toolbarButton)) {
            tabSelected = 14;
        }
        
        if (GUILayout.Toggle(tabSelected == 15, "Live Events", EditorStyles.toolbarButton)) {
            tabSelected = 15;
        }

        if (GUILayout.Toggle(tabSelected == 16, "Deep Linking", EditorStyles.toolbarButton)) {
            tabSelected = 16;
        }

        if (GUILayout.Toggle(tabSelected == 17, "Handling Errors", EditorStyles.toolbarButton)) {
            tabSelected = 17;
        }

        if (GUILayout.Toggle(tabSelected == 18, "Anti-Cheating", EditorStyles.toolbarButton)) {
            tabSelected = 18;
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(225, 100, position.width - 250, position.height));
        GUILayout.BeginVertical();
        EditorStyles.label.wordWrap = true;
        switch (tabSelected) {
            case 0:
                DrawGeneral();
                break;
            case 1:
                DrawEventTracking();
                break;
            case 2:
                DrawGameConfig();
                break;
            case 3:
                DrawAdvertisement();
                break;
            case 4:
                DrawPushNotifications();
                break;
            case 5:
                DrawUserId();
                break;
            case 6:
                DrawWalletShopInventory();
                break;
            case 7:
                DrawSocialLogin();
                break;
            case 8:
                DrawIAPPackages();
                break;
            case 9:
                DrawPromotions();
                break;
            case 10:
                DrawCustomerSupport();
                break;
            case 11:
                DrawGameState();
                break;
            case 12:
                DrawAssetBundles();
                break;
            case 13:
                DrawSplashScreenDailyBonus();
                break;
            case 14:
                DrawTieredEvents();
                break;
            case 15:
                DrawLiveEvents();
                break;
            case 16:
                DrawDeepLinking();
                break;
            case 17:
                DrawHandlingErrors();
                break;
            case 18:
                DrawAntiCheating();
                break;
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private void DrawGeneral() {
        GUILayout.Label("Introduction", EditorStyles.boldLabel);
        GUILayout.Label(
            "The Spil Game Unity Plugin contains various features that let Spil Games get detailed user behavior insights, get the highest ad revenues and tweak your game. The Unity plugin helps developers to integrate all required SDK’s as quick as possible with minimum effort.",
            EditorStyles.wordWrappedLabel);
        GUILayout.Label("");
        GUILayout.Label("You can find more general information about the Spil SDK here:", EditorStyles.wordWrappedLabel);
        if (GUILayout.Button("Implementing The Spil SDK", GUILayout.Width(400))) {
            Application.OpenURL("http://www.spilgames.com/integration/unity/implementing-spil-sdk/");
        }
        
        GUILayout.Label("");
        GUILayout.Label("When setting up your game to create an Android build, make sure you have done the following steps:", EditorStyles.wordWrappedLabel);
        GUILayout.Label("");
        GUILayout.Label("1. Used the correct AndroidManifest.xml definition. This means that the Android application is the SpilSDKApplication (com.spilgames.spilsdk.activities.SpilSDKApplication) and the main Android activity is the SpilUnityActivity (com.spilgames.spilsdk.activities.SpilUnityActivity). We advise that you pay attention to the supplied AndroidManifest.xml contained in the Plugin package.", EditorStyles.wordWrappedLabel);
        GUILayout.Label("2. The Google Play Services and AppCompat Dependencies are resolved using the mainTemplate.gradle file provided by the Spil SDK. If you are using Gradle building, make sure you do not have the .aar files for Google Play Services or Appcompat in your Plugins/Android folder as there will be dependency conflicts.", EditorStyles.wordWrappedLabel);
        GUILayout.Label("3. If you are not using the Unity Version 2017.+ or Gradle Main Template provided in the SDK make sure to copy the Google Play Services libraries in the correct folder (Plugins/Android) and that there are no multiple version of the Google Play Services. No subdirectories should be used.", EditorStyles.wordWrappedLabel);
        GUILayout.Label("4. Before building for Android, go to the Spil SDK menu (Top Bar), select the Configuration menu, go into the Android tab and click the Verify Android Setup button. Pay attention to the console for any potential issues.", EditorStyles.wordWrappedLabel);
        GUILayout.Label("");
    }

    private void DrawEventTracking() {
        GUILayout.Label("The Spil Games platform provides you with dedicated insight into how your users are experiencing your games. This is enabled through powerful event tracking.", EditorStyles.wordWrappedLabel);
        GUILayout.Label("");
        GUILayout.Label("You can find more Event Tracking information here:", EditorStyles.wordWrappedLabel);
        if (GUILayout.Button("Spil SDK Event tracking", GUILayout.Width(400))) {
            Application.OpenURL("http://www.spilgames.com/integration/unity/implementing-spil-sdk/spil-sdk-event-tracking/");
        }
    }

    private void DrawGameConfig() {
        GUILayout.Label(
            "Balance is key to a successful game. In single-player games, it determines whether the challenge level is appropriate to the audience. If there are multiple strategies or paths within your game to victory, it controls whether following one strategy is better or worse than following another. Within games that have several similar objects (such as cards in a trading-card game or weapons in a role-playing game), it regulates the objects themselves and, specifically, whether different objects have the same cost/benefit ratio.",
            EditorStyles.wordWrappedLabel);
        GUILayout.Label("");
        GUILayout.Label("You can find more Game Balance information here:", EditorStyles.wordWrappedLabel);
        if (GUILayout.Button("Spil SDK Game Balancing", GUILayout.Width(400))) {
            Application.OpenURL("http://www.spilgames.com/integration/unity/implementing-spil-sdk/spil-sdk-game-config/");
        }
    }

    private void DrawAdvertisement() {
        GUILayout.Label(
            "Typically, the monetization of your games comes through advertising. Showing the right ads to the right users at the right time can highly influence the revenues you can earn per ad. The Spil Games platform supports the use of Interstitial (smart) ads based on user behavior.",
            EditorStyles.wordWrappedLabel);
        GUILayout.Label("");
        GUILayout.Label("You can find more Smart Advertisements here:", EditorStyles.wordWrappedLabel);
        if (GUILayout.Button("Spil SDK Smart Advertisements", GUILayout.Width(400))) {
            Application.OpenURL("http://www.spilgames.com/integration/unity/implementing-spil-sdk/spil-sdk-advertisement-2/");
        }
    }

    private void DrawPushNotifications() {
        GUILayout.Label(
            "There are various reasons to use push notifications in your games, and they have numerous benefits in terms of engaging with customers and driving traffic. The use of push notifications can enhance the gaming experience for your users by adding another element to the game analysis, as well as helping you monetize your game.",
            EditorStyles.wordWrappedLabel);
        GUILayout.Label("");
        GUILayout.Label("You can find more Push Notification information here:", EditorStyles.wordWrappedLabel);
        if (GUILayout.Button("Spil SDK Push Notifications", GUILayout.Width(400))) {
            Application.OpenURL("http://www.spilgames.com/integration/unity/implementing-spil-sdk/spil-sdk-push-notifications/");
        }
    }

    private void DrawUserId() {
        GUILayout.Label(
            "When a user contacts Spil customer support he/she may be asked for a Spil user id. Spil currently does not require registration for users and so cannot link an email to a Spil user id. All users are essentially guest users and thus have an anonymous user-id.",
            EditorStyles.wordWrappedLabel);
        GUILayout.Label("");
        GUILayout.Label("You can find more User Identification information here:", EditorStyles.wordWrappedLabel);
        if (GUILayout.Button("Spil SDK User Identification", GUILayout.Width(400))) {
            Application.OpenURL("http://www.spilgames.com/integration/unity/implementing-spil-sdk/spil-sdk-user-id/");
        }
    }

    private void DrawWalletShopInventory() {
        GUILayout.Label(
            "Wallet: \nThe wallet feature is holding a users virtual balance of a particular currency. E.g. the user has 100 coins in his wallet. A wallet can contain multiple currencies, e.g. coins and diamonds.",
            EditorStyles.wordWrappedLabel);
        GUILayout.Label("In-game shop: \nA user can buy items or bundles (a pack of multiple items) with his virtual currency. Let’s say a user can buy a sword with 100 coins.", EditorStyles.wordWrappedLabel);
        GUILayout.Label("Inventory: \nWhen a user bought an item in the in-game shop it will be added to his personal inventory.", EditorStyles.wordWrappedLabel);

        GUILayout.Label("");
        GUILayout.Label("You can find more Wallet, Inventory or Shop information here:", EditorStyles.wordWrappedLabel);
        if (GUILayout.Button("Spil SDK Wallet, Shop & Inventory", GUILayout.Width(400))) {
            Application.OpenURL("http://www.spilgames.com/integration/unity/implementing-spil-sdk/spil-sdk-wallet-shop-inventory/");
        }
    }

    private void DrawSocialLogin() {
        GUILayout.Label(
            "By default, a user’s data is lost when they remove a game from their device. If they later re-install it, they start again as a new user. The social login feature solves this problem by binding the user’s progress to a specified Facebook account. After re-installing the game on the same or another device via a Facebook login, they can continue from their previous state. In addition, this feature can also be used to enable users to play across multiple devices while preserving their game progress. In this way, the user’s game experience can be enhanced.",
            EditorStyles.wordWrappedLabel);
        GUILayout.Label("");
        GUILayout.Label("You can find more Social Login information here:", EditorStyles.wordWrappedLabel);
        if (GUILayout.Button("Spil SDK Social Login", GUILayout.Width(400))) {
            Application.OpenURL("http://www.spilgames.com/integration/unity/implementing-spil-sdk/working-social-login-feature/");
        }
    }

    private void DrawIAPPackages() {
        GUILayout.Label(
            "In-games purchases refer to items or points that a player can buy for use within a game to improve a character or enhance the playing experience. These, together with the use of smart ads (described in “Smart Advertisements Support”) are the primary means by which games produce revenue for their makers. ",
            EditorStyles.wordWrappedLabel);
        GUILayout.Label("");
        GUILayout.Label("You can find more Managing In-Game Purchases information here:", EditorStyles.wordWrappedLabel);
        if (GUILayout.Button("Spil SDK Managing In-Game Purchases", GUILayout.Width(400))) {
            Application.OpenURL("http://www.spilgames.com/integration/unity/implementing-spil-sdk/sdk-iap-packages-promotions/");
        }
    }

    private void DrawPromotions() {
        GUILayout.Label(
            "The Promotions feature offers the possibility to award users with additional currencies and/or items for a limited amount of time or to reduce the prices for in-game bundles. The feature can be applied to In-Game Bundles and IAP Packages. ",
            EditorStyles.wordWrappedLabel);
        GUILayout.Label("");
        GUILayout.Label("You can find more SDK Promotions information here:", EditorStyles.wordWrappedLabel);
        if (GUILayout.Button("Spil SDK IAP Promotions", GUILayout.Width(400))) {
            Application.OpenURL("http://www.spilgames.com/integration/unity/implementing-spil-sdk/promotions/");
        }
    }

    private void DrawCustomerSupport() {
        GUILayout.Label(
            "The issue of customer support is an important aspect of games delivery. It is strongly recommended that you review its requirements with your Spil Games Account Manager as part of the initial engagement",
            EditorStyles.wordWrappedLabel);
        GUILayout.Label("");
        GUILayout.Label("You can find more Customer Support integration information here:", EditorStyles.wordWrappedLabel);
        if (GUILayout.Button("Spil SDK Cutomer Support", GUILayout.Width(400))) {
            Application.OpenURL("http://www.spilgames.com/integration/unity/implementing-spil-sdk/spil-sdk-customer-support/");
        }
    }

    private void DrawGameState() {
        GUILayout.Label(
            "The Spil SDK also allows the saving of custom data blobs, e.g. game states, that can be associated to a specific user. These game states come in two flavors, public and private. Private game states can be saved at any time, and that game state will be associated with the Spil guest user id. If you plan on saving a public game state, then you are required to provide a custom user id and provider.",
            EditorStyles.wordWrappedLabel);
        GUILayout.Label("");
        GUILayout.Label("You can find more Game State information here:", EditorStyles.wordWrappedLabel);
        if (GUILayout.Button("Spil SDK Working with Game State", GUILayout.Width(400))) {
            Application.OpenURL("http://www.spilgames.com/integration/unity/implementing-spil-sdk/spil-sdk-game-state/");
        }
    }

    private void DrawAssetBundles() {
        GUILayout.Label(
            "The Spil Games framework offers the possibility to have SLOT configuration for Asset Bundles. Asset Bundles configurations can be done through SLOT, and as so please ask your Product Manager to configure this before starting the implementation of this feature.",
            EditorStyles.wordWrappedLabel);
        GUILayout.Label("");
        GUILayout.Label("You can find more Asset Bundles information here:", EditorStyles.wordWrappedLabel);
        if (GUILayout.Button("Spil SDK Asset Bundles", GUILayout.Width(400))) {
            Application.OpenURL("http://www.spilgames.com/integration/unity/implementing-spil-sdk/implementing-asset-bundles-configurations/");
        }
    }
    
    private void DrawSplashScreenDailyBonus() {
        GUILayout.Label(
            "The Spil Games framework offers the possibility of supporting both splash and bonus screens. Splash screens appear while a game is loading. They are a very useful means of engaging with the user community, and can be used for a variety of reasons, including as an additional form of advertising, to restrict access to content such as pornography or gambling, and to grab the user’s attention through special offers. Typically, daily bonus screens are used to reward users each time they return to the game. Within the Spil Games framework, both screen types are implemented as HTML5 webpages that appear as pop-ups within the game. This has the advantage that changes can be made to them without the need to update the game itself.",
            EditorStyles.wordWrappedLabel);
        GUILayout.Label("");
        GUILayout.Label("You can find more Daily Bonus and Splash Screen information here:", EditorStyles.wordWrappedLabel);
        if (GUILayout.Button("Spil SDK Daily Bonus and Splash Screen", GUILayout.Width(400))) {
            Application.OpenURL("http://www.spilgames.com/integration/unity/implementing-spil-sdk/spil-sdk-splash-daily-bonus-screen/");
        }
    }

    private void DrawTieredEvents() {
        GUILayout.Label(
            "The Spil Games framework offers the possibility to provide predefined tiered events. A tiered event is a unique event(not to be confused with tracking events) which allows the user to get special rewards by spending currencies or items within the game. A tiered event consists of multiple tiers, each individual tier has a certain goal. The goal is always to spend a certain amount of currency or items and the player will get a reward after completing a tier and then progresses to the next tier. The tiered event is finished when all the tiers are completed or when the end date of the event has been reached. ",
            EditorStyles.wordWrappedLabel);
        GUILayout.Label("");
        GUILayout.Label("You can find more Tiered Events information here:", EditorStyles.wordWrappedLabel);
        if (GUILayout.Button("Spil SDK Tiered Events", GUILayout.Width(400))) {
            Application.OpenURL("http://www.spilgames.com/integration/unity/implementing-spil-sdk/tiered-events/");
        }
    }
    
    private void DrawLiveEvents() {
        GUILayout.Label(
            "The Spil Games framework offers the possibility to provide predefined live events. Such events allows the user to receive certain rewards by collecting and applying special in-game items which are only available during the duration of the event. Live events are usually combined with push notifications, splash screens and daily bonuses to motivate users to progress faster within the live event. ",
            EditorStyles.wordWrappedLabel);
        GUILayout.Label("");
        GUILayout.Label("You can find more Live Events information here:", EditorStyles.wordWrappedLabel);
        if (GUILayout.Button("Spil SDK Live Events", GUILayout.Width(400))) {
            Application.OpenURL("http://www.spilgames.com/integration/unity/implementing-spil-sdk/spil-sdk-live-events/");
        }
    }

    private void DrawDeepLinking() {
        GUILayout.Label(
            "The Spil SDK provides the functionality of Deep Linking. Deep Linking consists of using a URI (uniform resource identifier) that links to either installing the app on the app store or opening the app with a specific function in mind. The Spil Deep Linking is geared towards rewarding the user for clicking on the link, by providing meaningful content inside the game.",
            EditorStyles.wordWrappedLabel);
        GUILayout.Label("");
        GUILayout.Label("You can find more Deep Linking information here:", EditorStyles.wordWrappedLabel);
        if (GUILayout.Button("Spil SDK Deep Linking", GUILayout.Width(400))) {
            Application.OpenURL("http://www.spilgames.com/integration/unity/implementing-spil-sdk/spil-sdk-deep-linking/");
        }
    }

    private void DrawHandlingErrors() {
        GUILayout.Label(
            "The Spil SDK sends a multitude of error messages to the different error callbacks that reside in each of the SDK features. In order to have a better understanding where the SDK might fail, a list is provided bellow for each of the feature callbacks. The list only specifies the callbacks that have associated with them an Error Code.",
            EditorStyles.wordWrappedLabel);
        GUILayout.Label("");
        GUILayout.Label("You can find more Handling Errors information here:", EditorStyles.wordWrappedLabel);
        if (GUILayout.Button("Spil SDK Handling Errors", GUILayout.Width(400))) {
            Application.OpenURL("http://www.spilgames.com/integration/unity/implementing-spil-sdk/handling-errors/");
        }
    }

    private void DrawAntiCheating() {
        GUILayout.Label(
            "The Spil SDK takes various measures to prevent users from cheating. If you want to know the details please have a talk with the support team. In addition there are a few things you might want to implement into your game. ",
            EditorStyles.wordWrappedLabel);
        GUILayout.Label("");
        GUILayout.Label("You can find more Anti-cheating information here:", EditorStyles.wordWrappedLabel);
        if (GUILayout.Button("Spil SDK Anti-cheating", GUILayout.Width(400))) {
            Application.OpenURL("http://www.spilgames.com/integration/unity/implementing-spil-sdk/spil-sdk-anti-cheating/");
        }
    }
}