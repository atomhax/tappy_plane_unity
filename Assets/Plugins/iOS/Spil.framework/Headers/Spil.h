//
//  Spil.h
//  Spil
//
//  Copyright © 2015 Spil Games. All rights reserved.
//

#import "JsonUtil.h"
#import "HookBridge.h"
#import "GAI.h"

#define SPIL_SDK_VERSION @"2.7.2"

@class ImageContext;
@class Spil;
@class UserProfile;
@class SpilEventTracker;
@class Bundle;
@class PlayerCurrency;
@class PlayerItem;
@class Wallet;
@class Inventory;

@protocol SpilDelegate

@optional

/**
 * Ad events, params:
 * type = rewardVideo|interstitial|moreApps
 * reason = error|dismiss|reward
 * network = ChartBoost|Fyber|DFP
 * reward = rewardData(Fyber:Integer,Chartboost:Json{reward:"",currencyName:"",currencyId:""})|nil
 */
-(void)adAvailable:(nonnull NSString*)type; // An ad is available
-(void)adNotAvailable:(nonnull NSString*)type; // An ad is unavailable or did fail to load
-(void)adStart; // An ad has started
-(void)adFinished:(nonnull NSString*)type reason:(nonnull NSString*)reason reward:(nonnull NSString*)reward network:(nonnull NSString*)network; // An ad has finished (dismissed or an reward was granted)
-(void)openParentalGate; // The ad requires a parental gate check to continue, present the parental gate in this method and call the closedParentalGate method to pass the result back to the Spil SDK.

// Notification events
-(void)grantReward:(nonnull NSDictionary*)data;

// Splash screen events
-(void)splashScreenOpen;
-(void)splashScreenNotAvailable;
-(void)splashScreenClosed;
-(void)splashScreenOpenShop;
-(void)splashScreenError:(nonnull NSString*)message;
-(void)splashScreenData:(nonnull NSString*)payload;

// Daily bonus screen events
-(void)dailyBonusOpen;
-(void)dailyBonusNotAvailable;
-(void)dailyBonusClosed;
-(void)dailyBonusReward:(nonnull NSDictionary*)data;
-(void)dailyBonusError:(nonnull NSString*)message;

// Webview events
-(void)iapRequestPurchase:(nonnull NSString*)skuId;

// Config events
-(void)configUpdated;
-(void)configError:(nonnull NSString*)error;

// Package events
-(void)packagesLoaded;

// Game data events
-(void)spilGameDataAvailable;
-(void)spilGameDataError:(nonnull NSString*)message;

// Player data events
-(void)playerDataUpdated:(nonnull NSString*)reason updatedData:(nonnull NSString*)updatedData;
-(void)playerDataEmptyGacha;

// User data events
-(void)gameStateUpdated:(nonnull NSString*)access; // Access: private|public
-(void)otherUsersGameStateLoaded:(nonnull NSDictionary*)data forProvider:(nonnull NSString*)provider; // Data: <NSString* userId, NSString* data>

// Image cache
-(void)imageLoadSuccess:(nonnull NSString*)localPath imageContext:(nonnull ImageContext*)imageContext;
-(void)imageLoadFailed:(nonnull ImageContext*)imageContext withError:(nonnull NSString*)error;
-(void)imagePreloadingCompleted;

// IAP validation
-(void)iapValid:(nonnull NSArray*)items;
-(void)iapInvalid:(nonnull NSString*)message;

// Token claiming
-(void)rewardTokenReceived:(nonnull NSString*)token rewardData:(nonnull NSArray*)rewardJsonObject withRewardType:(nonnull NSString*)rewardType;
-(void)rewardTokenClaimed:(nonnull NSString*)rewardType reward:(nonnull NSArray*)reward;
-(void)rewardTokenClaimFailed:(nonnull NSString*)rewardType error:(nonnull NSString*)error;

// Server time
-(void)serverTimeRequestSuccess:(nonnull NSString*)unixTimestamp;
-(void)serverTimeRequestFailed:(nonnull NSString*)error;

