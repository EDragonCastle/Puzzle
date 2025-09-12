using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private CameraCategory cameraCategory;
    [SerializeField]
    private GameObject objectPoolDummy;


    public async UniTask Initalize()
    {
        // Manager constuctor
        EventManager eventManager = new EventManager();
        MaterialManager materialManager = new MaterialManager();
        Locator<MaterialManager>.Provide(materialManager);
        Locator<EventManager>.Provide(eventManager);

        ResourceManager resourceManager = new ResourceManager();
        await resourceManager.Load();

        #region Sound Category Setting
        var bgmList = new List<GameObject>();
        var sfxList = new List<GameObject>();

        bgmList.Add(resourceManager.GetResource(ResourceType.BGM, "Stage1"));

        sfxList.Add(resourceManager.GetResource(ResourceType.SFX, "None Swap"));
        sfxList.Add(resourceManager.GetResource(ResourceType.SFX, "Pop"));
        sfxList.Add(resourceManager.GetResource(ResourceType.SFX, "Pop2"));
        sfxList.Add(resourceManager.GetResource(ResourceType.SFX, "Pop3"));
        #endregion
        // Category Setting
        SoundCategory soundCategory = new SoundCategory(bgmList, sfxList);
        ElementCategory elementCategory = new ElementCategory(resourceManager.GetResource(ResourceType.Default, "Red Element"), resourceManager.GetResource(ResourceType.Default, "Blue Element"), resourceManager.GetResource(ResourceType.Default, "Green Element"), resourceManager.GetResource(ResourceType.Default, "Yellow Element"));
        UIPrefabCategory uiPrefabCategory = new UIPrefabCategory(resourceManager.GetResource(ResourceType.UI, "Title"), resourceManager.GetResource(ResourceType.UI, "Game Board"), resourceManager.GetResource(ResourceType.UI, "Life"), resourceManager.GetResource(ResourceType.UI, "Ranker"), resourceManager.GetResource(ResourceType.UI, "Game Over"));

        Factory factory = new Factory(elementCategory, objectPoolDummy);
        UIManager uiManager = new UIManager(uiPrefabCategory);
        SoundManager soundManager = new SoundManager(soundCategory, objectPoolDummy);
        CameraManager cameraManager = new CameraManager(cameraCategory);
        AdManager adManager = new AdManager();

        EDCServer server = new EDCServer();
        await server.InitalizeFirebase();

        // Provide Locator
        Locator<Factory>.Provide(factory);
        Locator<CameraManager>.Provide(cameraManager);
        Locator<UIManager>.Provide(uiManager);
        Locator<EDCServer>.Provide(server);
        Locator<SoundManager>.Provide(soundManager);
        Locator<AdManager>.Provide(adManager);

        Debug.Log("Complete GameManager");
    }

    private void Start()
    {
       var uiManager = Locator<UIManager>.Get();
       var title = uiManager.GetUIPrefabObject(UIPrefab.Title);
       title.SetActive(true);
       var soundManager = Locator<SoundManager>.Get();
       soundManager.PlayBGM(BGM.Title);
       Debug.Log("Complete Title Setting");
    }
}

