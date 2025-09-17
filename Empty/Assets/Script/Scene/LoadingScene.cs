using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using System;
using UnityEngine.AddressableAssets;

/// <summary>
/// Loading Scene을 담당하는 Class
/// </summary>
public class LoadingScene : MonoBehaviour
{
    // Loading Bar와 어떤 값이 Load 되고 있는지 
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
        await Initalize();
    }

    /// <summary>
    /// Loading 하는 함수
    /// </summary>
    /// <returns></returns>
    private async UniTask Initalize()
    {
        // Scene을 Loading하기 위해 IProgress를 만든다.
        IProgress<float> progress = new Progress<float>(p =>
        {
            if (loadingBar != null)
                loadingBar.value = p;

            if (loadingBarText != null)
                loadingBarText.text = $"{(int)(p * 100)}%";
        });

        // Resource를 Loading 하고 있다고 알려주자.
        if (loadingText != null)
            loadingText.text = "Loading Resource..";

        // setting을 null check 한다.
        if (setting != null)
        {
            // Resource를 Loading한다.
            await setting.Load(progress);
        }
        else
        {
            Debug.LogError("InitSetting not found in the Loaded Scene");
        }

        // Resource Loading이 끝났다면 잠시 뒤에 Scene을 Load 하도록 준비한다.
        progress.Report(1.0f);
        await UniTask.Delay(TimeSpan.FromSeconds(1f));

        progress.Report(0f);

        if (loadingText != null)
            loadingText.text = "Loading Scene..";

        // Addressable Asset에서 다음 Scene을 Load 할 준비를 한다.
        var sceneHandle = Addressables.LoadSceneAsync("Bubble Pop", UnityEngine.SceneManagement.LoadSceneMode.Single, activateOnLoad: false);

        // Scene이 Load가 될 때까지 Loop를 돈다.
        while (!sceneHandle.IsDone)
        {
            // PercentComplete는 로딩의 전체 진행률을 나타낸다.
            progress.Report(sceneHandle.PercentComplete);
            await UniTask.Yield();
        }

        // Load가 끝나면 다음 Frame에 Scene으로 이동한다.
        progress.Report(1.0f);
        await UniTask.Yield();

        await sceneHandle.Result.ActivateAsync();
    }
}