// Live events
-(void)liveEventStageOpen;
-(void)liveEventStageClosed;
-(void)liveEventAvailable;
-(void)liveEventNotAvailable;
-(void)liveEventError:(nonnull NSString*)error;
-(void)liveEventMetRequirements:(BOOL)metRequirements;
-(void)liveEventUsedExternalItems:(nonnull NSArray*)externalItem;
-(void)liveEventReward:(nonnull NSArray*)rewardList;
-(void)liveEventCompleted;

// Login
-(void)onLoginSuccessful:(BOOL)resetData withSocialProvider:(nullable NSString*)socialProvider withSocialId:(nullable NSString*)socialId isGuest:(BOOL)isGuest;
-(void)onLoginFailed:(nonnull NSString*)error;
-(void)onLogoutSuccessful;
-(void)onLogoutFailed:(nonnull NSString*)error;
-(void)onAuthError:(nonnull NSString*)error;
-(void)onRequestLogin;

// Userdata syncing
-(void)userDataAvailable;
-(void)userDataError:(nonnull NSString*)error;
-(void)userDataMergeConflict:(nonnull NSString*)localData remoteData:(nonnull NSString*)remoteData;
-(void)userDataMergeSuccessful;
-(void)userDataMergeFailed:(nonnull NSString*)mergeData mergeType:(nonnull NSString*)mergeType;
-(void)userDataHandleMerge:(nonnull NSString*)mergeType; // Called when a merge conflict option button was pressed
-(void)userDataSyncError;
-(void)userDatalockError;

@end

@interface Spil : NSObject {
    
}

// Define delegate property
@property (nonatomic, assign, nullable) id  delegate;

+(nonnull Spil*)sharedInstance;

#pragma mark General

/**
 * Initiates the SDK
 * @param Optional, options holds a dictionary with options like "isUnity"
 */
+(void)startWithOptions:(nullable NSDictionary*)options;

/**
 * Helper method to log a message to the console
 * Especially useful when building a wrapped games (e.g. Unity) where the log messages are sometimes stripped out.
 * This method gives the oppertunity to log the message at the native layer instead.
 * @param The message to log
 */
+(void)log:(nonnull NSString*)message;

/**
 * Method to set a custom bundle id, useful during debugging.
 * @param The custom bundle id to use
 */
+(void)setCustomBundleId:(nonnull NSString*)bundleId;

/**
 * Get the Spil user id
 * @returns The Spil user id
 */
+(nullable NSString*)getSpilUserId;

/**
 * Get the registered push notification token
 * @returns The registered push notification token
 */
+(nullable NSString*)getPushToken;

/**
 * Set a plugin name and version for the current session.
 * @param pluginName The plugin name
 * @param pluginVersion The plugin version
 */
+(void)setPluginInformation:(nonnull NSString*)pluginName pluginVersion:(nonnull NSString*)pluginVersion;

/**
 * Request the server timestamp
 */
+(void)requestServerTime;

#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wnullability-completeness"

/**
 * Shows a simple dialog, useful while debugging
 */
+(void)showDialog:(NSString*)title withMessage:(NSString*)message withOkButton:(NSString*)okButtonText handler:(void (^ __nullable)(UIAlertAction *action))handler;

#pragma clang diagnostic pop

#pragma mark App flow

/**
 * Forwarding Delegate method to let the Spil framework know when the app was launched
 * @param application Delegate application to be passed
 * @param launchOptions Dictionary with launch options
 */
+(void)application:(nullable UIApplication *)application didFinishLaunchingWithOptions:(nullable NSDictionary *)launchOptions;

/**
 * Forwarding Delegate method to let the Spil framework know when the app went to the background
 * @param application Delegate application to be passed
 */
+(void)applicationDidEnterBackground:(nullable UIApplication *)application;

/**
 * Forwarding Delegate method to let the Spil framework know when the app became active again after running in background
 * @param application Delegate application to be passed
 */
+(void)applicationDidBecomeActive:(nullable UIApplication *)application;

