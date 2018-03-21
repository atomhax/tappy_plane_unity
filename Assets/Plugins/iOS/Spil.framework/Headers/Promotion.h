//
//  Promotion.h
//  Spil
//
//  Created by Frank Slofstra on 05/02/2018.
//  Copyright © 2018 Spil Games. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface Promotion : NSObject

@property (assign, nonatomic) int id;
@property (strong, nonatomic) NSString *name;
@property (assign, nonatomic) int amountPurchased;
@property (assign, nonatomic) int maxPurchase;
@property (strong, nonatomic) NSString *label;
@property (assign, nonatomic) long long startDate;
@property (assign, nonatomic) long long endDate;
@property (strong, nonatomic) NSMutableArray *affectedEntities; // Array of PromotionEntity
@property (strong, nonatomic) NSMutableArray *extraEntities; // Array of PromotionEntity
@property (strong, nonatomic) NSMutableArray *priceOverride; // Array of PromotionEntity
@property (strong, nonatomic) NSMutableArray *gameAssets; // Array of GameAsset

-(id)initWithDictionary:(NSDictionary*)dict;

-(NSMutableDictionary*)toJSONObject;

@end
