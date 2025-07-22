using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager
{
    private readonly ElementCategory category;
    private readonly int initalizeLength;
    private Dictionary<GameObject, Stack<GameObject>> pools;

    private GameObject saveObjectPools;

    #region Object Pool Construct
    public ObjectPoolManager(ElementCategory _category, GameObject parent = null)
    {
        category = _category;
        initalizeLength = 20;
        pools = new Dictionary<GameObject, Stack<GameObject>>();
        saveObjectPools = parent;
        Initialize();
    }

    public ObjectPoolManager(ElementCategory _category, int length, GameObject parent = null)
    {
        category = _category;
        initalizeLength = length;
        pools = new Dictionary<GameObject, Stack<GameObject>>();
        saveObjectPools = parent;
        Initialize();
    }
    #endregion

    private void Initialize()
    {
        for(int j = 0; j < (int)ElementColor.End; j++)
        {
            var originePrefab = category.GetCategory((ElementColor)j);
            var prefabList = new Stack<GameObject>();
            for (int i = 0; i < initalizeLength; i++)
            {
                var copyGameObject = GameObject.Instantiate(originePrefab);
                copyGameObject.SetActive(false);
                if (saveObjectPools != null)
                    copyGameObject.transform.SetParent(saveObjectPools.transform);
                prefabList.Push(copyGameObject);
            }
            pools.Add(originePrefab, prefabList);
        }
    }

    public GameObject Get(ElementColor _color)
    {
        var prefabs = category.GetCategory(_color);
        var stackObjects = pools[prefabs];

        GameObject getObject = null;
        if(stackObjects.Count > 0)
        {
            getObject = stackObjects.Peek();
            getObject.SetActive(true);
            stackObjects.Pop();
            return getObject;
        }
        else
        {
            Debug.Log("Additional Object");
            getObject = GameObject.Instantiate(prefabs);
        }

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
}
