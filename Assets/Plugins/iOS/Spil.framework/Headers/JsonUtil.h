//
//  JsonUtil.h
//  Spil
//
//  Util class to convert foundation objects to and from json.
//  It does not support serialization.
//
//  Copyright © 2016 Spil Games. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface JsonUtil : NSObject

+(id)convertStringToObject:(NSString*) jsonString;
+(NSString*)convertObjectToJson:(id) object;
+(NSString*)convertObjectToReadableJson:(id) object;

@end
