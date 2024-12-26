using System;
using BubbleShooterGameToolkit.Scripts.Ads.AdUnits;
using UnityEngine;
#if UNITY_ANDROID

using YandexMobileAds;
using YandexMobileAds.Base; 
#endif

namespace BubbleShooterGameToolkit.Scripts.Ads.Networks
{
    [CreateAssetMenu(fileName = "YandexHandler", menuName = "BubbleShooterGameToolkit/Ads/YandexHandler")]
    public class YandexHandler : AdsHandlerBase
    {
        private IAdsListener _listener;
#if UNITY_ANDROID
        private Interstitial _interstitialAd;
        private RewardedAd _rewardedAd;
        InterstitialAdLoader interstitialAdLoader;
        RewardedAdLoader rewardedAdLoader;
#endif

        public override void Init(string _id, bool adSettingTestMode, IAdsListener listener)
        {
#if UNITY_ANDROID
            _listener = listener;

            interstitialAdLoader = new InterstitialAdLoader();
            interstitialAdLoader.OnAdLoaded += HandleInterstitialLoaded;
            interstitialAdLoader.OnAdFailedToLoad += HandleInterstitialFailedToLoad;


            rewardedAdLoader = new RewardedAdLoader();
            rewardedAdLoader.OnAdLoaded += HandleAdLoaded;
            rewardedAdLoader.OnAdFailedToLoad += HandleAdFailedToLoad;

            _listener?.OnAdsInitialized();

#endif
        }

        public override void Show(AdUnit adUnit)
        {
#if UNITY_ANDROID
            _listener?.Show(adUnit);
            Debug.Log(adUnit.AdReference.adType);
            if (adUnit.AdReference.adType == EAdType.Interstitial)
            {
                if (_interstitialAd != null)
                {
                    Debug.Log("Showing interstitial ad.");
                    _interstitialAd.Show();
                }
                else
                {
                    Debug.LogError("Interstitial ad is not ready yet.");
                }
            }
            else if (adUnit.AdReference.adType == EAdType.Rewarded)
            {
                if (_rewardedAd != null)
                {
                    Debug.Log("Showing rewarded ad.");
                    _rewardedAd.Show();
                }
                else
                {
                    Debug.LogError("Rewarded ad is not ready yet.");
                }
            }
#endif
        }
#if UNITY_ANDROID
        public void HandleAdLoaded(object sender, RewardedAdLoadedEventArgs args)
        {

            args.RewardedAd.OnAdShown += (x, y) => _listener.OnAdsShowComplete();
            Debug.Log("Rewarded ad loaded with response : " + args.RewardedAd.GetInfo());
            _listener?.OnAdsLoaded(args.RewardedAd.GetInfo().AdUnitId);
            // Rewarded ad was loaded successfully. Now you can handle it.
            _rewardedAd = args.RewardedAd;
        }
#endif

#if UNITY_ANDROID
        public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {

            Debug.LogError("Rewarded ad failed to load an ad with error : " + args.Message);
            // Ad {args.AdUnitId} failed for to load with {args.Message}
            // Attempting to load new ad from the OnAdFailedToLoad event is strongly discouraged.
        }
#endif
#if UNITY_ANDROID
        public void HandleInterstitialLoaded(object sender, InterstitialAdLoadedEventArgs args)
        {
            Debug.Log("Interstitial ad loaded with response : " + args.Interstitial.GetInfo());

            args.Interstitial.OnAdShown += (object sender, EventArgs args) => _listener.OnAdsShowComplete();

            _listener?.OnAdsLoaded(args.Interstitial.GetInfo().AdUnitId);
            // The ad was loaded successfully. Now you can handle it.
            _interstitialAd = args.Interstitial;
        }
#endif

#if UNITY_ANDROID
        public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.LogError("Interstitial ad failed to load an ad with error : " + args.Message);
            // Ad {args.AdUnitId} failed for to load with {args.Message}
            // Attempting to load a new ad from the OnAdFailedToLoad event is strongly discouraged.
        }
#endif
        public override void Load(AdUnit adUnit)
        {
#if UNITY_ANDROID
            if (adUnit.AdReference.adType == EAdType.Interstitial)
            {
                AdRequestConfiguration adRequestConfiguration = new AdRequestConfiguration.Builder(adUnit.PlacementId).Build();
                interstitialAdLoader.LoadAd(adRequestConfiguration);
            }
            else if (adUnit.AdReference.adType == EAdType.Rewarded)
            {
                AdRequestConfiguration adRequestConfiguration = new AdRequestConfiguration.Builder(adUnit.PlacementId).Build();
                rewardedAdLoader.LoadAd(adRequestConfiguration);
            }
#endif
        }

        public override bool IsAvailable(AdUnit adUnit)
        {
            return false;
        }
    }
}