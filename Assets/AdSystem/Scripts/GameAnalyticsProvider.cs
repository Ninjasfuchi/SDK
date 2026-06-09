using GameAnalyticsSDK;
using UnityEngine;

namespace AdSystem
{
    /// <summary>
    /// GameAnalytics implementation of <see cref="IAnalyticsService"/>. Once
    /// initialized, GameAnalytics tracks sessions/DAU/retention automatically;
    /// this wrapper just forwards custom and ad events. Configure your game/secret
    /// keys via <c>Window ▸ GameAnalytics ▸ Select Settings</c>.
    /// </summary>
    public class GameAnalyticsProvider : IAnalyticsService
    {
        public void Initialize()
        {
            if (Object.FindObjectOfType<GameAnalytics>() == null)
            {
                var go = new GameObject("GameAnalytics");
                go.AddComponent<GameAnalytics>();
                Object.DontDestroyOnLoad(go);
            }

            GameAnalytics.Initialize();
        }

        public void LogEvent(string eventName) => GameAnalytics.NewDesignEvent(eventName);
        public void LogEvent(string eventName, float value) => GameAnalytics.NewDesignEvent(eventName, value);

        public void LogAdEvent(AdEventType action, AdType type, string placement)
        {
            GameAnalytics.NewAdEvent(ToGaAction(action), ToGaType(type), "AppLovin", placement);
        }

        private static GAAdAction ToGaAction(AdEventType action) => action switch
        {
            AdEventType.Show => GAAdAction.Show,
            AdEventType.Click => GAAdAction.Click,
            AdEventType.FailedShow => GAAdAction.FailedShow,
            AdEventType.RewardReceived => GAAdAction.RewardReceived,
            _ => GAAdAction.Show
        };

        private static GAAdType ToGaType(AdType type) => type switch
        {
            AdType.Interstitial => GAAdType.Interstitial,
            AdType.Rewarded => GAAdType.RewardedVideo,
            AdType.Banner => GAAdType.Banner,
            _ => GAAdType.Interstitial
        };
    }
}
