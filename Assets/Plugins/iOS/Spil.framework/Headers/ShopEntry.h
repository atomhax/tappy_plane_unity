//
//  ShopEntry.h
//  Spil
//
//  Copyright © 2016 Spil Games. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Bundle.h"
#import "ImageEntry.h"
#import "Package.h"

@interface ShopEntry : NSObject

@property (nonatomic, assign) int id;
@property (nonatomic, assign) NSString *type; // PACKAGE|BUNDLE
@property (nonatomic, strong) NSString* label;
@property (nonatomic, assign) int position;
@property (nonatomic, strong) NSMutableArray *imageEntries; // ImageEntry

-(id)initWithDictionary:(NSDictionary*)dict;

-(NSDictionary*)toJSONObject;

-(Bundle*)getBundle;

-(Package*)getPackage;

-(NSString*)getImageUrl:(NSString*)name;

@end
