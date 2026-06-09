using System;
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
using AOT;
#endif

namespace AdSystem
{
    /// <summary>
    /// iOS App Tracking Transparency (ATT) helper. On iOS 14+ the system shows the
    /// tracking permission prompt; ads/analytics can only read the IDFA once the
    /// user has responded, so the SDK requests this <b>before</b> initialization.
    /// On Android / Editor / pre-iOS 14 it is a no-op that reports "authorized".
    /// </summary>
    public static class AppTracking
    {
#if UNITY_IOS && !UNITY_EDITOR
        private delegate void TrackingAuthorizationCallback(int status);

        [DllImport("__Internal")]
        private static extern void _AdSystemRequestTrackingAuthorization(TrackingAuthorizationCallback callback);

        private static Action<int> _onComplete;

        [MonoPInvokeCallback(typeof(TrackingAuthorizationCallback))]
        private static void HandleAuthorization(int status)
        {
            var callback = _onComplete;
            _onComplete = null;
            callback?.Invoke(status);
        }

        /// <param name="onComplete">
        /// Called (on the main thread) once the user responds. Status: 0 = not
        /// determined, 1 = restricted, 2 = denied, 3 = authorized.
        /// </param>
        public static void Request(Action<int> onComplete)
        {
            _onComplete = onComplete;
            _AdSystemRequestTrackingAuthorization(HandleAuthorization);
        }
#else
        public static void Request(Action<int> onComplete) => onComplete?.Invoke(3);
#endif
    }
}
