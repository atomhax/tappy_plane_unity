//
//  SpilUserHandler.h
//  Spil
//
//  Copyright © 2016 Spil Games. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface SpilUserHandler : NSObject

+(SpilUserHandler*)sharedInstance;

// --- Properties ---

@property (nonatomic, assign) BOOL authConflicted;
@property (nonatomic, assign) int authErrorCode;
@property (nonatomic, retain) NSMutableDictionary *deviceVersions;
@property (nonatomic, retain) NSMutableDictionary *metadata;

// --- Spil user id ---

-(void)syncSpilUserId;
-(NSString*)getSpilUserId;

// --- Device id ---

-(NSString*)getDeviceId;

// --- Spil user token ---

-(NSString*)getSpilUserToken;

// --- Reset ---

-(void)resetUID:(NSString*)uid;
-(void)resetSpilUserToken:(NSString*)token;
-(void)resetProfile;

// --- Login ---

-(void)loginWithExternalUserId:(NSString*)externalUserId externalProviderId:(NSString*)externalProviderId externalToken:(NSString*)externalToken;
-(BOOL)isLoggedIn;
-(void)logout:(BOOL)global;
-(void)userPlayAsGuest;
-(void)resetData;
-(void)showOnAuthorizedDialog:(NSString*)title message:(NSString*)message loginButtonText:(NSString*)loginButtonText guestButtonText:(NSString*)guestButtonText;
-(void)showOnMergeDialog:(NSString*)title message:(NSString*)message localButtonText:(NSString*)localButtonText remoteButtonText:(NSString*)remoteButtonText mergeButtonText:(NSString*)mergeButtonText;
-(void)showOnSyncDialog:(NSString*)title message:(NSString*)message mergeButtonText:(NSString*)mergeButtonText;

// --- Userdata ---

-(void)setPrivateGameState:(NSString*)privateData sendUpdate:(Boolean)sendUpdate;
-(NSString*)getPrivateGameState;
-(void)setPublicGameState:(NSString*)publicData sendUpdate:(Boolean)sendUpdate;
-(NSString*)getPublicGameState;
-(void)getOtherUsersGameState:(NSString*)provider userIds:(NSArray*)userIds;

// --- Userdata syncing ---

-(void)requestUserData;
-(void)mergeUserData:(NSString*)mergedUserData mergeType:(NSString*)mergeType;
-(void)increaseDeviceVersion;

// --- Auth errors ---

-(void)onAuthError:(int)pAuthErrorCode;

// --- External user id ---

-(void)setExternalUserId:(NSString*)userId forProviderId:(NSString*)providerId;
-(NSString*)getExternalUserId;
-(NSString*)getExternalUserProvider;
-(NSDictionary*)getExternalUserRequestData;

// --- Updates ---

-(void)friendsGameStateLoaded:(NSDictionary*)gameStates provider:(NSString*)provider;

@end
