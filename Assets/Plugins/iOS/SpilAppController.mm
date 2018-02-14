#import "UnityAppController.h"
#import "Spil/Spil.h"

@interface SpilAppController : UnityAppController {
}

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions;
- (void)application:(UIApplication *)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken;
- (void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo fetchCompletionHandler:(void (^)(UIBackgroundFetchResult result))completionHandler;
- (BOOL)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary<NSString *, id> *)options;
- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation;
- (BOOL)application:(UIApplication *)application continueUserActivity:(NSUserActivity *)userActivity restorationHandler:(void (^)(NSArray *))restorationHandler;
- (NSUInteger)application:(UIApplication*)application supportedInterfaceOrientationsForWindow:(UIWindow*)window;

@end

@implementation SpilAppController

// Used to setup the spil sdk
- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
    [Spil application:application didFinishLaunchingWithOptions:launchOptions];
    return [super application:application didFinishLaunchingWithOptions:launchOptions];
}

// Used to register for push notifications using the Spil SDK
- (void)application:(UIApplication *)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken {
    [Spil didRegisterForRemoteNotificationsWithDeviceToken:deviceToken];
}

// Used to handle push notification using the Spil SDK
- (void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo fetchCompletionHandler:(void (^)(UIBackgroundFetchResult result))completionHandler {
    [Spil application:application didReceiveRemoteNotification:userInfo];
}

// Used to handle deeplinking, through a url scheme
- (BOOL)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary<NSString *, id> *)options {
    
    NSString *inUrl = [url absoluteString];
    NSLog(@"[SPIL] (BOOL)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary<NSString *, id> *)options, with URL: %@. With options: %@", inUrl, options);
    
    NSString *sourceApplication = options != nil ? options[UIApplicationOpenURLOptionsSourceApplicationKey] : nil;
    NSString *annotation = options != nil ? options[UIApplicationOpenURLOptionsAnnotationKey] : nil;
    return [self application:app openURL:url sourceApplication:sourceApplication annotation:annotation];
}

// Used to handle deeplinking, through a url scheme
- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation {
    
    NSString *inUrl = [url absoluteString];
    NSLog(@"[SPIL] (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation, with URL: %@", inUrl);
    
    // Temp: added to prevent a fb sdk login crash when sourceApplication or annotation is nil
    if (annotation == nil) {
        annotation = [NSNull null];
    }
    if (sourceApplication == nil) {
        sourceApplication = @"";
    }
    [Spil application:application openURL:url sourceApplication:sourceApplication annotation:annotation];
    return [super application:application openURL:url sourceApplication:sourceApplication annotation:annotation];
}

// Used to handle deeplinking, through a universal link
- (BOOL)application:(UIApplication *)application continueUserActivity:(NSUserActivity *)userActivity restorationHandler:(void (^)(NSArray *))restorationHandler {
    
    [Spil application:application continueUserActivity:userActivity restorationHandler:restorationHandler];
    
    return TRUE;
    
    // This throws an exception because the method does not exist.
    // return [super application:application continueUserActivity:userActivity restorationHandler:restorationHandler];
}

- (void)applicationDidBecomeActive:(UIApplication *)application {
	[Spil applicationDidBecomeActive:application];
    [super applicationDidBecomeActive:application];
}

- (void)applicationDidEnterBackground:(UIApplication *)application {
    [Spil applicationDidEnterBackground:application];
    [super applicationDidEnterBackground:application];
}

- (NSUInteger)application:(UIApplication*)application supportedInterfaceOrientationsForWindow:(UIWindow*)window {
    return (1 << UIInterfaceOrientationLandscapeLeft) |
           (1 << UIInterfaceOrientationLandscapeRight);
}

@end

IMPL_APP_CONTROLLER_SUBCLASS(SpilAppController)
