using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private ElementCategory elementCategory;
    [SerializeField]
    private GameObject objectPoolDummy;

    void Start()
    {
        Factory factory = new Factory(elementCategory, objectPoolDummy);
        Locator.ProvideFactory(factory);
    }

}

