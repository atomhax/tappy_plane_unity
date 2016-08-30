//
//  Spil.h
//  Spil
//
//  Created by Martijn van der Gun on 10/1/15.
//  Copyright © 2015 Spil Games. All rights reserved.
//

#import "JsonUtil.h"
#import "HookBridge.h"
#import "GAI.h"

#define SDK_VERSION @"2.0.8"

@class Spil;
@class UserProfile;

@protocol SpilDelegate

@optional

/**
 * Ad events, params:
 * type = rewardVideo|interstitial|moreApps
 * reason = error|dismiss|reward
 * network = ChartBoost|Fyber|DFP
 * reward = rewardData(Fyber:Integer,Chartboost:Json{reward:"",currencyName:"",currencyId:""})|nil
 */
-(void)adAvailable:(NSString*)type; // An ad is available
-(void)adNotAvailable:(NSString*)type; // An ad is unavailable or did fail to load
-(void)adStart; // An ad has started
-(void)adFinished:(NSString*)type reason:(NSString*)reason reward:(NSString*)reward network:(NSString*)network; // An ad has finished (dismissed or an reward was granted)

// Notification events
-(void)grantReward:(NSDictionary*)data;

// Config events
-(void)configUpdated;

// Package events
-(void)packagesLoaded;

// Game data events
-(void)spilGameDataAvailable;
-(void)spilGameDataError:(NSString*)message;

// Player data events
-(void)playerDataAvailable;
-(void)playerDataError:(NSString*)message;
-(void)playerDataUpdated:(NSString*)reason updatedData:(NSString*)updatedData;

// User data events
-(void)gameStateUpdated:(NSString*)access; // Access: private|public
-(void)otherUsersGameStateLoaded:(NSDictionary*)data forProvider:(NSString*)provider; // Data: <NSString* userId, NSString* data>
-(void)gameStateError:(NSString*)message;

// Automated events
-(void)openGameShop;

@end

@interface Spil : NSObject {
    
}

// Define delegate property
@property (nonatomic, assign) id  delegate;

+(Spil*)sharedInstance;

#pragma mark General

/**
 * Initiates the API
 */
+(void)start;

/**
 *  Initiates the API with options
 *
 *  @param options holds a dictionary with options like "isUnity"
 */
+(void)startWithOptions:(NSDictionary*)options;

/**
 *  Show advanced debug logs
 *
 *  @param advancedLoggingEnabled Enables or disables the advanced log printing
 */
+(void)setAdvancedLoggingEnabled:(BOOL)advancedLoggingEnabled;

/**
 *  Get the Spil user id
 *
 */
+(NSString*)getSpilUserId;

/**
 *  Set a plugin name and version for the current session.
 *
 *  @param pluginName The plugin name
 *  @param pluginVersion The plugin version
 */
+(void)setPluginInformation:(NSString*)pluginName pluginVersion:(NSString*)pluginVersion;

#pragma mark App flow

/**
 * Forwarding Delegate method to let the Spil framework know when the app was launched
 *
 *  @param application Delegate application to be passed
 *  @param launchOptions Dictionary with launch options
 */
+(void)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions;

/**
 *  Forwarding Delegate method to let the Spil framework know when the app went to the background
 *
 *  @param application Delegate application to be passed
 */
+(void)applicationDidEnterBackground:(UIApplication *)application;

/**
 *  Forwarding Delegate method to let the Spil framework know when the app became active again after running in background
 *
 *  @param application Delegate application to be passed
 */
+(void)applicationDidBecomeActive:(UIApplication *)application;

/**
 *  Handle remote notification packages
 *
 *  @param Application     Reference to the UIApplication object
 *  @param userInfo        Reference to the push notification payload
 */
