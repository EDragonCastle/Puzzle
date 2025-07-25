using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private ElementCategory elementCategory;
    [SerializeField]
    private GameObject objectPoolDummy;

    [SerializeField]
    private UIPrefabCategory uiPrefabCategory;

    private async void Awake()
    {
        // Manager constuctor
        Factory factory = new Factory(elementCategory, objectPoolDummy);
        EventManager eventManager = new EventManager();
        UIManager uiManager = new UIManager(uiPrefabCategory);

        EDCServer server = new EDCServer();
        await server.InitalizeFirebase();

        // Provide Locator
        Locator<Factory>.Provide(factory);
        Locator<EventManager>.Provide(eventManager);
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

