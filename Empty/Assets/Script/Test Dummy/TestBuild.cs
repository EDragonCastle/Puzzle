using UnityEngine;
using System.Collections.Generic;

public class TestBuild : MonoBehaviour
{
    // ������ �� ������ ����� ������ �� ���� ���� �� ����.

    private async void Awake()
    {
        // �������δ� �ַ� 3����.

        // Addressable Test (Resource�� ���õǾ��� ������ �� ���� �����ϱ�? �����ĺ�)
        // �Ƹ� ����ٸ� Addressable.LoadAssetAsync���� ã�ٰ� ��� ���߰���.
        ResourceManager resourceManager = new ResourceManager();
        await resourceManager.Load();

        // ���� ������� �� ������ �� ���� �ִ�.
        var bgmList = new List<GameObject>();
        var sfxList = new List<GameObject>();

        bgmList.Add(resourceManager.GetResource(ResourceType.BGM, "Stage1"));

        sfxList.Add(resourceManager.GetResource(ResourceType.SFX, "None Swap"));
        sfxList.Add(resourceManager.GetResource(ResourceType.SFX, "Pop"));
        sfxList.Add(resourceManager.GetResource(ResourceType.SFX, "Pop2"));
        sfxList.Add(resourceManager.GetResource(ResourceType.SFX, "Pop3"));

        // AD Manager (Package�� ����?)
        //var adManager = new AdManager();

        // EDC Server (Async�� ����?)
        //var server = new EDCServer();
        //await server.InitalizeFirebase();
    }
}
