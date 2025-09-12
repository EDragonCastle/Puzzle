using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPool<T, CATEGORY>
{
    private readonly CATEGORY category;
    private readonly int initalizeLength;

    // Resource, Object
    private Dictionary<GameObject, Stack<GameObject>> pools;

    private GameObject saveObjectPools;

    #region AddressObject Object Pool Consgtrcut
    public ObjectPool(CATEGORY _category, GameObject parent = null)
    {
        category = _category;
        initalizeLength = 20;
        pools = new Dictionary<GameObject, Stack<GameObject>>();
        saveObjectPools = parent;
        Initialize();
    }

    public ObjectPool(CATEGORY _category, int length, GameObject parent = null)
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
        if (typeof(CATEGORY) == typeof(ElementCategory))
        {
            var ElementCategory = category as ElementCategory;

            for (int j = 0; j < (int)ElementColor.End; j++)
            {
                var originePrefab = ElementCategory.GetCategory((ElementColor)j);
                var prefabList = new Stack<GameObject>();
                for (int i = 0; i < initalizeLength; i++)
                {
                    // 위에 prefab을 받아와서 instantiate로 생성
                    GameObject copyGameObject = GameObject.Instantiate(originePrefab, saveObjectPools.transform);

                    if (copyGameObject != null)
                    {
                        var materialManager = Locator<MaterialManager>.Get();
                        materialManager.CreateMaterial(copyGameObject, (ElementColor)j);
                        copyGameObject.SetActive(false);
                        prefabList.Push(copyGameObject);
                    }
                    else
                        Debug.LogError("Exist Not Object");
                }
                pools.Add(originePrefab, prefabList);
            }
        }
    }

    public GameObject Get(T _value)
    {
        if (typeof(CATEGORY) == typeof(ElementCategory) && typeof(T) == typeof(ElementColor))
        {
            var elementCategory = category as ElementCategory;

            if(_value is ElementColor color)
            {
                var prefabs = elementCategory.GetCategory(color);
                var stackObjects = pools[prefabs];

                GameObject getObject = null;
                if (stackObjects.Count > 0)
                {
                    getObject = stackObjects.Pop();
                    getObject.SetActive(true);
                }
                else
                {
                    getObject = GameObject.Instantiate(prefabs, saveObjectPools.transform);
                    var materialManager = Locator<MaterialManager>.Get();
                    materialManager.CreateMaterial(getObject, color);
                }
                return getObject;
            }
        }
        else if(typeof(CATEGORY) == typeof(SoundCategory) && typeof(T) == typeof(SFX))
        {
            var soundCategory = category as SoundCategory;

            if(_value is SFX sfx)
            {
                var prefabs = soundCategory.GetSound(sfx);

                GameObject getObject = null;
                // 안에 key 값이 없으면 stack을 생성한다.
                if(!pools.ContainsKey(prefabs))
                {
                    var stack = new Stack<GameObject>();
                    pools.Add(prefabs, stack);

                    getObject = GameObject.Instantiate(prefabs, saveObjectPools.transform);
                    var component = getObject.GetComponent<PooledSoundObject>();
                    if (component != null)
                        component.SetSFX(sfx);
                    return getObject;
                }
                else
                {
                    var stackObjects = pools[prefabs];
                    if (stackObjects.Count > 0)
                    {
                        getObject = stackObjects.Pop();
                        getObject.SetActive(true);
                    }
                    else
                    {
                        getObject = GameObject.Instantiate(prefabs, saveObjectPools.transform);
                        var component = getObject.GetComponent<PooledSoundObject>();
                        if (component != null)
                            component.SetSFX(sfx);
                    }

                    return getObject;
                }
            }
        }

        return null;
    }

    public void Return(GameObject destoryObject)
    {
        if (typeof(CATEGORY) == typeof(ElementCategory))
        {
            var elementCategory = category as ElementCategory;
            var elementObject = destoryObject.GetComponent<IUIElement>();

            if (elementObject != null)
            {
                var elementInfo = elementObject.GetElementInfo();
                elementInfo.isVisits = false;
                elementInfo.position = default;
                elementObject.SetElementInfo(elementInfo);
                destoryObject.SetActive(false);

                var returnObject = elementCategory.GetCategory((ElementColor)elementInfo.color);
                var stackObjects = pools[returnObject];
                stackObjects.Push(destoryObject);
            }
            else
            {
                Debug.Log($"{destoryObject} is not Exist IUIElement Component");
            }
        }
        else if(typeof(CATEGORY) == typeof(SoundCategory))
        {
            var soundCategory = category as SoundCategory;
            var component = destoryObject.GetComponent<PooledSoundObject>();
            if (component != null)
            {
                var sfx = component.GetSFX();
                var sfxObject = soundCategory.GetSound(sfx);
                var stackObject = pools[sfxObject];
                stackObject.Push(destoryObject);
            }
            else
            {
                Debug.Log($"{destoryObject} is not Exist PooledSoundObject Component");
            }
        }
    }
}
