using System;
using System.Reflection;
using UnityEngine;

namespace AdSystem
{
    /// <summary>
    /// GameAnalytics implementation of <see cref="IAnalyticsService"/>.
    ///
    /// GameAnalytics ships its C# API in the default assembly (no asmdef), which an
    /// asmdef-based package cannot reference directly. To stay a self-contained,
    /// drop-in package we bind to GameAnalytics via reflection: no compile-time
    /// dependency, and if GameAnalytics is absent the provider degrades to a no-op.
    ///
    /// Once initialized, GameAnalytics tracks sessions/DAU/retention automatically;
    /// this wrapper just forwards custom and ad events. Configure your game/secret
    /// keys via <c>Window ▸ GameAnalytics ▸ Select Settings</c>.
    /// </summary>
    public class GameAnalyticsProvider : IAnalyticsService
    {
        private Type gaType;
        private Type adActionType;
        private Type adTypeType;

        private MethodInfo designEvent1;
        private MethodInfo designEvent2;
        private MethodInfo adEvent;

        public void Initialize()
        {
            gaType = FindType("GameAnalyticsSDK.GameAnalytics");
            if (gaType == null)
            {
                Debug.LogWarning("[AdSystem] GameAnalytics SDK not found in project. Analytics disabled.");
                return;
            }

            adActionType = FindType("GameAnalyticsSDK.GAAdAction");
            adTypeType = FindType("GameAnalyticsSDK.GAAdType");

            CacheMethods();
            EnsureRuntimeObject();

            var init = gaType.GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static,
                null, Type.EmptyTypes, null);
            init?.Invoke(null, null);
        }

        public void LogEvent(string eventName)
        {
            designEvent1?.Invoke(null, new object[] { eventName });
        }

        public void LogEvent(string eventName, float value)
        {
            if (designEvent2 == null) return;
            var valueType = designEvent2.GetParameters()[1].ParameterType;
            var converted = Convert.ChangeType(value, valueType);
            designEvent2.Invoke(null, new[] { eventName, converted });
        }

        public void LogAdEvent(AdEventType action, AdType type, string placement)
        {
            if (adEvent == null || adActionType == null || adTypeType == null) return;

            var gaAction = Enum.Parse(adActionType, action.ToString());
            var gaType = Enum.Parse(adTypeType, ToGaTypeName(type));
            adEvent.Invoke(null, new[] { gaAction, gaType, "AppLovin", placement });
        }

        private void CacheMethods()
        {
            foreach (var m in gaType.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                var p = m.GetParameters();

                if (m.Name == "NewDesignEvent")
                {
                    if (p.Length == 1 && p[0].ParameterType == typeof(string))
                        designEvent1 = m;
                    else if (p.Length == 2 && p[0].ParameterType == typeof(string) &&
                             (p[1].ParameterType == typeof(float) || p[1].ParameterType == typeof(double)))
                        designEvent2 = m;
                }
                else if (m.Name == "NewAdEvent" && adActionType != null &&
                         p.Length == 4 && p[0].ParameterType == adActionType)
                {
                    adEvent = m;
                }
            }
        }

        private void EnsureRuntimeObject()
        {
#pragma warning disable CS0618 // FindObjectOfType: kept for compatibility with older Unity versions
            if (UnityEngine.Object.FindObjectOfType(gaType) != null) return;
#pragma warning restore CS0618

            var go = new GameObject("GameAnalytics");
            go.AddComponent(gaType);
            UnityEngine.Object.DontDestroyOnLoad(go);
        }

        private static Type FindType(string fullName)
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var t = asm.GetType(fullName);
                if (t != null) return t;
            }

            return null;
        }

        private static string ToGaTypeName(AdType type) => type switch
        {
            AdType.Interstitial => "Interstitial",
            AdType.Rewarded => "RewardedVideo",
            AdType.Banner => "Banner",
            _ => "Interstitial"
        };
    }
}
