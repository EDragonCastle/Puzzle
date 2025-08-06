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
        EventManager eventManager = new EventManager();
        MaterialManager materialManager = new MaterialManager();
        Locator<MaterialManager>.Provide(materialManager);
        Locator<EventManager>.Provide(eventManager);

        Factory factory = new Factory(elementCategory, materialManager, objectPoolDummy);
        UIManager uiManager = new UIManager(uiPrefabCategory);
        CameraManager cameraManager = new CameraManager(cameraCategory);

        EDCServer server = new EDCServer();
        await server.InitalizeFirebase();

        // Provide Locator
   
        Locator<Factory>.Provide(factory);
        Locator<CameraManager>.Provide(cameraManager);
        Locator<UIManager>.Provide(uiManager);
        Locator<EDCServer>.Provide(server);
    }

    private void Start()
    {
        var uiManager = Locator<UIManager>.Get();
        var title = uiManager.GetUIPrefabObject(UIPrefab.Title);
        title.SetActive(true);
    }

}

