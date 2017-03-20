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
    NSString *sourceApplication = options[UIApplicationOpenURLOptionsSourceApplicationKey];
    NSString *annotation = options[UIApplicationOpenURLOptionsAnnotationKey];
    return [self application:app openURL:url sourceApplication:sourceApplication annotation:annotation];
}

// Used to handle deeplinking, through a url scheme
- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation {
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
    return [super application:application continueUserActivity:userActivity restorationHandler:restorationHandler];
}

@end

IMPL_APP_CONTROLLER_SUBCLASS(SpilAppController)