using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object�� �����ϴ� ����
/// SetActive True, False�� �����Ѵ�.
/// </summary>
/// <typeparam name="T">Object Type</typeparam>
/// <typeparam name="CATEGORY">���ϴ� Category�� �����Ѵ�</typeparam>
public class ObjectPool<T, CATEGORY>
{
    private readonly CATEGORY category;
    private readonly int initalizeLength;

    // key : origin Object, value : Clone Object List
    private Dictionary<GameObject, Stack<GameObject>> pools;

    private GameObject saveObjectPools;

    // Object Pool ������
    #region AddressObject Object Pool Consgtrcut
    /// <summary>
    /// Object Pool �����ڸ� ����Ѵ�.
    /// </summary>
    /// <param name="_category">Category ����</param>
    /// <param name="parent">Object���� �� ���� ���� ����</param>
    public ObjectPool(CATEGORY _category, GameObject parent = null)
    {
        category = _category;
        initalizeLength = 20;
        pools = new Dictionary<GameObject, Stack<GameObject>>();
        saveObjectPools = parent;
        Initialize();
    }

    /// <summary>
    /// Object Pool �����ڸ� ����Ѵ�.
    /// </summary>
    /// <param name="_category">Category ����</param>
    /// <param name="length">Object Pool�� ���� Size</param>
    /// <param name="parent">Object���� �� ���� ���� ����</param>
    public ObjectPool(CATEGORY _category, int length, GameObject parent = null)
    {
        category = _category;
        initalizeLength = length;
        pools = new Dictionary<GameObject, Stack<GameObject>>();
        saveObjectPools = parent;
        Initialize();
    }
    #endregion

    // �����ڿ��� ������ �Լ�
    private void Initialize()
    {
        // ElementCategory�� Ȯ���Ѵ�.
        if (typeof(CATEGORY) == typeof(ElementCategory))
        {
            // category�� Element Category�� ����
            var ElementCategory = category as ElementCategory;

            // Color�� ������ŭ ��ȸ�ϸ鼭 Object ����
            for (int j = 0; j < (int)ElementColor.End; j++)
            {
                // int�� Enum ������ �����ؼ� Category���� Origine Prefab�� �����´�.
                var originePrefab = ElementCategory.GetCategory((ElementColor)j);
                var prefabList = new Stack<GameObject>();
                
                // �ʱ� size��ŭ ��ȸ�ؼ� �����Ѵ�.
                for (int i = 0; i < initalizeLength; i++)
                {
                    // prefab�� �޾ƿͼ� instantiate�� ���� ���� ������ �� �ֵ��� �����Ѵ�.
                    GameObject copyGameObject = GameObject.Instantiate(originePrefab, saveObjectPools.transform);
                    
                    // Object �������� Ȯ��
                    if (copyGameObject != null)
                    {
                        // Material Manager���� �ʱ� Material �����ϵ��� �Ѵ�.
                        var materialManager = Locator<MaterialManager>.Get();
                        materialManager.CreateMaterial(copyGameObject, (ElementColor)j);
                        
                        // ��Ȱ��ȭ ��Ű�� Stack�� �ִ´�.
                        copyGameObject.SetActive(false);
                        prefabList.Push(copyGameObject);
                    }
                    else
                        Debug.LogError("Exist Not Object");
                }
                
                // �������� Stack�� ������ ���� Dictionary�� �־ �����Ѵ�.
                pools.Add(originePrefab, prefabList);
            }
        }
    }

    // Type�� �޴� Object�� �����´�.
    public GameObject Get(T _value)
    {
        // CATEGORY type�� T Type�� �����Ǿ� �ִ��� Ȯ���Ѵ�.
        if (typeof(CATEGORY) == typeof(ElementCategory) && typeof(T) == typeof(ElementColor))
        {
            // Category�� ElementCategory�� �����Ѵ�.
            var elementCategory = category as ElementCategory;

            // _value ���� ElementColor color�� �����Ѵ�.
            if(_value is ElementColor color)
            {
                // color�� ���� Origine Prefab�� ã�´�.
                var prefabs = elementCategory.GetCategory(color);

                // Prefab�� ���� object Pool�� ����Ǿ� �ִ� stack�� �����´�.
                var stackObjects = pools[prefabs];

                // �̸� object �غ�
                GameObject getObject = null;
                
                // stack Count�� ��� �ִ��� Ȯ���Ѵ�. ��� ������ ���� �����ؼ� Object�� ����ϰ�, ��� ���� �ʴٸ� Stack���� �ϳ��� ����.
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
        // CATEGORY�� SoundCategory������ SFX Type���� Ȯ���Ѵ�.
        else if(typeof(CATEGORY) == typeof(SoundCategory) && typeof(T) == typeof(SFX))
        {
            // category�� SoundCategory�� �����Ѵ�.
            var soundCategory = category as SoundCategory;

            // _value�� sfx�� �����Ѵ�.
            if(_value is SFX sfx)
            {
                // sfx ���� ���� Origine Prefab�� �����´�.
                var prefabs = soundCategory.GetSound(sfx);

                GameObject getObject = null;

                // pool���� sfx ������ ���ٸ� ���� Stack�� �ְ� �ִٸ� Stack���� ���´�.
                if(!pools.ContainsKey(prefabs))
                {
                    // ���ο� Stack�� ����� Dictionary�� �ִ´�.
                    var stack = new Stack<GameObject>();
                    pools.Add(prefabs, stack);

                    // ������ Object���� Pooled Component Object�� �������� sfx�� ����.
                    getObject = GameObject.Instantiate(prefabs, saveObjectPools.transform);
                    var component = getObject.GetComponent<PooledSoundObject>();

                    if (component != null)
                        component.SetSFX(sfx);
                    return getObject;
                }
                else
                {
                    // Object Pool���� origine ������ Stack�� �����´�.
                    var stackObjects = pools[prefabs];
                    
                    // Stack�� ����ִ��� Ȯ���ؼ� ��� �ִٸ� Object�� ���� �����ϰ�, ��� ���� �ʴٸ� stack���� �����´�.
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

    /// <summary>
    /// Object�� Pool�� �ݳ��Ѵ�.
    /// </summary>
    /// <param name="destoryObject">�ݳ��� object</param>
    public void Return(GameObject destoryObject)
    {
        // �ݳ��ϴ� ���� CATEGORY�� Ȯ���Ѵ�.
        if (typeof(CATEGORY) == typeof(ElementCategory))
        {
            // category�� ElementCategory�� �����ϰ� Component�� Ȯ���Ѵ�.
            var elementCategory = category as ElementCategory;
            var elementObject = destoryObject.GetComponent<IUIElement>();

            // null check�� �Ѵ�.
            if (elementObject != null)
            {
                // �ٽ� �ʱ� ���·� �ǵ����� Pool�� �ִ´�.
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
            // category�� SoundCategory�� �����ϰ� Component�� Ȯ���Ѵ�.
            var soundCategory = category as SoundCategory;
            var component = destoryObject.GetComponent<PooledSoundObject>();

            // null check ��
            if (component != null)
            {
                // origine�� sfx�� �����صд�.
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
