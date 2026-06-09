using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdSystem
{
    public class SdkManager : MonoBehaviour, IAdService
    {
        [Header("Configuration")]
        [SerializeField] private AdConfig config;

        public event Action OnInterstitialLoaded;
        public event Action OnInterstitialFailed;
        public event Action OnInterstitialClosed;
        public event Action OnRewardedLoaded;
        public event Action OnRewardedFailed;
        public event Action OnRewardedClosed;
        public event Action OnUserRewarded;

        private int interstitialRetryAttempt;
        private int rewardedRetryAttempt;

        private Coroutine interstitialRetryCoroutine;
        private Coroutine rewardedRetryCoroutine;
        private readonly Dictionary<float, WaitForSecondsRealtime> waitCache = new();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            AdServices.Register(this);
        }

        private void Start()
        {
            InitializeSDK();
        }

        private void InitializeSDK()
        {
            if (!config)
            {
                Debug.LogError("[SdkManager] AdConfig is not assigned. Assign an AdConfig asset in the Inspector.");
                return;
            }

            MaxSdkCallbacks.OnSdkInitializedEvent += OnSdkInitialized;
            SetupAdCallbacks();
            MaxSdk.InitializeSdk();
        }

        private void OnSdkInitialized(MaxSdkBase.SdkConfiguration sdkConfiguration)
        {
            Debug.Log("SDK Initialized Successfully");
            LoadInterstitial();
            LoadRewarded();
        }

        private void SetupAdCallbacks()
        {
            // Interstitial
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedCallback;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedCallback;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenCallback;

            // Rewarded
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedLoadedCallback;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedFailedCallback;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedHiddenCallback;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnAdReceivedRewardCallback;
        }

        #region Interstitial Callbacks
        private void OnInterstitialLoadedCallback(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            interstitialRetryAttempt = 0;
            OnInterstitialLoaded?.Invoke();
        }

        private void OnInterstitialLoadFailedCallback(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            OnInterstitialFailed?.Invoke();

            interstitialRetryAttempt++;
            var retryDelay = Mathf.Pow(2, Mathf.Min(6, interstitialRetryAttempt));

            if (interstitialRetryCoroutine != null) StopCoroutine(interstitialRetryCoroutine);
            interstitialRetryCoroutine = StartCoroutine(WaitAndLoadInterstitial(retryDelay));
        }

        private void OnInterstitialHiddenCallback(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnInterstitialClosed?.Invoke();
            LoadInterstitial();
        }
        #endregion

        #region Rewarded Callbacks
        private void OnRewardedLoadedCallback(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            rewardedRetryAttempt = 0;
            OnRewardedLoaded?.Invoke();
        }

        private void OnRewardedFailedCallback(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            OnRewardedFailed?.Invoke();

            rewardedRetryAttempt++;
            var retryDelay = Mathf.Pow(2, Mathf.Min(6, rewardedRetryAttempt));

            if (rewardedRetryCoroutine != null) StopCoroutine(rewardedRetryCoroutine);
            rewardedRetryCoroutine = StartCoroutine(WaitAndLoadRewarded(retryDelay));
        }

        private void OnRewardedHiddenCallback(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnRewardedClosed?.Invoke();
            LoadRewarded();
        }

        private void OnAdReceivedRewardCallback(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            OnUserRewarded?.Invoke();
        }
        #endregion

        #region IAdService Implementation
        public void ShowInterstitial()
        {
#if !UNITY_EDITOR
            if (IsInterstitialReady())
            {
                MaxSdk.ShowInterstitial(config.InterstitialAdUnitId);
            }
            else
            {
                LoadInterstitial();
            }
#else
            Debug.Log("Test Interstitial Ad Displayed");
            OnInterstitialClosed?.Invoke();
#endif
        }

        public void ShowRewarded()
        {
#if !UNITY_EDITOR
            if (IsRewardedReady())
            {
                MaxSdk.ShowRewardedAd(config.RewardedAdUnitId);
            }
            else
            {
                LoadRewarded();
            }
#else
            Debug.Log("Test Rewarded Ad Completed. Reward Granted");
            OnUserRewarded?.Invoke();
            OnRewardedClosed?.Invoke();
#endif
        }

        public bool IsInterstitialReady()
        {
#if UNITY_EDITOR
            return true;
#else
            return MaxSdk.IsInterstitialReady(config.InterstitialAdUnitId);
#endif
        }

        public bool IsRewardedReady()
        {
#if UNITY_EDITOR
            return true;
#else
            return MaxSdk.IsRewardedAdReady(config.RewardedAdUnitId);
#endif
        }
        #endregion

        private void LoadInterstitial()
        {
#if !UNITY_EDITOR
            MaxSdk.LoadInterstitial(config.InterstitialAdUnitId);
#endif
        }

        private void LoadRewarded()
        {
#if !UNITY_EDITOR
            MaxSdk.LoadRewardedAd(config.RewardedAdUnitId);
#endif
        }

        #region Optimized Coroutines
        private WaitForSecondsRealtime GetCachedWait(float seconds)
        {
            if (waitCache.TryGetValue(seconds, out var wait)) return wait;
            wait = new WaitForSecondsRealtime(seconds);
            waitCache[seconds] = wait;
            return wait;
        }

        private IEnumerator WaitAndLoadInterstitial(float delay)
        {
            yield return GetCachedWait(delay);
            LoadInterstitial();
        }

        private IEnumerator WaitAndLoadRewarded(float delay)
        {
            yield return GetCachedWait(delay);
            LoadRewarded();
        }
        #endregion

        private void OnDestroy()
        {
            AdServices.Unregister(this);

            if (!Application.isPlaying) return;

            if (interstitialRetryCoroutine != null) StopCoroutine(interstitialRetryCoroutine);
            if (rewardedRetryCoroutine != null) StopCoroutine(rewardedRetryCoroutine);

            MaxSdkCallbacks.OnSdkInitializedEvent -= OnSdkInitialized;
            CleanupAdCallbacks();
        }

        private void CleanupAdCallbacks()
        {
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent -= OnInterstitialLoadedCallback;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent -= OnInterstitialLoadFailedCallback;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent -= OnInterstitialHiddenCallback;

            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent -= OnRewardedLoadedCallback;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= OnRewardedFailedCallback;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent -= OnRewardedHiddenCallback;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= OnAdReceivedRewardCallback;
        }
    }
    
}