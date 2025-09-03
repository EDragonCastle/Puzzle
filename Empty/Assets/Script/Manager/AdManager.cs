using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.UI;
using System;
public class AdManager : MonoBehaviour
{

#if UNITY_ANDROID
    private string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
    private string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
    private string adUnitId = "unused";
#endif

    #region Banner

    private BannerView bannerView;
    [SerializeField]
    private GameObject closeButtonObject;

    private AdPosition currentAdPosition;

    public void CreateBannerView()
    {
        Debug.Log("Createing Banner View");

        if (bannerView != null)
        {
            DestoryBanner();
        }

        currentAdPosition = AdPosition.Top;
        bannerView = new BannerView(adUnitId, AdSize.Banner, currentAdPosition);

        // ���� ũ�� ����

        var adSize = new AdSize(200, 250);
        bannerView = new BannerView(adUnitId, adSize, AdPosition.Top);


        // ���� ��ġ ����
        // bannerView = new BannerView(adUnitId, AdSize.Banner, 0, 50);

        ListenToAdEvents();

        LoadAd();
    }

    public void LoadAd()
    {
        if (bannerView == null)
        {
            CreateBannerView();
        }

        var adRequest = new AdRequest();

        Debug.Log("Loading Banner AD");
        bannerView.LoadAd(adRequest);
    }

    private void ListenToAdEvents()
    {
        // ���� ���������� Load ���� ��
        bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view Loaded ad with reponse" + bannerView.GetResponseInfo());

            // �� �ʺ�� ���̸� ������ �ݱ� ��ư�� ������ �̵��ϰ� ����.
            float loadAdWidthSize = bannerView.GetWidthInPixels();
            float loadAdHeightSize = bannerView.GetHeightInPixels();

            if (closeButtonObject != null)
            {
                closeButtonObject.SetActive(true);
                // ��ġ ����
            }
        };