+(void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo;

#pragma mark Event tracking

/**
 *  Track a basic named event
 *
 *  @param name The name of the event. Replace spaces with an underscore
 */
+(void) trackEvent:(NSString*)name;

/**
 *  Track a named events with a key / value object
 *
 *  @param name The name of the event. Replace spaces with an underscore
 *  @param params A key value dictionary holding the params
 */
+(void) trackEvent:(NSString*)name withParameters:(NSDictionary *)params;

/**
 *  Track a basic named event with a response
 *
 *  @param name  The name of the event. Replace spaces with an underscore
 *  @param block A block with response param that will be executed when the server sends a reponse on the tracked event
 */
+(void) trackEvent:(NSString*)name onResponse:(void (^)(id response))block;

/**
 *  Track a named event params and a response
 *
 *  @param name   The name of the event. Replace spaces with an underscore
 *  @param params A key value dictionary holding the params
 *  @param block  A block with response param that will be executed when the server sends a reponse on the tracked event
 */
+(void) trackEvent:(NSString*)name withParameters:(NSDictionary *)params onResponse:(void (^)(id response))block;

#pragma mark Send message

/**
 *  Unity message sender
 *
 *  @param messageName     Name of the message, should match the function in unity
 *  @param objectName      The name of the spil object where the script is attached to. In most cases "SpilSDK"
 *  @param data            An object which can be serialized to json
 */
+(void)sendMessage:(NSString*)messageName toObject:(NSString*)objectName withData:(id)data;

/**
 *  Unity message sender
 *
 *  @param messageName     Name of the message, should match the function in unity
 *  @param objectName      The name of the spil object where the script is attached to. In most cases "SpilSDK"
 *  @param parameterString A json string holding the data to send
 */
+(void)sendMessage:(NSString*)messageName toObject:(NSString*)objectName withString:(NSString*)parameterString;

#pragma mark Push notifications

/**
 *  Disable the automated registration message for push notifications
 *  Should be called before [Spil start]
 *
 */
+(void)disableAutomaticRegisterForPushNotifications;

/**
 *  Helper function to register for push notifications
 */
+(void)registerPushNotifications;

/**
 *  Helper function to forward the app delegate listener on the deviceToken
 */
+(void)didRegisterForRemoteNotificationsWithDeviceToken:(NSData*)deviceToken;

#pragma mark Config

/**
 * Get the latest stored game configuration, typically a synchronized json object coming from the server.
 *
 * @return NSDictionary object representation from the stored game configuration
 */
+(NSDictionary*)getConfig;

/**
 * Get a specific value from a particular key from the game configuration
 * 
 * @param Name of the key. Type must be NSString.
 * @return returns the object from a key, only first hiergy
 */
+(id)getConfigValue:(NSString*)keyString;

#pragma mark Packages

/**
 * Get the latest stored store packages.
 *
 * @return NSArray object representation from the stored store packages
 */
+(NSArray*)getAllPackages;

/**
 * Get a specific package from the store
 *
 * @param Name of the key. Type must be NSString.
 * @return returns the store package, or nil if not found
 */
+(NSDictionary*)getPackageByID:(NSString*)keyString;

/**
 * Get the latest stored store promotions.
 *
 * @return NSArray object representation from the stored store promotions
 */
+(NSArray*)getAllPromotions;

/**
 * Get a specific promotion from the store
 *
 * @param Name of the key. Type must be NSString.
 * @return returns the store promotion, or nil if not found
 */
+(NSDictionary*)getPromotionByID:(NSString*)keyString;

/**
 * Refresh the package and promotion data
 */
+(void)requestPackages;

#pragma mark Ads

/**
 * Show the more apps screen
 */
+(void)showMoreApps;

/**
 * Show the last requested reward video
 */
+(void)playRewardVideo;

/**
 * Helper method to determine if the ad provider is initialized
 */
+(BOOL)isAdProviderInitialized:(NSString*)identifier;

/**
 *  Show a toast when a reward is unlocked
 */
+(void)showToastOnVideoReward:(BOOL)enabled;

#pragma mark UserData & GameData

/**
 * Request the player data
 */
+(void)requestPlayerData;

/**
 * Request the game data
 */
+(void)requestGameData;

/**
 * Returns the entire user profile as json
 */
+(NSString*)getUserProfile;

/**
 * Returns the wallet data as json
 */
+(NSString*)getWallet;

/**
 * Returns the player data as json
 */
+(NSString*)getSpilGameData;

/**
 * Returns the inventory data as json
 */
+(NSString*)getInventory;

/**
 * Returns the shop data as json
 */
+(NSString*)getShop;

/**
 * Returns the shop promotions data as json
 */
+(NSString*)getShopPromotions;

/**
 * Add currency to the wallet
 * @param currencyId    Id of the currency
 * @param amount        Amount to add
 * @param reason        The add reason
 */
+(void)addCurrencyToWallet:(int)currencyId withAmount:(int)amount withReason:(NSString*)reason;

/**
 * Subtract currency from the wallet
 * @param currencyId    Id of the currency
 * @param amount        Amount to subtract
 * @param reason        The subtract reason
 */
+(void)subtractCurrencyFromWallet:(int)currencyId withAmount:(int)amount withReason:(NSString*)reason;

/**
 * Add item to the inventory
 * @param itemId        Id of the item
 * @param amount        Amount to add
 * @param reason        The add reason
 */
+(void)addItemToInventory:(int)itemId withAmount:(int)amount withReason:(NSString*)reason;

/**
 * Subtract item to from the inventory
 * @param itemId        Id of the item
 * @param amount        Amount to subtract
 * @param reason        The subtract reason
 */
+(void)subtractItemFromInventory:(int)itemId withAmount:(int)amount withReason:(NSString*)reason;

/**
 * Uses the bundle and will add the items to the inventory and subtract the currency from the wallet
 * @param bundleId      Id of the bundle
 * @param reason        The bundle reason
 */
+(void)consumeBundle:(int)bundleId withReason:(NSString*)reason;

#pragma mark Customer support

/**
 * Shows the help center
 */
+(void)showHelpCenter;

/**
 * Shows the contact center
 */
+(void)showContactCenter;

/**
 * Shows the help center webview version
 */
+(void)showHelpCenterWebview;

#pragma mark User data

/**
 * Get the custom user id
 */
+(NSString*)getUserId;

/**
 * Get the custom provider id
 */
+(NSString*)getUserProvider;

/**
 *  Set a custom user id for a specified service.
 *
 *  @param userId The social user id to use
 *  @param providerId The id of the service (e.g. facebook)
 */
+(void)setUserId:(NSString*)userId forProviderId:(NSString*)providerId;

/**
 *  Set private game state data.
 *
 *  @param privateData The private data to store
 */
+(void)setPrivateGameState:(NSString*)privateData;

/**
 *  Get private game state data.
 *
 */
+(NSString*)getPrivateGameState;

/**
 *  Set public game state data.
 *
 *  @param publicData The public data to store
 */
+(void)setPublicGameState:(NSString*)publicData;

/**
 *  Get public game state data.
 */
+(NSString*)getPublicGameState;

/**
 *  Get the public game state data of other users, 
 *  based on the user id of a custom provider.
 *
 *  @param provider The provider to request the data from
 *  @param userIds The user ids
 */
+(void)getOtherUsersGameState:(NSString*)provider userIds:(NSArray*)userIds;

#pragma test methods (dev)

/**
 * NOTE: Those methods are exposed just for ad testing, they should not be referenced in the final implementation, params:
 * adProvider: DFP|Fyber|ChartBoost
 * adType: interstitial|rewardVideo|moreApps
 * parentalGate: not implemented yet (always false)
 */

+(void)devRequestAd:(NSString*)provider withAdType:(NSString*)adType withParentalGate:(BOOL)parentalGate;
+(void)devShowRewardVideo:(NSString*)adProvider;
+(void)devShowInterstitial:(NSString*)adProvider;
+(void)devShowMoreApps:(NSString*)adProvider;

@end