using UnityEngine;
using GoogleMobileAds.Api;

/// <summary>
/// 하단 전체에 맞춘 Adaptive Banner 광고
/// </summary>
public class BannerAd : MonoBehaviour
{
#if UNITY_ANDROID
    private string bannerAdId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
    private string bannerAdId = "ca-app-pub-3940256099942544/2934735716";
#else
    private string bannerAdId = "unused";
#endif

    private BannerView bannerView;

    void Start()
    {
        MobileAds.Initialize(initStatus =>
        {
            RequestAdaptiveBanner();
        });
    }

    /// <summary>
    /// 기기 너비에 맞는 하단 배너 광고 요청
    /// </summary>
    void RequestAdaptiveBanner()
    {
        // 현재 디바이스의 전체 너비에 맞춰 배너 크기 계산
        int adWidth = AdSize.FullWidth;
        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(adWidth);

        // 배너 생성: 하단에 위치
        bannerView = new BannerView(bannerAdId, adaptiveSize, AdPosition.Bottom);

        // 광고 요청 후 로드
        AdRequest request = new AdRequest();
        bannerView.LoadAd(request);
    }

    void OnDestroy()
    {
        bannerView?.Destroy();
    }
}
