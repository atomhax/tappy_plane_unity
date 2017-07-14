//
//  Inventory.h
//  Spil
//
//  Copyright © 2016 Spil Games. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "PlayerItem.h"

@interface Inventory : NSObject

@property (strong, nonatomic) NSMutableArray *items; // PlayerItem
@property (nonatomic) int offset;
@property (strong, nonatomic) NSString* logic;

-(id)init;
-(id)initWithDictionary:(NSDictionary*)data;

-(NSDictionary*)toJSONObject;
-(NSString*)toJSONString;

-(NSArray*)getItemsJSONArray;

-(void)updateItem:(PlayerItem*)item;

-(NSArray*)getUpdatedItems:(bool)includeContent;

-(PlayerItem*)getItem:(int)id;

-(void)reset;

-(void)resetDeltas;

@end
