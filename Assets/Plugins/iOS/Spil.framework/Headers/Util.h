//
//  Util.h
//  Spil
//
//  Copyright © 2016 Spil Games. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

@interface Util : NSObject

+(UIColor*)colorFromHexString:(NSString *)hexString;

+(BOOL)date:(NSDate*)date isBetweenDate:(NSDate*)beginDate andDate:(NSDate*)endDate;
+(NSDate*)parseDate:(NSString*)string;

+(NSString*)urlEncode:(NSString*)input;
+(NSString*)urlDecode:(NSString*)input;

+(NSString*)sha256:(NSString *)input;
+(NSString*)createSHA512:(NSString *)input;

+(UIViewController*)topMostController;

@end
