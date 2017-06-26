//
//  SpilActionHandler.h
//  trackerSample
//
//  Copyright (c) 2015 Spil Games. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

@interface SpilActionHandler : NSObject

+(void)handleAction:(NSDictionary*)action withCallBackUID:(NSString*)callbackUID;
+(void)handleAction:(NSDictionary*)action withResponse:(void (^)(id response))block;
+(void)showWebview:(NSDictionary*)data withActionName:(NSString*)name;

@end
