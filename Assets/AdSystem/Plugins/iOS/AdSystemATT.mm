#import <Foundation/Foundation.h>
#import <AppTrackingTransparency/AppTrackingTransparency.h>

typedef void (*AdSystemTrackingCallback)(int status);

extern "C" {

void _AdSystemRequestTrackingAuthorization(AdSystemTrackingCallback callback)
{
    if (@available(iOS 14, *))
    {
        [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {
            // Marshal back to the main thread before re-entering Unity.
            dispatch_async(dispatch_get_main_queue(), ^{
                if (callback != NULL)
                {
                    callback((int)status);
                }
            });
        }];
    }
    else
    {
        // Pre-iOS 14: ATT is unavailable, treat as authorized.
        if (callback != NULL)
        {
            callback(3);
        }
    }
}

}