/**
 * Handle remote notification packages
 * @param Application     Reference to the UIApplication object
 * @param userInfo        Reference to the push notification payload
 */
+(void)application:(nullable UIApplication *)application didReceiveRemoteNotification:(nullable NSDictionary *)userInfo;

/**
 * Forwarding Delegate method to let the Spil framework handle deeplinks
 * @param Application          Reference to the UIApplication object
 * @param openURL              The deeplink url
 * @param sourceApplication    The app name which triggered the deeplink
 * @param annotation           The anotation of the deeplink
 */
+(BOOL)application:(nullable UIApplication *)application openURL:(nullable NSURL *)url sourceApplication:(nullable NSString *)sourceApplication annotation:(nullable id)annotation;

#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wnullability-completeness"

/**
 * Forwarding Delegate method to let the Spil framework handle deeplinks
 * @param Application          Reference to the UIApplication object
 * @param continueUserActivity The user activity object
 * @param restorationHandler   The restoration handler
 */
+(BOOL)application:(nullable UIApplication *)application continueUserActivity:(nullable NSUserActivity *)userActivity restorationHandler:(void (^)(NSArray *))restorationHandler;

#pragma clang diagnostic pop

#pragma mark Event tracking

/**
 * Track a milstone achieved event
 * @param name          The name of the milestone
 */
+(void)trackMilestoneAchievedEvent:(nonnull NSString*)name;

/**
 * Track a level start
 * @param level         The name of the level
 * @param customCreated Indictating if the level was custom created
 * @param creatorId     The id of the creator of the level
 */
+(void)trackLevelStartEvent:(nonnull NSString*)level customCreated:(bool)customCreated creatorId:(nonnull NSString*)creatorId;

/**
 * Track a level complete
 * @param level         The name of the level
 * @param score         The final score the player achieves at the end of the level
 * @param stars         The # of stars (or any other rating system) the player achieves at the end of the level
 * @param turns         The # of moves/turns taken to complete the level
 * @param customCreated Indictating if the level was custom created
 * @param creatorId     The id of the creator of the level
 */
+(void)trackLevelCompleteEvent:(nonnull NSString*)level score:(double)score stars:(int)stars turns:(int)turns customCreated:(bool)customCreated creatorId:(nonnull NSString*)creatorId;

/**
 * Track a level fail
 * @param level         The name of the level
 * @param score         The final score the player achieves at the end of the level
 * @param stars         The # of stars (or any other rating system) the player achieves at the end of the level
 * @param turns         The # of moves/turns taken to complete the level
 * @param customCreated Indictating if the level was custom created
 * @param creatorId     The id of the creator of the level
 */
+(void)trackLevelFailedEvent:(nonnull NSString*)level score:(double)score stars:(int)stars turns:(int)turns customCreated:(bool)customCreated creatorId:(nonnull NSString*)creatorId;

/**
 * Track a level up
 * @param level         The name of the level
 * @param score         The final score the player achieves at the end of the level
 * @param objectId      The level up object identifier
 * @param skillId       The skill id
 */
+(void)trackLevelUpEvent:(nonnull NSString*)level objectId:(nonnull NSString*)objectId skillId:(nonnull NSString*)skillId;

/**
 * Track an item equip
 * @param equippedItem  The id of the equipped item
 * @param equippedTo    The id of were the item will be equipped to
 */
+(void)trackEquipEvent:(nonnull NSString*)equippedItem equippedTo:(nonnull NSString*)equippedTo;

/**
 * Track an item upgrade
 * @param upgradeId     The id of the upgraded item
 * @param level         The level in which the upgrade happened
 * @param reason        The upgrade reason
 * @param iteration     The upgrade iteration
 */
+(void)trackUpgradeEvent:(nonnull NSString*)upgradeId level:(nonnull NSString*)level reason:(nonnull NSString*)reason iteration:(int)iteration;

/**
 * Track a level create event
 * @param levelId       The id of the created level
 * @param creatorId     The id of the level creator
 */