        // ���� Load ���� �ʾ��� ��
        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.Log("Banner view Failed to load ad with error" + error);
            if (closeButtonObject != null)
                closeButtonObject.SetActive(false);
        };

        // ad�� recorded impression ��, 
        bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"Banner view paid {adValue.Value} {adValue.CurrencyCode}");
        };

        bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recoreded impression");
        };

        // ad Ŭ������ ��
        bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was Clicked");
        };

        bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened");
        };

        bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed");
            if (closeButtonObject != null)
            {
                closeButtonObject.SetActive(true);
            }
        };
    }

    public void DestoryBanner()
    {
        if (bannerView != null)
        {
            Debug.Log("Destory Banner view");
            bannerView.Destroy();
            bannerView = null;
            if (closeButtonObject != null)
            {
                closeButtonObject.SetActive(false);
            }
        }
    }

    private void AdjustCloseButtonPosition(float width, float height, AdPosition position)
    {
        if (closeButtonObject == null)
        {
            Debug.Log("close Button Object None Exist");
            return;
        }
        var buttonRectTransform = closeButtonObject.GetComponent<RectTransform>();
        if (buttonRectTransform == null)
        {
            Debug.Log("Button Object not has RectTransform Component");
            return;
        }

        var canvasScaler = closeButtonObject.GetComponentInParent<CanvasScaler>();
        if (canvasScaler == null)
        {
            Debug.Log("None Exist CanvasScaler Component in closeButton Object");
            return;
        }

        // �ʺ�� ���̸� ������ ��ư�� ��ġ�� ������ �� �ִ� ����� ���������� �����ؾ� �Ѵ�
        // ��������.. �׳� ���� ��ġ�� �� ������ true false�� �ϴ� �͵� ����̾�.
    }

    #endregion

    #region Interstitial

    private InterstitialAd interstitialAd;

    private void LoadInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        Debug.Log("Loading Intersitial AD");

        var adRequest = new AdRequest();

        InterstitialAd.Load(adUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError($"interstital ad failed to load ad with error : {error}");
                return;
            }
            Debug.Log($"Insterstital ad load with response : {ad.GetResponseInfo()}");

            interstitialAd = ad;
            RegisterReloadHandler(interstitialAd);

            ShowInterstitialAd();
        });
    }

    private void ShowInterstitialAd()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstital ad.");
            interstitialAd.Show();
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet");
            LoadInterstitialAd();
        }
    }

    private void RegisterReloadHandler(InterstitialAd registerAd)
    {
        registerAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial Ad Full Screen content closed");
            //LoadInterstitialAd();
        };

        registerAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError($"Interstitial ad failed to open full screen content with error : {error}");
            LoadInterstitialAd();
        };

        registerAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("���� ���� ��ü ȭ�� ������ ����");
        };

        registerAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("���� ���� ���� ��ϵ�");
        };

        registerAd.OnAdClicked += () =>
        {
            Debug.Log("���� ���� Ŭ����");
        };
    }


    #endregion

    #region Native / Native Overlay : Version 9.0.0 Requirements
    // https://developers.google.com/admob/unity/native?hl=ko
    // ���� ���� Unity�� ����� ���� Native Plugin�� ������ �ߴܵǾ���.
    // ��ſ� Native Overlay�� ���� ����ؾ� �Ѵ�.

    private NativeOverlayAd nativeOverlayAd;

    // Plugin�� version�̶� ���� �ʾƼ� �Ұ����ϴ�. ���� GoogleUmpDependencies.xml �� ����
    // version�� �䱸 version���� �ξ� ���Ƽ� �ش� ad�� ����� �Ұ����ϴ�.
    public void LoadNativeOverlayAd()
    {
        if (nativeOverlayAd != null)
            DestoryNativeOverlayAd();

        Debug.Log("Loading Native Overlay Ad");

        var adRequest = new AdRequest();

        var options = new NativeAdOptions
        {
            AdChoicesPlacement = AdChoicesPlacement.TopRightCorner,
            MediaAspectRatio = MediaAspectRatio.Any,
        };

        NativeOverlayAd.Load(adUnitId, adRequest, options,
            (NativeOverlayAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    Debug.LogError($"Native Overlay ad failed to load ad with error : {error}");
                    return;
                }

                if (ad == null)
                {
                    Debug.LogError("Unexpected error : Native Overlay ad load event fired with null ad and null error");
                    return;
                }

                Debug.Log($"Native Overlay ad loaded with response : {ad.GetResponseInfo()}");
                nativeOverlayAd = ad;
                RegisterNativeAd(nativeOverlayAd);
                RenderNativeOverlayAd();
            });
    }

    private void RegisterNativeAd(NativeOverlayAd registerAd)
    {
        registerAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("NativeOverlay Ad Full Screen content closed");
        };

        registerAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Native Overlay AD�� ����Ǿ���.");
        };

        registerAd.OnAdClicked += () =>
        {
            Debug.Log("Native AD�� Ŭ�� �Ǿ����ϴ�.");
        };

        registerAd.OnAdPaid += (AdValue value) =>
        {
            Debug.Log($"{value}��ŭ ����Ǿ���.");
        };

        registerAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("��ü ȭ������ Content�� ��µǰ� �ִ�.");
        };
    }

    private void RenderNativeOverlayAd()
    {
        if (nativeOverlayAd != null)
        {
            Debug.Log("Rendering Native Ad");

            var style = new NativeTemplateStyle
            {
                TemplateId = NativeTemplateId.Medium,
                MainBackgroundColor = Color.red,
                CallToActionText = new NativeTemplateTextStyle
                {
                    BackgroundColor = Color.green,
                    FontSize = 9,
                    TextColor = Color.white,
                    Style = NativeTemplateFontStyle.Bold
                }
            };

            nativeOverlayAd.RenderTemplate(style, AdPosition.Bottom);
        }
    }

    private void ShowNativeOverlayAd()
    {
        if (nativeOverlayAd != null)
        {
            Debug.Log("Show Native overlay ad");
            nativeOverlayAd.Show();
        }
    }

    private void HideNativeOverlayAd()
    {
        if (nativeOverlayAd != null)
        {
            Debug.Log("Hiding Native Overlay Ad");
            nativeOverlayAd.Hide();
        }
    }

    private void DestoryNativeOverlayAd()
    {
        if (nativeOverlayAd != null)
        {
            Debug.Log("Destory Native Overlay Ad");
            nativeOverlayAd.Destroy();
            nativeOverlayAd = null;
        }
    }


    #endregion

    #region Reward 

    private RewardedAd rewardedAd;
    InitializationStatus initstatus;

    private void Initalize()
    {
        MobileAds.Initialize((InitializationStatus _initstatus) =>
        {
            if (_initstatus == null)
            {
                Debug.LogError("Google Mobile Ads initialization failed.");
                return;
            }

            initstatus = _initstatus;
        });
    }

    public void LoadRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        Debug.Log("Loading the Rewarded ad");

        var adRequest = new AdRequest();

        adUnitId = "ca-app-pub-3940256099942544/5224354917";

        RewardedAd.Load(adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError($"Reward an failed to load Ad with {error}");
                    return;
                }
                Debug.Log($"Reward ad load with reponse : {ad.GetResponseInfo()}");

                // ���� �� Ȯ�� �� SSV Callback �˻�
                // UserId�� ��¥ UserId�� �ְ�, CustomData���� ���� �� �� UserId�� �ִ´�.
                var options = new ServerSideVerificationOptions()
                {
                    UserId = adUnitId,
                    CustomData = Guid.NewGuid().ToString(),
                };

                ad.SetServerSideVerificationOptions(options);
                rewardedAd = ad;
                RegisterRewardAd(rewardedAd);
            });
    }

    public bool ShowRewardedAd(IReward rewardInterface)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"Rewarded ad reward the user. Type: {reward.Type}, Amout: {reward.Amount}");
                // ���⿡ ������ ������ �ȴ�.
                rewardInterface?.HandleReward();
            });
        }
        else
        {
            Debug.LogError("Ad�� �غ���� �ʾҴ�.");
            return false;
        }

        return true;
    }


    private void RegisterRewardAd(RewardedAd rewardAd)
    {
        rewardAd.OnAdPaid += (AdValue value) =>
        {
            Debug.Log($"Reward ad paid {value.Value} {value.CurrencyCode}");
        };

        rewardAd.OnAdClicked += () =>
        {
            Debug.Log("Reward ad clicked");
        };

        rewardAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };

        rewardAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };

        rewardAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");

            var soundManager = Locator<SoundManager>.Get();
            soundManager.PlayBGM(BGM.Title);

            LoadRewardedAd();
        };

        rewardAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError($"Rewarded ad failed to open full screen content with error {error}");
            LoadRewardedAd();
        };
    }

    #endregion

    #region Reward Interstitial

    private RewardedInterstitialAd rewardedInterstitialAd;

    public void LoadRewardedInterstitialAd()
    {
        if (rewardedInterstitialAd != null)
        {
            rewardedInterstitialAd.Destroy();
            rewardedInterstitialAd = null;
        }

        Debug.Log("Loading the rewarded interstitial ad.");

        var adRequest = new AdRequest();

        adUnitId = "ca-app-pub-3940256099942544/5224354917";
        //adRequest.Keywords.Add("unity-admob-sample");

        RewardedInterstitialAd.Load(adUnitId, adRequest,
            (RewardedInterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError($"rewarded interstitial ad failed to load an ad with error: {error}");
                    return;
                }

                Debug.Log($"Reward Interstitial ad Loaded with Response: {ad.GetResponseInfo()}");
                var options = new ServerSideVerificationOptions
                {
                    UserId = adUnitId,
                    CustomData = Guid.NewGuid().ToString()
                };

                ad.SetServerSideVerificationOptions(options);
                rewardedInterstitialAd = ad;
            });
    }

    private void ShowRewardInterstitialAd()
    {
        if (rewardedInterstitialAd != null && rewardedInterstitialAd.CanShowAd())
        {
            rewardedInterstitialAd.Show((Reward reward) =>
            {
                Debug.Log($"Rewarded interstitial ad reward the user. Type: {reward.Type}, Amount: {reward.Amount}");
            });
        }
    }

    private void RegisterRewardInterstitialAd(RewardedInterstitialAd ad)
    {
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"Rewarded interstitial ad paid {adValue.Value} {adValue.CurrencyCode}.");
        };

        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded interstitial ad recorded an impression.");
        };

        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded interstitial ad was clicked.");
        };

        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded interstitial ad full screen content opened.");
        };

        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded interstitial ad full screen content closed.");
            LoadRewardedInterstitialAd();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError($"Rewarded interstitial ad failed to open full screen content with error : {error}");
            LoadRewardedInterstitialAd();
        };
    }

    #endregion

    #region App Opening : Version 7.1.0 Requirements

    private AppOpenAd appOpenAd;

    private void LoadAppOpenAd()
    {
        if (appOpenAd != null)
        {
            appOpenAd.Destroy();
            appOpenAd = null;
        }

        Debug.Log("Loading the app open ad");

        var adRequest = new AdRequest();

        AppOpenAd.Load(adUnitId, adRequest,
            (AppOpenAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError($"App open ad Failed to load ad with error : {error}");
                    return;
                }

                Debug.Log($"App open AD loaded with response: {ad.GetResponseInfo()}");
                appOpenAd = ad;
                RegisterEventHandlers(appOpenAd);
            });
    }

    private void RegisterEventHandlers(AppOpenAd ad)
    {
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"App open ad paid {adValue.Value} {adValue.CurrencyCode}.");
        };

        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("App open ad recorded an impression.");
        };

        ad.OnAdClicked += () =>
        {
            Debug.Log("App open ad was clicked.");
        };

        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("App open ad full screen content opened.");
        };

        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("App open ad full screen content closed.");
            LoadAppOpenAd();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError($"App open ad failed to open full screen content with error : {error}");
            LoadAppOpenAd();
        };
    }

    private void ShowAppOpenAd()
    {
        if (appOpenAd != null && appOpenAd.CanShowAd())
        {
            Debug.Log("Showing app open ad.");
            appOpenAd.Show();
        }
        else
        {
            Debug.LogError("App open ad is not ready yet.");
        }
    }

    #endregion

    private void Awake()
    {
        Initalize();
    }
}

public enum ADType
{
    Banner,
    Interstitial,
    Reward,
    RewardInterstitial,
}

public enum RewardType
{
    Gold,
    Score,
}