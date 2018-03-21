//
//  PackageItem.h
//  Spil
//
//  Created by Frank Slofstra on 02/02/2018.
//  Copyright © 2018 Spil Games. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface PackageItem : NSObject

@property (assign, nonatomic) int id;
@property (strong, nonatomic) NSString *type;
@property (assign, nonatomic) int value;
    
-(id)initWithDictionary:(NSDictionary*)dict;
    
-(NSMutableDictionary*)toJSONObject;
    
@end
