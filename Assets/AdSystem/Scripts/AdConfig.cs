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

        public string InterstitialAdUnitId =>
#if UNITY_ANDROID
            interstitialAndroid;
#elif UNITY_IOS
            interstitialIOS;
#else
            interstitialAndroid;
#endif

        public string RewardedAdUnitId =>
#if UNITY_ANDROID
            rewardedAndroid;
#elif UNITY_IOS
            rewardedIOS;
#else
            rewardedAndroid;
#endif
    }
}