//
//  ImageContext.h
//  Spil
//
//  Created by Frank Slofstra on 20/02/17.
//  Copyright © 2017 Spil Games. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface ImageContext : NSObject

@property (nonatomic, assign) int id;
@property (nonatomic, retain) NSString *imageType;
@property (nonatomic, retain) NSString *imageUrl;

-(id)initWithIdx:(int)idx imageType:(NSString*)imageTypeId url:(NSString*)url;

-(NSMutableDictionary*)toJSONObject;

@end
