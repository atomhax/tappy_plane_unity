//
//  SpilUserHandler.h
//  Spil
//
//  Copyright © 2016 Spil Games. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface SpilUserHandler : NSObject

+(SpilUserHandler*)sharedInstance;

// Spil user id

-(void)syncSpilUserId;
-(NSString*)getSpilUserId;

// Reset

-(void)resetUID;
-(void)resetProfile;

// External user id

-(void)setExternalUserId:(NSString*)userId forProviderId:(NSString*)providerId;
-(NSString*)getExternalUserId;
-(NSString*)getExternalUserProvider;
-(NSDictionary*)getExternalUserRequestData;

// User data

-(void)setPrivateGameState:(NSString*)privateData sendUpdate:(Boolean)sendUpdate;
-(NSString*)getPrivateGameState;
-(void)setPublicGameState:(NSString*)publicData sendUpdate:(Boolean)sendUpdate;
-(NSString*)getPublicGameState;
-(void)getOtherUsersGameState:(NSString*)provider userIds:(NSArray*)userIds;
-(void)requestMyGameState;

// Updates

-(void)gameStateUpdateReceived:(NSString*)privateData public:(NSString*)publicData;
-(void)friendsGameStateLoaded:(NSDictionary*)gameStates provider:(NSString*)provider;

@end
