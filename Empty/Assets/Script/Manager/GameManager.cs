using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private ElementCategory elementCategory;
    [SerializeField]
    private UIPrefabCategory uiPrefabCategory;
    [SerializeField]
    private CameraCategory cameraCategory;
    [SerializeField]
    private GameObject objectPoolDummy;
    

    private async void Awake()
    {
        // Manager constuctor
        MaterialManager materialManager = new MaterialManager();
        Factory factory = new Factory(elementCategory, materialManager, objectPoolDummy);
        EventManager eventManager = new EventManager();
        UIManager uiManager = new UIManager(uiPrefabCategory);
        CameraManager cameraManager = new CameraManager(cameraCategory);

        EDCServer server = new EDCServer();
        await server.InitalizeFirebase();

        // Provide Locator
        Locator<Factory>.Provide(factory);
        Locator<EventManager>.Provide(eventManager);
        Locator<CameraManager>.Provide(cameraManager);
        Locator<UIManager>.Provide(uiManager);
        Locator<EDCServer>.Provide(server);
        Locator<MaterialManager>.Provide(materialManager);
    }

    private void Start()
    {
        var uiManager = Locator<UIManager>.Get();
        var title = uiManager.GetUIPrefabObject(UIPrefab.Title);
        title.SetActive(true);
    }

}

