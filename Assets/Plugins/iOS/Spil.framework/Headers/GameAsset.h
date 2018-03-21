//
//  GameAsset.h
//  Spil
//
//  Created by Frank Slofstra on 05/02/2018.
//  Copyright © 2018 Spil Games. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface GameAsset : NSObject

@property (strong, nonatomic) NSString *locale;
@property (strong, nonatomic) NSString *position;
@property (strong, nonatomic) NSString *type;
@property (strong, nonatomic) NSString *name;
@property (strong, nonatomic) NSString *value;

-(id)initWithDictionary:(NSDictionary*)dict;

-(NSMutableDictionary*)toJSONObject;

@end
