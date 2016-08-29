//
//  HookBridge.h
//  SpilStaticLib
//
//  Created by Martijn van der Gun on 6/22/15.
//  Copyright (c) 2015 Martijn van der Gun. All rights reserved.
//


#import <UIKit/UIKit.h>

@interface HookBridge : NSObject {
    
}

#ifdef __cplusplus
extern "C" {

    // --- Event tracking ---
    
    void initEventTracker();
    
    void initEventTrackerWithOptions(const char* options);
    
    void trackEventNative(const char* eventName);
    
    void trackEventWithParamsNative(const char* eventName, const char* jsonStringParams);
    
    // --- Push messages ---
    
    void disableAutomaticRegisterForPushNotificationsNative();
    
    void registerForPushNotifications();
    
    void setPushNotificationKey(const char* pushKey);
    
    void handlePushNotification(const char* notificationStringParams);
    
    // --- App flow ---
    
    void applicationDidFinishLaunchingWithOptions(const char* launchOptions);
    
    void applicationDidEnterBackground();
    
    void applicationDidBecomeActive();
    
    // --- Util ---
    
    void UnitySendMessage(const char* obj, const char* method, const char* msg);
    
    char* cStringCopy(const char* string);
    
    char* getSpilUserIdNative();
    
    void setPluginInformationNative(const char* pluginName, const char* pluginVersion);

    // --- Config ---
    
    char* getConfigNative ();
    
    char* getConfigValueNative (const char* keyName);
    
    // --- Packages & Promotions ---
    
    void requestPackagesNative ();
    
    char* getPackageNative (const char* keyName);
    
    char* getAllPackagesNative ();
    
    char* getPromotionNative (const char* keyName);

    // --- ADS ---
    
    void showMoreAppsNative ();

    void playRewardVideoNative ();
    
    void devRequestAdNative(const char* providerName, const char* adTypeName, const bool parentalGate);
    
    void devShowRewardVideoNative(const char* providerName);
    
    void devShowInterstitialNative(const char* providerName);
    
    void devShowMoreAppsNative(const char* providerName);

    void showToastOnVideoReward(const bool enabled);
    
    // --- Game & Player data ---

    char* getWalletNative ();
    
    char* getSpilGameDataNative ();
    
    char* getInventoryNative ();
    
    void addCurrencyToWalletNative (int currencyId, int amount, char* reasonName);
    
    void subtractCurrencyFromWalletNative(int currencyId, int amount, char* reasonName);
    
    void addItemToInventoryNative (int itemId, int amount, char* reasonName);
    
    void subtractItemFromInventoryNative (int itemId, int amount, char* reasonName);
    
    void consumeBundleNative (int itemId, char* reasonName);
    
    // --- Customer support ---
    
    void showHelpCenterNative();
    
    void showContactCenterNative();
    
    void showHelpCenterWebviewNative();
    
    // --- User data ---
    
    char* getUserIdNative();
    
    char* getUserProviderNative();
    
    void setUserIdNative(const char* providerId, const char* userId);

    void setPrivateGameStateNative(const char* privateData);

    char* getPrivateGameStateNative();

    void setPublicGameStateNative(const char* publicData);
    
    char* getPublicGameStateNative();

    void getOtherUsersGameStateNative(const char* provider, const char* userIdsJsonArray);
}

#endif

+ (void) sendMessage:(NSString*)messageName toObject:(NSString*)objectName withParameter:(NSString*)parameterString;

@end