+(void)trackLevelCreateEvent:(nonnull NSString*)levelId creatorId:(nonnull NSString*)creatorId;

/**
 * Track a download event
 * @param levelId       The id of the downloaded level
 * @param creatorId     The id of the level creator
 * @param rating        The level rating
 */
+(void)trackLevelDownloadEvent:(nonnull NSString*)levelId creatorId:(nonnull NSString*)creatorId rating:(int)rating;

/**
 * Track a level rate event
 * @param levelId       The id of the rated level
 * @param creatorId     The id of the level creator
 * @param rating        The level rating
 */
+(void)trackLevelRateEvent:(nonnull NSString*)levelId creatorId:(nonnull NSString*)creatorId rating:(int)rating;

/**
 * Track the start of an endless level
 */
+(void)trackEndlessModeStartEvent;

/**
 * Track the end of an endless mode level
 * @param level         The distance completed in the endless level
 */
+(void)trackEndlessModeEndEvent:(int)distance;

/**
 * Track a player dies event
 * @param level         The name of the level in which the player dies
 */
+(void)trackPlayerDiesEvent:(nonnull NSString*)level;

/**
 * Track a wallet/inventory update
 * @param reason        The reason for which the wallet or the inventory has been updated
 *                      A list of default resons can be found here: {@link com.spilgames.spilsdk.playerdata.PlayerDataUpdateReasons}
 * @param location      The location where the event occurred (ex.: Shop Screen, End of the level Screen)
 * @param currencyList  A list containing the currency objects that have been changed with the event.
 *                      {@link com.spilgames.spilsdk.models.tracking.TrackingCurrency}
 * @param itemsList     A list containing the item objects that have been changed with the event.
 *                      {@link com.spilgames.spilsdk.models.tracking.TrackingItem}
 */
+(void)trackWalletInventoryEvent:(nonnull NSString*)reason withReasonDetails:(nullable NSString*)reasonDetails location:(nullable NSString*)location currencyList:(nullable NSString*)currencyList itemList:(nullable NSString*)itemsList;

/**
 * Track a successful iap
 * @param skuId             The product identifier of the item that was purchased
 * @param transactionId     The transaction identifier of the item that was purchased (also called orderId)
 * @param purchaseDate      The date and time that the item was purchased
 */
+(void)trackIAPPurchasedEvent:(nonnull NSString*)skuId transactionId:(nonnull NSString*)transactionId purchaseDate:(nonnull NSString*)purchaseDate;

/**
 * Track a restored iap
 * @param skuId                 The product identifier of the item that was purchased
 * @param originalTransactionId For a transaction that restores a previous transaction, the transaction identifier of the original transaction.
 *                              Otherwise, identical to the transaction identifier
 * @param originalPurchaseDate  For a transaction that restores a previous transaction, the date of the original transaction
 */
+(void)trackIAPRestoredEvent:(nonnull NSString*)skuId originalTransactionId:(nonnull NSString*)originalTransactionId originalPurchaseDate:(nonnull NSString*)originalPurchaseDate;

/**
 * Track a failed iap
 * @param skuId     The product identifier of the item that was purchased
 * @param error     Error description or error code
 */
+(void)trackIAPFailedEvent:(nonnull NSString*)skuId error:(nonnull NSString*)error;

/**
 * Track the completion of a tutorial
 */
+(void)trackTutorialCompleteEvent;

/**
 * Track a skipped tutorial
 * Track the skipping of a tutorial
 */
+(void)trackTutorialSkippedEvent;

/**
 * Track a register
 * @param platform      The platform for which the registration occurred (ex.: Facebook)
 */
+(void)trackRegisterEvent:(nonnull NSString*)platform;

/**
 * Track a share
 * @param platform      The platform for which the share occurred (ex.: Facebook)
 */
+(void)trackShareEvent:(nonnull NSString*)platform;

/**
 * Track an invite
 * @param platform      The platform for which the invite occurred (ex.: Facebook)
 */
+(void)trackInviteEvent:(nonnull NSString*)platform;

