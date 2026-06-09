namespace AdSystem
{
    public enum AdEventType
    {
        Show,
        Click,
        FailedShow,
        RewardReceived
    }

    public enum AdType
    {
        Interstitial,
        Rewarded,
        Banner
    }

    /// <summary>
    /// Network-agnostic analytics contract. Day-1 / Day-7 / Day-30 retention,
    /// DAU/MAU and sessions are derived automatically by the backend from the
    /// SDK's session events, so this surface only covers custom + ad events.
    /// </summary>
    public interface IAnalyticsService
    {
        void Initialize();

        void LogEvent(string eventName);
        void LogEvent(string eventName, float value);

        void LogAdEvent(AdEventType action, AdType type, string placement);
    }
}
