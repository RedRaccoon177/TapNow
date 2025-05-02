using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.Events;

/// <summary>
/// AdMob 광고 제어 클래스 (전면 광고 및 보상형 광고 관리)
/// - 앱 시작 시 광고 초기화 및 로드
/// - 필요한 시점에서 광고 표시 및 콜백 처리
/// </summary>
public class AdManager : MonoBehaviour
{
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

#if UNITY_ANDROID
    private string interstitialAdId = "ca-app-pub-7844558573816016/4456931359";
    private string rewardedAdId = "ca-app-pub-7844558573816016/7286049335";
#elif UNITY_IPHONE
    private string interstitialAdId = "ca-app-pub-3940256099942544/4411468910";
    private string rewardedAdId = "ca-app-pub-3940256099942544/1712485313";
#else
    private string interstitialAdId = "unused";
    private string rewardedAdId = "unused";
#endif

    private UnityAction interstitialCallback;
    private UnityAction rewardedCallback;

    void Start()
    {
        MobileAds.Initialize(initStatus => { });
        LoadInterstitial();
        LoadRewarded();
    }

    /// <summary>
    /// 전면 광고 표시 요청
    /// </summary>
    public void ShowInterstitial(UnityAction onAdClosed)
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialCallback = onAdClosed;
            interstitialAd.Show();
        }
        else
        {
            Debug.Log("전면 광고 준비되지 않음, 콜백 바로 호출");
            onAdClosed?.Invoke();
        }
    }

    /// <summary>
    /// 전면 광고 로드 및 이벤트 등록
    /// </summary>
    private void LoadInterstitial()
    {
        AdRequest request = new AdRequest();

        InterstitialAd.Load(interstitialAdId, request, (ad, error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("전면 광고 로드 실패: " + error);
                return;
            }

            interstitialAd = ad;

            ad.OnAdFullScreenContentClosed += () =>
            {
                interstitialCallback?.Invoke();
                LoadInterstitial();
            };

            ad.OnAdFullScreenContentFailed += (adError) =>
            {
                Debug.LogError("전면 광고 열기 실패: " + adError);
                LoadInterstitial();
            };
        });
    }

    /// <summary>
    /// 보상형 광고 표시 요청
    /// </summary>
    public void ShowRewarded(UnityAction onRewarded)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedCallback = onRewarded;
            rewardedAd.Show((reward) =>
            {
                Debug.Log("보상 지급: " + reward.Type + " x" + reward.Amount);
                rewardedCallback?.Invoke();
            });
        }
        else
        {
            Debug.Log("보상형 광고 준비되지 않음");
        }
    }

    /// <summary>
    /// 보상형 광고 로드 및 이벤트 등록
    /// </summary>
    private void LoadRewarded()
    {
        AdRequest request = new AdRequest();

        RewardedAd.Load(rewardedAdId, request, (ad, error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("보상형 광고 로드 실패: " + error);
                return;
            }

            rewardedAd = ad;

            ad.OnAdFullScreenContentClosed += () =>
            {
                LoadRewarded();
            };

            ad.OnAdFullScreenContentFailed += (adError) =>
            {
                Debug.LogError("보상형 광고 열기 실패: " + adError);
                LoadRewarded();
            };
        });
    }
}
