using UnityEngine;

namespace AdSystem
{
    [CreateAssetMenu(fileName = "AdConfig", menuName = "AdSystem/Ad Config")]
    public class AdConfig : ScriptableObject
    {
        [Header("Interstitial Ad Unit IDs")]
         [SerializeField] private string interstitialAndroid = "ANDROID_INT_ID_HERE";
         [SerializeField] private string interstitialIOS = "IOS_INT_ID_HERE";

        [Header("Rewarded Ad Unit IDs")]
        [SerializeField] private string rewardedAndroid = "ANDROID_REWARDED_ID_HERE";
        [SerializeField] private string rewardedIOS = "IOS_REWARDED_ID_HERE";

        public string InterstitialAdUnitId => Platform(interstitialAndroid, interstitialIOS);

        public string RewardedAdUnitId => Platform(rewardedAndroid, rewardedIOS);

        private static string Platform(string android, string ios)
        {
#if UNITY_IOS
            return ios;
#else
            return android;
#endif
        }
    }
}