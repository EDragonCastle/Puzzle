using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestResourceManager : MonoBehaviour
{
    ResourceManager resourceManager;

    private GameObject prefabObject;
    private GameObject testObject;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Memory Load");
        InitalizeSetting();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Object Instaniate");
            prefabObject = resourceManager.GetResource(ResourceType.BGM, "Stage1");

            if(prefabObject != null)
                testObject = Instantiate(prefabObject);
        }

        // Clone�� �ϸ� �� �ǰ�, ������ �ؾ� �ϴµ� ������ ���?
        // ���纻���� �ݳ��ϸ� ���� �� �� ��� ������ ������ �ݳ��ؾ� �Ѵٴ� ����� �˾Ҵ�.
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Memory Release");
            resourceManager.ReleaseResource(prefabObject);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Object Destory");
            Destroy(testObject);
        }
    }

    private void InitalizeSetting()
    {
        resourceManager = new ResourceManager();


        // Sound Category Setting
        var bgmList = new List<GameObject>();
        var sfxList = new List<GameObject>();

        bgmList.Add(resourceManager.GetResource(ResourceType.BGM, "Stage1"));

        sfxList.Add(resourceManager.GetResource(ResourceType.SFX, "None Swap"));
        sfxList.Add(resourceManager.GetResource(ResourceType.SFX, "Pop"));
        sfxList.Add(resourceManager.GetResource(ResourceType.SFX, "Pop2"));
        sfxList.Add(resourceManager.GetResource(ResourceType.SFX, "Pop3"));

        SoundCategory soundCategory = new SoundCategory(bgmList, sfxList);

        // Element Category Setting
        ElementCategory elementCategory = new ElementCategory(resourceManager.GetResource(ResourceType.Default, "Red Element"),
                                                              resourceManager.GetResource(ResourceType.Default, "Blue Element"),
                                                              resourceManager.GetResource(ResourceType.Default, "Green Element"),
                                                              resourceManager.GetResource(ResourceType.Default, "Yellow Element"));

        // UI Prefab Setting
        UIPrefabCategory uIPrefabCategory = new UIPrefabCategory(resourceManager.GetResource(ResourceType.UI, "Title"),
                                                                 resourceManager.GetResource(ResourceType.UI, "Game Board"),
                                                                 resourceManager.GetResource(ResourceType.UI, "Life"),
                                                                 resourceManager.GetResource(ResourceType.UI, "Ranker"),
                                                                 resourceManager.GetResource(ResourceType.UI, "Game Over"));

        Debug.Log("Init Setting Complete");
    }

}
