//
//  SpilPackageHandler.h
//
//  Copyright © 2016 Spil Games. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface SpilPackageHandler : NSObject

+(SpilPackageHandler*)sharedInstance;

-(NSArray*)getAllPromotions;
-(NSArray*)getAllPackages;
-(NSDictionary*)getPackageByID:(NSString*)packageId;
-(NSArray*)getPromotionsByID:(NSString*)promotionId;
-(void)requestPackages;
-(void)storePackages:(NSDictionary*)data;

-(NSArray*)getImageUrls;

@end
