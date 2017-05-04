//
//  Bundle.h
//  Spil
//
//  Copyright © 2016 Spil Games. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "BundleItem.h"
#import "BundlePrice.h"

@interface Bundle : NSObject

@property (assign, nonatomic) int id;
@property (strong, nonatomic) NSString *name;
@property (strong, nonatomic) NSMutableArray *prices; // Array of BundlePrice's
@property (strong, nonatomic) NSMutableArray *items; // Array of BundleItem's
@property (strong, nonatomic) NSString *imageUrl;
@property (strong, nonatomic) NSString *displayName;
@property (strong, nonatomic) NSString *displayDescription;

-(id)initWithDictionary:(NSDictionary*)data;

-(NSDictionary*)toJSONObject;

-(NSArray*)getItemsJSONArray;
-(NSArray*)getPricesJSONArray;

@end
