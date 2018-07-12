//
//  PlayerCurrency.h
//  Spil
//
//  Copyright © 2016 Spil Games. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Currency.h"

@interface PlayerCurrency : Currency

@property (nonatomic) int currentBalance;
@property (nonatomic) int delta;
@property (nonatomic) int overflow;

-(id)init;
-(id)initWithCurrency:(Currency*)currency;
-(id)initWithDictionary:(NSDictionary*)dict;

-(NSDictionary*)toJSONObject;

@end
