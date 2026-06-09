namespace AdSystem
{
    /// <summary>
    /// Static entry point for analytics, mirroring <see cref="AdServices"/>.
    /// Defaults to a no-op service so calls are always safe (Editor, or before
    /// a provider is registered).
    /// </summary>
    public static class Analytics
    {
        public static IAnalyticsService Current { get; private set; } = new NullAnalyticsService();

        public static bool IsRegistered => Current is not NullAnalyticsService;

        public static void Register(IAnalyticsService service)
        {
            Current = service ?? new NullAnalyticsService();
        }

        public static void Unregister(IAnalyticsService service)
        {
            if (Current == service)
                Current = new NullAnalyticsService();
        }

        public static void LogEvent(string eventName) => Current.LogEvent(eventName);
        public static void LogEvent(string eventName, float value) => Current.LogEvent(eventName, value);
        public static void LogAdEvent(AdEventType action, AdType type, string placement) =>
            Current.LogAdEvent(action, type, placement);
    }

    /// <summary>No-op analytics used in the Editor and before registration.</summary>
    public sealed class NullAnalyticsService : IAnalyticsService
    {
        public void Initialize() { }
        public void LogEvent(string eventName) { }
        public void LogEvent(string eventName, float value) { }
        public void LogAdEvent(AdEventType action, AdType type, string placement) { }
    }
}
