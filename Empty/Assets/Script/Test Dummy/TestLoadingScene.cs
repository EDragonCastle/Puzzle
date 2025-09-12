using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using System;


public class TestLoadingScene : MonoBehaviour
{
    [SerializeField]
    private Slider loadingBar;
    [SerializeField]
    private TMP_Text loadingText;


    private async void Start()
    {
        // Loading Scene에서 시작
        
    }

    // 무엇을 원하는지 알아야 해.

    private async UniTask LoadingScene()
    {
        var sceneLoadingOperation = SceneManager.LoadSceneAsync("Bubble Pop");
        sceneLoadingOperation.allowSceneActivation = false;

    }
}
