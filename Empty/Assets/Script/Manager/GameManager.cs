using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Game을 실행하기 위한 필수 Manager
/// </summary>
[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    // 카메라를 담고 있는 Category
    [SerializeField]
    private CameraCategory cameraCategory;
    
    // Object들을 담을 공간
    [SerializeField]
    private GameObject objectPoolDummy;

    private void Awake()
    {
        // Event Manager와 Material Manager를 생성하고 중재자 Locator에 등록한다.
        EventManager eventManager = new EventManager();
        MaterialManager materialManager = new MaterialManager();
        Locator<MaterialManager>.Provide(materialManager);
        Locator<EventManager>.Provide(eventManager);

        // Loading Scene에서 생성한 Resource Manager를 Locator에서 받아온다.
        var resourceManager = Locator<ResourceManager>.Get();

        // Sound Category를 위해 필요한 준비 과정
        #region Sound Category Setting
        var bgmList = new List<GameObject>();
        var sfxList = new List<GameObject>();

        bgmList.Add(resourceManager.GetResource(ResourceType.BGM, "Stage1"));

        sfxList.Add(resourceManager.GetResource(ResourceType.SFX, "None Swap"));
        sfxList.Add(resourceManager.GetResource(ResourceType.SFX, "Pop"));
        sfxList.Add(resourceManager.GetResource(ResourceType.SFX, "Pop2"));
        sfxList.Add(resourceManager.GetResource(ResourceType.SFX, "Pop3"));
        #endregion

        // Category에 담을 Object들을 넣는 과정
        SoundCategory soundCategory = new SoundCategory(bgmList, sfxList);
        ElementCategory elementCategory = new ElementCategory(resourceManager.GetResource(ResourceType.Default, "Red Element"), resourceManager.GetResource(ResourceType.Default, "Blue Element"), resourceManager.GetResource(ResourceType.Default, "Green Element"), resourceManager.GetResource(ResourceType.Default, "Yellow Element"));
        UIPrefabCategory uiPrefabCategory = new UIPrefabCategory(resourceManager.GetResource(ResourceType.UI, "Title"), resourceManager.GetResource(ResourceType.UI, "Game Board"), resourceManager.GetResource(ResourceType.UI, "Life"), resourceManager.GetResource(ResourceType.UI, "Ranker"), resourceManager.GetResource(ResourceType.UI, "Game Over"));

        // Category에 Object를 넣었다면 Manager들에게 넣는다.
        Factory factory = new Factory(elementCategory, objectPoolDummy);
        UIManager uiManager = new UIManager(uiPrefabCategory);
        SoundManager soundManager = new SoundManager(soundCategory, objectPoolDummy);
        CameraManager cameraManager = new CameraManager(cameraCategory);

        // Locator에 Manager들을 등록한다.
        Locator<Factory>.Provide(factory);
        Locator<CameraManager>.Provide(cameraManager);
        Locator<UIManager>.Provide(uiManager);
        Locator<SoundManager>.Provide(soundManager);

        Debug.Log("Complete GameManager");
    }

    private void Start()
    {
        // Title 화면을 가져온다.
        var uiManager = Locator<UIManager>.Get();
        var title = uiManager.GetUIPrefabObject(UIPrefab.Title);
        title.SetActive(true);

        // Title Sound를 실행한다.
        var soundManager = Locator<SoundManager>.Get();
        soundManager.PlayBGM(BGM.Title);
        Debug.Log("Complete Title Setting");
    }
}

