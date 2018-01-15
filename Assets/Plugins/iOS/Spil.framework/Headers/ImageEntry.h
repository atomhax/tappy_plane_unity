//
//  ImageEntry.h
//  Spil
//
//  Created by Frank Slofstra on 05/12/2017.
//  Copyright © 2017 Spil Games. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface ImageEntry : NSObject

@property (nonatomic, strong) NSString* name;
@property (nonatomic, strong) NSString* imageUrl;

-(id)initWithDictionary:(NSDictionary*)dict;

-(NSDictionary*)toJSONObject;

@end
