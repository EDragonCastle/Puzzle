using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using System;
using UnityEngine.AddressableAssets;


public class TestLoadingScene : MonoBehaviour
{
    [SerializeField]
    private Slider loadingBar;
    [SerializeField]
    private TMP_Text loadingBarText;
    [SerializeField]
    private TMP_Text loadingText;
    [SerializeField]
    private InitalizeSetting setting;

    private async void Start()
    {
        // Loading Scene에서 시작
        await LoadingScene();
    }

    // 무엇을 원하는지 알아야 해.

    private async UniTask LoadingScene()
    {
        // Scene을 Loading하기 위해 IProgress를 만든다.
        IProgress<float> progress = new Progress<float>(p =>
        {
            if (loadingBar != null)
                loadingBar.value = p;

            if (loadingBarText != null)
                loadingBarText.text = $"{(int)(p * 100)}%";
        });

        if (loadingText != null)
            loadingText.text = "Loading Resource..";

        if (setting != null)
        {
            await setting.Load(progress); 
        }
        else
        {
            Debug.LogError("InitSetting not found in the Loaded Scene");
        }
     
        progress.Report(1.0f);
        await UniTask.Delay(TimeSpan.FromSeconds(1f));

        progress.Report(0f);

        if (loadingText != null)
            loadingText.text = "Loading Scene..";

        var sceneHandle = Addressables.LoadSceneAsync("Bubble Pop", UnityEngine.SceneManagement.LoadSceneMode.Single, activateOnLoad: false);

        while (!sceneHandle.IsDone)
        {
            // PercentComplete는 로딩의 전체 진행률을 나타냅니다.
            progress.Report(sceneHandle.PercentComplete); 
            await UniTask.Yield();
        }

        progress.Report(1.0f);
        await UniTask.Yield();

        await sceneHandle.Result.ActivateAsync();
    }
}
