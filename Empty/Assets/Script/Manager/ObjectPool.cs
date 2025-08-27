using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using System.Threading.Tasks;

public class ObjectPool
{
    private readonly ElementCategory category;
    private readonly int initalizeLength;
    // Resource, Object
    private Dictionary<AssetReferenceGameObject, Stack<GameObject>> pools;

    private GameObject saveObjectPools;
    private MaterialManager materialManager;

    private TaskCompletionSource<bool> initializeCompleteSource;
    private Task initalizeTask => initializeCompleteSource.Task;

    #region AddressObject Object Pool Consgtrcut
    public ObjectPool(ElementCategory _category, MaterialManager _materialManager, GameObject parent = null)
    {
        category = _category;
        initalizeLength = 20;
        pools = new Dictionary<AssetReferenceGameObject, Stack<GameObject>>();
        saveObjectPools = parent;
        materialManager = _materialManager;
        Initialize();
    }

    public ObjectPool(ElementCategory _category, MaterialManager _materialManager, int length, GameObject parent = null)
    {
        category = _category;
        initalizeLength = length;
        pools = new Dictionary<AssetReferenceGameObject, Stack<GameObject>>();
        saveObjectPools = parent;
        materialManager = _materialManager;
        Initialize();
    }
    #endregion

    #region AddressObject
    private async void Initialize()
    {
        initializeCompleteSource = new TaskCompletionSource<bool>();

        for (int j = 0; j < (int)ElementColor.End; j++)
        {
            var originePrefab = category.GetCategory((ElementColor)j);
            var prefabList = new Stack<GameObject>();
            for (int i = 0; i < initalizeLength; i++)
            {
                // 위에 prefab을 받아와서 instantiate로 생성
                GameObject copyGameObject = null;

                if (saveObjectPools != null)
                {
                    var handle = Addressables.InstantiateAsync(originePrefab, saveObjectPools.transform);
                    copyGameObject = await handle.Task;

                    if(copyGameObject != null)
                    {
                        materialManager.CreateMaterial(copyGameObject, (ElementColor)j);
                        copyGameObject.SetActive(false);
                        prefabList.Push(copyGameObject);
                    }
                }
            }
            pools.Add(originePrefab, prefabList);
        }

        initializeCompleteSource.TrySetResult(true);
    }

    public GameObject Get(ElementColor _color)
    {
        var prefabs = category.GetCategory(_color);
        var stackObjects = pools[prefabs];

        GameObject getObject = null;
        if (stackObjects.Count > 0)
        {
            getObject = stackObjects.Peek();
            getObject.SetActive(true);
            stackObjects.Pop();
            return getObject;
        }
        else
        {
            return NewObject(prefabs, _color);
        }
    }

    private GameObject NewObject(AssetReferenceGameObject prefabs, ElementColor _color)
    {
        var getObject = Addressables.InstantiateAsync(prefabs, saveObjectPools.transform).WaitForCompletion();
        materialManager.CreateMaterial(getObject, _color);
        return getObject;
    }

    public void Return(GameObject destoryObject)
    {
        var elementObject = destoryObject.GetComponent<IUIElement>();

        if (elementObject != null)
        {
            var elementInfo = elementObject.GetElementInfo();
            elementInfo.isVisits = false;
            elementInfo.position = default;
            elementObject.SetElementInfo(elementInfo);
            destoryObject.SetActive(false);

            var returnObject = category.GetCategory((ElementColor)elementInfo.color);
            var stackObjects = pools[returnObject];
            stackObjects.Push(destoryObject);
        }
        else
        {
            Debug.Log($"{destoryObject} is not Exist IUIElement Component");
        }
    }
    #endregion


}