/**
 * Track a basic named event
 * @param name         The name of the event. Replace spaces with an underscore
 */
+(void) trackEvent:(nonnull NSString*)name;

/**
 * Track a named events with a key / value object
 * @param name The name of the event. Replace spaces with an underscore
 * @param params A key value dictionary holding the params
 */
+(void) trackEvent:(nonnull NSString*)name withParameters:(nullable NSDictionary *)params;

#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wnullability-completeness"

/**
 * Track a basic named event with a response
 * @param name  The name of the event. Replace spaces with an underscore
 * @param block A block with response param that will be executed when the server sends a reponse on the tracked event
 */
+(void) trackEvent:(nonnull NSString*)name onResponse:(void (^)(id response))block;

/**
 * Track a named event params and a response
 * @param name   The name of the event. Replace spaces with an underscore
 * @param params A key value dictionary holding the params
 * @param block  A block with response param that will be executed when the server sends a reponse on the tracked event
 */
+(void) trackEvent:(nonnull NSString*)name withParameters:(nonnull NSDictionary *)params onResponse:(void (^)(id response))block;

/**
 * Track an error event
 * @param type    The error type (e.g. "adjust")
 * @param action  The error action (e.g. "intall")
 * @param message The error message (e.g. "no connection")
 */
+(void) trackErrorWithType:(NSString*)type withAction:(NSString*)action withMessage:(NSString*)message;

#pragma clang diagnostic pop

#pragma mark Send message

/**
 * Unity message sender
 * @param messageName     Name of the message, should match the function in unity
 * @param objectName      The name of the spil object where the script is attached to. In most cases "SpilSDK"
 * @param data            An object which can be serialized to json
 */
+(void)sendMessage:(nonnull NSString*)messageName toObject:(nonnull NSString*)objectName withData:(nonnull id)data;

/**
 * Unity message sender
 * @param messageName     Name of the message, should match the function in unity
 * @param objectName      The name of the spil object where the script is attached to. In most cases "SpilSDK"
 * @param parameterString A json string holding the data to send
 */
+(void)sendMessage:(nonnull NSString*)messageName toObject:(nonnull NSString*)objectName withString:(nonnull NSString*)parameterString;

#pragma mark Push notifications

/**
 * Disable the automated registration message for push notifications
 * Should be called before [Spil start]
 */
+(void)disableAutomaticRegisterForPushNotifications;

/**
 * Helper function to register for push notifications
 */
+(void)registerPushNotifications;

/**
 * Helper function to forward the app delegate listener on the deviceToken
 */
+(void)didRegisterForRemoteNotificationsWithDeviceToken:(nonnull NSData*)deviceToken;

#pragma mark Token claiming

/**
 * Used to claim the reward token from the backend
 * @param token      The token to claim
 * @param rewardType The reward type to which the token belongs
 */
+(void)claimToken:(nonnull NSString*)token withRewardType:(nonnull NSString*)rewardType;

#pragma mark Config

/**
 * Request the game config
 */
+(void)requestGameConfig;

/**
 * Get the latest stored game configuration, typically a synchronized json object coming from the server.
 * @return NSDictionary object representation from the stored game configuration
 */
+(nullable NSDictionary*)getConfig;

/**
 * Get a specific value from a particular key from the game configuration
 * @param Name of the key. Type must be NSString.
 * @return returns the object from a key, only first hiergy
 */
+(nullable id)getConfigValue:(nonnull NSString*)keyString;

#pragma mark Packages

/**
 * Get the latest stored store packages.
 * @return NSArray object representation from the stored store packages
 */
+(nullable NSArray*)getAllPackages;

/**
 * Get a specific package from the store
 * @param Name of the key. Type must be NSString.
 * @return returns the store package, or nil if not found
 */
+(nullable NSDictionary*)getPackageByID:(nonnull NSString*)keyString;

/**
 * Get the latest stored store promotions.
 * @return NSArray object representation from the stored store promotions
 */
+(nullable NSArray*)getAllPromotions;

