#import "UnityAppController.h"
#import "Spil/Spil.h"

@interface SpilAppController : UnityAppController {
}

- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation;
- (BOOL)application:(UIApplication *)application continueUserActivity:(NSUserActivity *)userActivity restorationHandler:(void (^)(NSArray *))restorationHandler;

@end

@implementation SpilAppController

// Used to handle deeplinking, through a url scheme
- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation {
    // Temp: added to prevent a fb sdk login crash when annotation is nil
    if (annotation == nil) {
        annotation = [NSNull null];
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
