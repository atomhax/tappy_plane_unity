//
//  SpilPackageHandler.h
//
//  Copyright © 2016 Spil Games. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "Promotion.h"
#import "Package.h"
#import "ShopEntry.h"

@interface SpilPackageHandler : NSObject

@property (strong, nonatomic) NSMutableArray *loadedPackages;
@property (strong, nonatomic) NSMutableArray *loadedPromotions;

+(SpilPackageHandler*)sharedInstance;

-(void)requestPackages;
-(void)savePackages;
-(NSArray*)getAllPackages;
-(NSArray*)getAllPackagesObject;
-(NSDictionary*)getPackageByPackageID:(NSString*)packageId;
-(Package*)getPackageByPackageIDObject:(NSString*)packageId;
-(NSDictionary*)getPackageByID:(int)id;
-(Package*)getPackageByIDObject:(int)id;

-(void)requestPromotions;
-(void)savePromotions;
-(NSArray*)getAllPromotions;
-(NSArray*)getAllPromotionsObject;
-(NSDictionary*)getBundlePromotion:(int)bundleId;
-(Promotion*)getBundlePromotionObject:(int)bundleId;
-(NSDictionary*)getPackagePromotion:(NSString*)packageId;
-(Promotion*)getPackagePromotionObject:(NSString*)packageId;
-(void)trackBoughtPromotion:(Promotion*)promotion;
-(void)showPromotionScreen:(int)promotionId;
-(BOOL)hasActiveTabPromotion:(int)tabId;
-(BOOL)hasActiveEntryPromotion:(ShopEntry*)shopEntry;
-(BOOL)hasPackagePromotion:(NSString*)packageId;

-(NSArray*)getImageUrls;

@end