/**
 * Get all promotion for a package
 * @param Name of the key. Type must be NSString.
 * @return returns a promotions list for the package id
 */
+(nullable NSArray*)getPromotionsByID:(nonnull NSString*)keyString;

/**
 * Refresh the package and promotion data
 */
+(void)requestPackages;

#pragma mark Ads

/**
 * Requests the more apps screen
 */
+(void)requestMoreApps;

/**
 * Show the more apps screen
 */
+(void)showMoreApps;

/**
 * Request a reward video
 * @param rewardType    The expected reward type (optional)
 * @param location      The location the reward video was requested from (optional)
 */
+(void)requestRewardVideo:(nullable NSString*)rewardType withLocation:(nullable NSString*)location;

/**
 * Show the last requested reward video
 * @param rewardType    The expected reward type (optional)
 * @param location      The location the reward video was requested from (optional)
 */
+(void)playRewardVideo:(nullable NSString*)rewardType withLocation:(nullable NSString*)location;

/**
 * Helper method to determine if the ad provider is initialized
 */
+(BOOL)isAdProviderInitialized:(nonnull NSString*)identifier;

/**
 * Show a toast when a reward is unlocked
 */
+(void)showToastOnVideoReward:(BOOL)enabled;

/**
 * Call to inform the SDK that the parental gate was (not) passes
 */
+(void)closedParentalGate:(BOOL)pass;

#pragma mark UserData & GameData

/**
 * Request the player data
 */
+(void)updatePlayerData;

/**
 * Request the game data
 */
+(void)requestGameData;

/**
 * Returns the entire user profile as json
 */
+(nullable NSString*)getUserProfile;

/**
 * Returns the wallet data as an object
 */
+(nullable Wallet*)getWalletData;

/**
 * Returns the wallet data as json
 */
+(nullable NSString*)getWallet;

/**
 * Returns the configured game data as json
 */
+(nullable NSString*)getSpilGameData;

/**
 * Returns the inventory data as an object
 */
+(nullable Inventory*)getInventoryData;

/**
 * Returns the inventory data as json
 */
+(nullable NSString*)getInventory;

/**
 * Returns the shop data as json
 */
+(nullable NSString*)getShop;

/**
 * Returns the shop promotions data as json
 */
+(nullable NSString*)getShopPromotions;

/**
 * Add currency to the wallet
 * @param currencyId    Id of the currency
 * @param amount        Amount to add
 * @param reason        The add reason
 * @param location      The location where the event happened, for example level1
 * @param transactionId The transaction id used
 */
+(void)addCurrencyToWallet:(int)currencyId withAmount:(int)amount withReason:(nonnull NSString*)reason withReasonDetails:(nullable NSString*)reasonDetails withLocation:(nullable NSString*)location withTransactionId:(nullable NSString*)transactionId;

/**
 * Subtract currency from the wallet
 * @param currencyId    Id of the currency
 * @param amount        Amount to subtract
 * @param reason        The subtract reason
 * @param location      The location where the event happened, for example level1
 * @param transactionId The transaction id used
 */
+(void)subtractCurrencyFromWallet:(int)currencyId withAmount:(int)amount withReason:(nonnull NSString*)reason withReasonDetails:(nullable NSString*)reasonDetails withLocation:(nullable NSString*)location withTransactionId:(nullable NSString*)transactionId;

/**
 * Add item to the inventory
 * @param itemId        Id of the item
 * @param amount        Amount to add
 * @param reason        The add reason
 * @param location      The location where the event happened, for example level1
 * @param transactionId The transaction id used
 */
+(void)addItemToInventory:(int)itemId withAmount:(int)amount withReason:(nonnull NSString*)reason withReasonDetails:(nullable NSString*)reasonDetails withLocation:(nullable NSString*)location withTransactionId:(nullable NSString*)transactionId;

/**
 * Subtract item to from the inventory
 * @param itemId        Id of the item
 * @param amount        Amount to subtract
 * @param reason        The subtract reason
 * @param location      The location where the event happened, for example level1
 * @param transactionId The transaction id used
 */
