#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace AdSystem.Editor
{
    /// <summary>
    /// Injects the App Tracking Transparency usage description into the generated
    /// Xcode Info.plist. Without this key, calling the ATT prompt crashes the app,
    /// so this runs automatically on every iOS build — no manual setup required.
    /// </summary>
    public static class IOSPostProcessor
    {
        // Shown in the system ATT prompt and reviewed by Apple. Adjust the wording
        // to match how your game actually uses tracking.
        private const string TrackingUsageDescription =
            "Your data will be used to deliver personalized ads to you.";

        [PostProcessBuild(100)]
        public static void OnPostProcessBuild(BuildTarget target, string buildPath)
        {
            if (target != BuildTarget.iOS) return;

            var plistPath = Path.Combine(buildPath, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            plist.root.SetString("NSUserTrackingUsageDescription", TrackingUsageDescription);

            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
}
#endif
