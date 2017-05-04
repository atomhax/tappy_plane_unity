//
//  ShopEntry.h
//  Spil
//
//  Copyright © 2016 Spil Games. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Bundle.h"

@interface ShopEntry : NSObject

@property (nonatomic, assign) int bundleId;
@property (nonatomic, strong) NSString* label;
@property (nonatomic, assign) int position;

-(id)initWithDictionary:(NSDictionary*)dict;

-(NSDictionary*)toJSONObject;

-(Bundle*)getBundle;

@end
