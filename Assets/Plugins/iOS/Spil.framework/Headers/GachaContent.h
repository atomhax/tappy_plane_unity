//
//  Gacha.h
//  Spil
//
//  Copyright © 2017 Spil Games. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface GachaContent : NSObject

@property (assign, nonatomic) int amount;
@property (assign, nonatomic) int id;
@property (strong, nonatomic) NSString *type;
@property (assign, nonatomic) int weight;
@property (assign, nonatomic) int position;
@property (strong, nonatomic) NSString *imageUrl;

-(id)initWithDictionary:(NSDictionary*)data;

-(NSDictionary*)toJSONObject;

@end