+(void)subtractItemFromInventory:(int)itemId withAmount:(int)amount withReason:(nonnull NSString*)reason withReasonDetails:(nullable NSString*)reasonDetails withLocation:(nullable NSString*)location withTransactionId:(nullable NSString*)transactionId;

/**
 * Uses the bundle and will add the items to the inventory and subtract the currency from the wallet
 * @param bundleId      Id of the bundle
 * @param reason        The bundle reason
 * @param location      The location where the event happened, for example level1
 * @param transactionId The transaction id used
 */
+(void)buyBundle:(int)bundleId withReason:(nonnull NSString*)reason withReasonDetails:(nullable NSString*)reasonDetails withLocation:(nullable NSString*)location withTransactionId:(nullable NSString*)transactionId;

/**
 * Open the gacha and add the content to the inventory
 * @param itemId        The id of the gacha
 * @param reason        The bundle reason
 * @param reasonDetails The bundle reasonDetails
 * @param location      The location where the event happened, for example level1
 */
+(void)openGacha:(int)itemId withReason:(nonnull NSString*)reason withReasonDetails:(nullable NSString*)reasonDetails withLocation:(nullable NSString*)location;

/**
 * Get all the shop tabs
 * Returns an array with ShopTab objects
 */
+(nonnull NSMutableArray*)getShopTabs;

/**
 * Get a bundle by bundle id
 * @param bundleId      Id of the bundle
 * Returns a bundle matching the provided bundle id
 */
+(nullable Bundle*)getBundle:(int)bundleId;

/**
 * Get an item by id
 * @param itemId        Id of the item
 * Returns an item matching the provided item id
 */
+(nullable PlayerItem*)getItem:(int)itemId;

/**
 * Get a currency by the provided currency id
 * @param currencyId    Id of the currency
 * Returns a currency matching the provided currency id
 */
+(nullable PlayerCurrency*)getCurrency:(int)currencyId;

/**
 * Resets all the player data
 */
+(void)resetPlayerData;

/**
 * Resets the inventory data
 */
+(void)resetInventory;

/**
 * Resets the wallet data
 */
+(void)resetWallet;

#pragma mark Customer support

/**
 * Shows the help center webview version
 * @param url   The url to open
 */
+(void)showHelpCenterWebview:(nonnull NSString*)url;

#pragma mark Web

/**
 * Request the daily bonus screen
 */
+(void)requestDailyBonus;

/**
 * Request a splash screen
 * @param type   The type of splashscreen to request
 */
+(void)requestSplashScreen:(nullable NSString*)type;

#pragma mark User data

/**
 * Get the custom user id
 */
+(nullable NSString*)getExternalUserId;

/**
 * Get the custom provider id
 */
+(nullable NSString*)getExternalUserProvider;

/**
 * Set a custom user id for a specified service.
 * @param userId The social user id to use
 * @param providerId The id of the service (e.g. facebook)
 */
+(void)setExternalUserId:(nonnull NSString*)userId forProviderId:(nonnull NSString*)providerId;

/**
 * Set private game state data.
 * @param privateData The private data to store
 */
+(void)setPrivateGameState:(nonnull NSString*)privateData;

/**
 * Get private game state data.
 */
+(nullable NSString*)getPrivateGameState;

/**
 * Set public game state data.
 * @param publicData The public data to store
 */
+(void)setPublicGameState:(nonnull NSString*)publicData;

/**
 * Get public game state data.
 */
+(nullable NSString*)getPublicGameState;

/**
 * Get the public game state data of other users, based on the user id of a custom provider.
 * @param provider The provider to request the data from
 * @param userIds The user ids
 */
+(void)getOtherUsersGameState:(nonnull NSString*)provider userIds:(nonnull NSArray*)userIds;

/**
 * NOTE: Depricated
 * Request the private gamestate.
 */
+(void)requestMyGameState;

#pragma image cache

/**
 * Converts a web url to local file path
 * @param url The image url
 * Return the local image path, returns nil if their is no local file path for the url
 */
