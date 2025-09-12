using UnityEngine;
using System.Collections.Generic;

public class TestBuild : MonoBehaviour
{
    // 시작할 때 문제가 생기니 시작할 때 뭔가 있을 것 같다.

    private async void Awake()
    {
        // 원인으로는 주로 3가지.

        // Addressable Test (Resource와 관련되었기 때문에 이 곳이 문제일까? 유력후보)
        // 아마 멈춘다면 Addressable.LoadAssetAsync에서 찾다가 계속 멈추겠지.
        ResourceManager resourceManager = new ResourceManager();
        await resourceManager.Load();

        // 실제 사용했을 때 문제가 될 수도 있다.
        var bgmList = new List<GameObject>();
        var sfxList = new List<GameObject>();

        bgmList.Add(resourceManager.GetResource(ResourceType.BGM, "Stage1"));

        sfxList.Add(resourceManager.GetResource(ResourceType.SFX, "None Swap"));
        sfxList.Add(resourceManager.GetResource(ResourceType.SFX, "Pop"));
        sfxList.Add(resourceManager.GetResource(ResourceType.SFX, "Pop2"));
        sfxList.Add(resourceManager.GetResource(ResourceType.SFX, "Pop3"));

        // AD Manager (Package가 문제?)
        //var adManager = new AdManager();

        // EDC Server (Async가 문제?)
        //var server = new EDCServer();
        //await server.InitalizeFirebase();
    }
}
