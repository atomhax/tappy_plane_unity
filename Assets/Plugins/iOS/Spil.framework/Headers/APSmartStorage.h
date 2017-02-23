//
//  APSmartStorage.h
//  APSmartStorage
//
//  Created by Alexey Belkevich on 1/15/14.
//  Copyright (c) 2014 alterplay. All rights reserved.
//

#import <Foundation/Foundation.h>

typedef id (^APParsingBlock)(NSData *data, NSURL *url);

@interface APSmartStorage : NSObject

@property (nonatomic, copy) APParsingBlock parsingBlock;
@property (nonatomic, assign) NSUInteger maxObjectCount;
@property (nonatomic, assign) BOOL isExcludedFromBackup;
@property (nonatomic, strong) NSURLSessionConfiguration *sessionConfiguration;

+ (instancetype)sharedInstance;

- (void)loadObjectWithURL:(NSURL *)url completion:(void (^)(id object, NSError *error, NSURL *url))completion;
- (void)loadObjectWithURL:(NSURL *)url storeInMemory:(BOOL)storeInMemory completion:(void (^)(id object, NSError *error, NSURL *url))completion;
- (void)loadObjectWithURL:(NSURL *)url storeInMemory:(BOOL)storeInMemory progress:(void (^)(NSUInteger percents))progress completion:(void (^)(id object, NSError *error, NSURL *url))completion;

- (void)reloadObjectWithURL:(NSURL *)url completion:(void (^)(id object, NSError *error, NSURL *url))completion;
- (void)reloadObjectWithURL:(NSURL *)url storeInMemory:(BOOL)storeInMemory completion:(void (^)(id object, NSError *error, NSURL *url))completion;
- (void)reloadObjectWithURL:(NSURL *)url storeInMemory:(BOOL)storeInMemory progress:(void (^)(NSUInteger percents))progress completion:(void (^)(id object, NSError *error, NSURL *url))completion;

- (void)removeObjectWithURLFromMemory:(NSURL *)url;
- (void)removeObjectWithURLFromStorage:(NSURL *)url;

- (void)removeAllFromMemory;
- (void)removeAllFromStorage;

- (NSString*)filePathForURL:(NSString*)url;

@end
