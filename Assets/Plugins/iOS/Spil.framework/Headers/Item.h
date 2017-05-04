//
//  Item.h
//  Spil
//
//  Copyright © 2016 Spil Games. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface Item : NSObject

@property (assign, nonatomic) int id;
@property (strong, nonatomic) NSString *name;
@property (assign, nonatomic) int type;
@property (assign, nonatomic) int initialValue;
@property (strong, nonatomic) NSString *imageUrl;
@property (strong, nonatomic) NSString *displayName;
@property (strong, nonatomic) NSString *displayDescription;

-(id)initWithDictionary:(NSDictionary*)dict;

-(NSMutableDictionary*)toJSONObject;

@end
