using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using System;
using UnityEngine.AddressableAssets;

/// <summary>
/// Loading Scene�� ����ϴ� Class
/// </summary>
public class LoadingScene : MonoBehaviour
{
    // Loading Bar�� � ���� Load �ǰ� �ִ��� 
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
        // Loading Scene���� ����
        await Initalize();
    }

    /// <summary>
    /// Loading �ϴ� �Լ�
    /// </summary>
    /// <returns></returns>
    private async UniTask Initalize()
    {
        // Scene�� Loading�ϱ� ���� IProgress�� �����.
        IProgress<float> progress = new Progress<float>(p =>
        {
            if (loadingBar != null)
                loadingBar.value = p;

            if (loadingBarText != null)
                loadingBarText.text = $"{(int)(p * 100)}%";
        });

        // Resource�� Loading �ϰ� �ִٰ� �˷�����.
        if (loadingText != null)
            loadingText.text = "Loading Resource..";

        // setting�� null check �Ѵ�.
        if (setting != null)
        {
            // Resource�� Loading�Ѵ�.
            await setting.Load(progress);
        }
        else
        {
            Debug.LogError("InitSetting not found in the Loaded Scene");
        }

        // Resource Loading�� �����ٸ� ��� �ڿ� Scene�� Load �ϵ��� �غ��Ѵ�.
        progress.Report(1.0f);
        await UniTask.Delay(TimeSpan.FromSeconds(1f));

        progress.Report(0f);

        if (loadingText != null)
            loadingText.text = "Loading Scene..";

        // Addressable Asset���� ���� Scene�� Load �� �غ� �Ѵ�.
        var sceneHandle = Addressables.LoadSceneAsync("Bubble Pop", UnityEngine.SceneManagement.LoadSceneMode.Single, activateOnLoad: false);

        // Scene�� Load�� �� ������ Loop�� ����.
        while (!sceneHandle.IsDone)
        {
            // PercentComplete�� �ε��� ��ü ������� ��Ÿ����.
            progress.Report(sceneHandle.PercentComplete);
            await UniTask.Yield();
        }

        // Load�� ������ ���� Frame�� Scene���� �̵��Ѵ�.
        progress.Report(1.0f);
        await UniTask.Yield();

        await sceneHandle.Result.ActivateAsync();
    }
}