+(nullable NSString*)getImagePathForUrl:(nonnull NSString*)url;

/**
 * Requests an image based on the url provided
 * @param url The URL to load
 * @param idx The id this image belongs to (optional)
 * @param imageType The image type of this image (optional)
 */
+(void)requestImage:(nonnull NSString*)url withId:(int)idx withImageType:(nullable NSString*)imageType;

/**
 * Removes all images from the disk cache
 */
+(void)clearDiskCache;

/**
 * Automatically preloads all images for all items and bundles
 */
+(void)preloadItemAndBundleImages;

#pragma live events

/**
 * Request the live event
 */
+(void)requestLiveEvent;

/**
 * Open the live event
 */
+(void)openLiveEvent;

/**
 * Request the live event start
 */
+(int)getLiveEventStartDate;

/**
 * Request the live event end time
 */
+(int)getLiveEventEndDate;

/**
 * Request the live event config
 */
+(nonnull NSDictionary*)getLiveEventConfig;

#pragma user login

/**
 * Used to login, will automatically call setExternalUserId
 * @param externalUserId The social user id to use
 * @param externalProviderId The id of the service (e.g. facebook)
 * @param externalToken The auth token provided by the external provider
 */
+(void)loginWithExternalUserId:(nonnull NSString*)externalUserId externalProviderId:(nonnull NSString*)externalProviderId externalToken:(nonnull NSString*)externalToken;

/**
 * Returns the current login status
 */
+(BOOL)isLoggedIn;

/**
 * Logout the user
 */
+(void)logout:(BOOL)global;

/**
 * Logout the user and let him continue as guest
 */
+(void)userPlayAsGuest;

/**
 * Will reset the gameconfig, packages/promotions, gamedata, playerdata & gamestate
 */
+(void)resetData;

/**
 * Show the default auth error dialog
 */
+(void)showAuthorizedDialog:(nonnull NSString*)title message:(nonnull NSString*)message loginButtonText:(nonnull NSString*)loginButtonText guestButtonText:(nonnull NSString*)guestButtonText;

#pragma mark data syncing

/**
 * Get the device id
 */
+(nonnull NSString*)getDeviceId;

/**
 * Request the current userdata, which includes the wallet, inventory and private gamestate
 */
+(void)requestUserData;

/**
 * Used to merge resolve a merge conflict in the current userdata which is triggered by mergeConflict:remoteData: delegate
 * @param mergedUserData The result of the merge
 */
+(void)mergeUserData:(nonnull NSString*)mergedUserData mergeType:(nonnull NSString*)mergeType;

/**
 * Show the default merge error dialog
 */
+(void)showMergeConflictDialog:(nonnull NSString*)title message:(nonnull NSString*)message localButtonText:(nonnull NSString*)localButtonText remoteButtonText:(nonnull NSString*)remoteButtonText mergeButtonText:(nonnull NSString*)mergeButtonText;

/**
 * Show the default merge error dialog
 */
+(void)showSyncErrorDialog:(nonnull NSString*)title message:(nonnull NSString*)message mergeButtonText:(nonnull NSString*)mergeButtonText;

/**
 * Show the default user data merge failed dialog
 */
+(void)showUserDataMergeFailedDialog:(nonnull NSString*)title withMessage:(nonnull NSString*)message retryButtonText:(nonnull NSString*)retryButton mergeData:(nonnull NSString*)mergeData mergeType:(nonnull NSString*)mergeType;

#pragma test methods (dev)

/**
 * NOTE: Those methods are exposed just for ad testing, they should not be referenced in the final implementation, params:
 * adProvider: DFP|Fyber|ChartBoost
 * adType: interstitial|rewardVideo|moreApps
 * parentalGate: not implemented yet (always false)
 */

+(void)devRequestAd:(nonnull NSString*)provider withAdType:(nonnull NSString*)adType withParentalGate:(BOOL)parentalGate;
+(nullable NSString*)getRawAdProvidersData;

@end
