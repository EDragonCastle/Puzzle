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
        // Loading Scene���� ����
        
    }

    // ������ ���ϴ��� �˾ƾ� ��.

    private async UniTask LoadingScene()
    {
        var sceneLoadingOperation = SceneManager.LoadSceneAsync("Bubble Pop");
        sceneLoadingOperation.allowSceneActivation = false;

    }
}
