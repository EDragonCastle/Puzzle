using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private ElementCategory elementCategory;
    [SerializeField]
    private GameObject objectPoolDummy;

    private void Awake()
    {
        Factory factory = new Factory(elementCategory, objectPoolDummy);
        Locator.ProvideFactory(factory);
        
        EventManager eventManager = new EventManager();
        Locator.ProvideEventManager(eventManager);

        // Generic Test
        GenericLocator<Factory>.Provide(factory);
        GenericLocator<EventManager>.Provide(eventManager);
    }

}

