using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private ElementCategory elementCategory;

    void Start()
    {
        Factory factory = new Factory(elementCategory);
        Locator.ProvideFactory(factory);
    }

}

