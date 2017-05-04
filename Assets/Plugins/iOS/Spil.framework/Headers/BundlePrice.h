//
//  BundlePrice.h
//  Spil
//
//  Copyright © 2016 Spil Games. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "PlayerCurrency.h"

@interface BundlePrice : NSObject

@property (assign, nonatomic) int currencyId;
@property (assign, nonatomic) int value;

-(id)initWithDictionary:(NSDictionary*)dict;

-(NSDictionary*)toJSONObject;

-(PlayerCurrency*)getCurrency;

@end
