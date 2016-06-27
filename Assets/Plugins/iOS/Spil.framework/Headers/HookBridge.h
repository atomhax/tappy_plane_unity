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
    
    void setPushNotificationKey(const char* pushKey);
    
    void handlePushNotification(const char* notificationStringParams);
    
    void registerForPushNotifications(const char* obj);
    
    // --- App flow ---
    
    void applicationDidEnterBackground();
    
    void applicationDidBecomeActive();
    
    // --- Util ---
    
    void UnitySendMessage(const char* obj, const char* method, const char* msg);
    
    char* cStringCopy(const char* string);
    
    char* getSpilUIDNative();

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

    void showIntersitialNative ();

    void playRewardVideoNative ();
    
    void devRequestAdNative(const char* providerName, const char* adTypeName, const bool parentalGate);
    
    void devShowRewardVideoNative(const char* providerName);
    
    void devShowInterstitialNative(const char* providerName);
    
    void devShowMoreAppsNative(const char* providerName);

    void showToastOnVideoReward(const bool enabled);
    
    // --- Game & Player data ---
    
    //char* getUserProfileNative ();

    char* getWalletNative ();
    
    char* getSpilGameDataNative ();
    
    char* getInventoryNative ();
    
    void addCurrencyToWallet(int currencyId, int amount, char* reasonName);
    
    void subtractCurrencyFromWallet(int currencyId, int amount, char* reasonName);
    
    void addItemToInventory(int itemId, int amount, char* reasonName);
    
    void subtractItemFromInventory(int itemId, int amount, char* reasonName);
    
    void consumeBundle(int itemId, char* reasonName);
}

#endif

+ (void) sendMessage:(NSString*)messageName toObject:(NSString*)objectName withParameter:(NSString*)parameterString;

@end