using System;

namespace AdSystem
{
    public interface IAdService
    {
        void ShowInterstitial();
        void ShowRewarded();

        bool IsInterstitialReady();
        bool IsRewardedReady();

        event Action OnInterstitialLoaded;
        event Action OnInterstitialFailed;
        event Action OnInterstitialClosed;
        event Action OnRewardedLoaded;
        event Action OnRewardedFailed;
        event Action OnRewardedClosed;
        event Action OnUserRewarded;
    }
}