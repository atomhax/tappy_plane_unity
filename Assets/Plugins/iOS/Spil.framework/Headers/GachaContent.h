//
//  Gacha.h
//  Spil
//
//  Created by Frank Slofstra on 11/07/2017.
//  Copyright © 2017 Spil Games. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface GachaContent : NSObject

@property (assign, nonatomic) int amount;
@property (assign, nonatomic) int id;
@property (strong, nonatomic) NSString *type;
@property (assign, nonatomic) int weight;

-(id)initWithDictionary:(NSDictionary*)data;

-(NSDictionary*)toJSONObject;

@end
