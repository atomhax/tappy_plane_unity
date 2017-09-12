//
//  SpilEventTracker.h
//  trackerSample
//
//  Copyright (c) 2015 Spil Games. All rights reserved.
//


#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

@interface SpilEventTracker : NSObject

@property (nonatomic, retain) NSString *adjustId;

+(SpilEventTracker*)sharedInstance;

-(void)startUsingUnity:(BOOL)pUnityEnabled;

-(void)setCustomBundleId:(NSString*)bundleId;
-(void)setPluginInformation:(NSString*)pluginNameParam pluginVersion:(NSString*)pluginVersionParam;
-(void)setPushKey:(NSString*)pushKey;

-(NSString*)getBundleId;
-(NSString*)getLanguage;
-(NSString*)getOsVersion;
-(NSString*)getAppVersion;
-(NSString*)getDeviceModel;
-(NSString*)getTimezoneOffset;
-(NSString*)getSessionId;
-(NSString*)getEndPointPath;
-(NSString*)getTimeStamp;
-(NSString*)getPluginName;
-(NSString*)getPluginVersion;
-(NSString*)getPushKey;
-(BOOL)getIsUnity;
-(BOOL)isUnsafe;
-(BOOL)debugModeEnabled;
-(BOOL)stagingEnabled;

-(void)trackEvent:(NSString*)name;
-(void)trackEvent:(NSString*)name withParameters:(NSDictionary *)parameters;
-(void)trackEvent:(NSString*)name onResponse:(void (^)(id response))block;
-(void)trackEvent:(NSString*)name withParameters:(NSDictionary *)parameters onResponse:(void (^)(id response))block;

-(void)scheduleNextQueuedEvent:(BOOL)wasQueued hasConnection:(BOOL)hasConnection;

-(void)checkForNewSubdomain;

-(void)applicationDidEnterBackground:(UIApplication *)application;
-(void)applicationDidBecomeActive:(UIApplication *)application;

@end
