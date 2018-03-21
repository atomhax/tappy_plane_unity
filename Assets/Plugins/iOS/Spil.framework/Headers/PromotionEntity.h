//
//  PromotionEntity.h
//  Spil
//
//  Created by Frank Slofstra on 05/02/2018.
//  Copyright © 2018 Spil Games. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface PromotionEntity : NSObject

@property (assign, nonatomic) int id;
@property (strong, nonatomic) NSString *type; // ITEM|CURRENCY|PACKAGE|BUNDLE
@property (assign, nonatomic) int amount;

-(id)initWithDictionary:(NSDictionary*)dict;

-(NSMutableDictionary*)toJSONObject;

@end
