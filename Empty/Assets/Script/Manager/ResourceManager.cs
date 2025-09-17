using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

/// <summary>
/// Addressable Asset���� ����� Resource���� �����ϰ� �ִ� Manager
/// </summary>
public class ResourceManager
{
    // enum���ε� �Ϸ� ������ string���� �����ϵ��� �ߴ�.
    private Dictionary<string, HandleObject> managementHandleList;

    // Load�� �� Resource���� ������ Dictionary
    private Dictionary<ResourceType, List<GameObject>> loadedResourceList;

    #region ResourceManager ������
    /// <summary>
    /// Resource Manager ������ �� ���� �ʱ�ȭ
    /// </summary>
    public ResourceManager()
    {
        managementHandleList = new Dictionary<string, HandleObject>();
        loadedResourceList = new Dictionary<ResourceType, List<GameObject>>();
    }
    #endregion
    

    // �� ó�� Load�� Object
    public async UniTask Load()
    {
        await LoadResource(ResourceType.Animation, "SkeletonGraphic (celestial-circus-pro)");
        await LoadResource(ResourceType.Animation, "SkeletonGraphic (cloud-pot)");

        await LoadResource(ResourceType.UI, "Game Board");
        await LoadResource(ResourceType.UI, "Game Over");
        await LoadResource(ResourceType.UI, "Life");
        await LoadResource(ResourceType.UI, "Ranker");
        await LoadResource(ResourceType.UI, "Title");

        await LoadResource(ResourceType.SFX, "None Swap");
        await LoadResource(ResourceType.SFX, "Pop");
        await LoadResource(ResourceType.SFX, "Pop2");
        await LoadResource(ResourceType.SFX, "Pop3");

        await LoadResource(ResourceType.BGM, "Stage1");

        await LoadResource(ResourceType.Default, "Blue Element");
        await LoadResource(ResourceType.Default, "Green Element");
        await LoadResource(ResourceType.Default, "Yellow Element");
        await LoadResource(ResourceType.Default, "Red Element");
    }

    /// <summary>
    /// Load�� Object List���� �����´�.
    /// </summary>
    /// <param name="type">���ϴ� Resource Type</param>
    /// <param name="resourceName">Resource �̸�</param>
    /// <returns>GameObject</returns>
    public GameObject GetResource(ResourceType type, string resourceName)
    {
        // Resource Type�� �ִ��� Ȯ���Ѵ�.
        if (loadedResourceList.ContainsKey(type))
        {
            // List�� �����ͼ� ��ȸ�ϴµ� ������ Object�� Return �Ѵ�.
            var loadTypeResourceList = loadedResourceList[type];
        
            foreach(var resourceObject in loadTypeResourceList)
            {
                if (resourceObject.name == resourceName)
                    return resourceObject;
            }
        }

        // ������ Ȥ�� �ٸ� Type�� �ִ� ������ Ȯ���Ѵ�.
        for(int i = 0; i < loadedResourceList.Count; i++)
        {
            if(!loadedResourceList.ContainsKey((ResourceType)i))
                continue;

            var listObject = loadedResourceList[(ResourceType)i];
            foreach(var resource in listObject)
            {
                if (resource.name == resourceName)
                    return resource;
            }
        }

        // �׷��� ���ٸ� Log�� ����.
        Debug.LogError($"Don't Exist {resourceName}");

        return null;
    }

    /// <summary>
    /// ���� Addressable Asset���� Load�� Object
    /// </summary>
    /// <param name="type">Resource Type</param>
    /// <param name="resourceName">Resource �̸�</param>
    /// <returns></returns>
    public async UniTask LoadResource(ResourceType type, string resourceName)
    {
        // Addressable Asset���� Load�� �Ѵ�.
        var handle = Addressables.LoadAssetAsync<GameObject>(resourceName);
        var loadedObject = await handle.Task;

        // Addressable Asset���� Ȯ���ߴµ� ������ �����Ѵ�.
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            // �̸����� �����ǰ� �ִ� Object���� Ȯ���ϴµ� ������ �����ؼ� ���� �����, ������ �̹� Load�ƴٰ� �˸���.
            if (!managementHandleList.ContainsKey(resourceName))
            {
                HandleObject handleObject = new HandleObject();
                handleObject.handle = handle;
                handleObject.type = type;
                managementHandleList.Add(loadedObject.name, handleObject);
            }
            else
                return;

            // LoadList�� Type�� �ִ��� Ȯ���ؼ� ���ٸ� ���� ���� �ְ�, �ִٸ� �ִ´�.
            if (!loadedResourceList.ContainsKey(type))
            {
                loadedResourceList.Add(type, new List<GameObject>());
                loadedResourceList[type].Add(loadedObject);
            }
            else
                loadedResourceList[type].Add(loadedObject);
        }
    }

    /// <summary>
    /// ����� Resource�� �ݳ��Ѵ�.
    /// </summary>
    /// <param name="releaseObject">�ݳ��� Resource Object</param>
    public void ReleaseResource(GameObject releaseObject)
    {
        // �����ǰ� �ִ� List�� �ִ��� Ȯ���Ѵ�.
        if (managementHandleList.ContainsKey(releaseObject.name))
        {
            // List���� �����Ѵ�.
            var handleObject = managementHandleList[releaseObject.name];
            var resourceList = loadedResourceList[handleObject.type];

            // ��ȸ�ϸ鼭 ������ �����Ѵ�.
            for(int i = 0; i < resourceList.Count; i++)
            {
                var resourceObject = resourceList[i];
                if(resourceObject == releaseObject)
                {
                    resourceList.RemoveAt(i);
                    break;
                }
            }

            // �޸𸮿� ��ϵǾ� �ִ� ���� �ݳ� �� Dictonary�� ����Ǿ� �ִ°� �����ϰ� Addressable Asset���� �ٽ� �ݳ��Ѵ�.
            Debug.Log("Release Handle");
            Addressables.Release(handleObject.handle);
            managementHandleList.Remove(releaseObject.name);
        }
        else
        {
            Debug.Log($"Exits Not {releaseObject.name}");
            return;
        }
    }

    // ���� Load�ߴ� ���
    private void LegacyLoad()
    {
        // Animation
        Addressables.LoadAssetAsync<GameObject>("SkeletonGraphic (celestial-circus-pro)").Completed += (handle) => OnAssetLoad(handle, ResourceType.Animation);
        Addressables.LoadAssetAsync<GameObject>("SkeletonGraphic (cloud-pot)").Completed += (handle) => OnAssetLoad(handle, ResourceType.Animation);

        // UI
        Addressables.LoadAssetAsync<GameObject>("Game Board").Completed += (handle) => OnAssetLoad(handle, ResourceType.UI);
        Addressables.LoadAssetAsync<GameObject>("Game Over").Completed += (handle) => OnAssetLoad(handle, ResourceType.UI);
        Addressables.LoadAssetAsync<GameObject>("Life").Completed += (handle) => OnAssetLoad(handle, ResourceType.UI);
        Addressables.LoadAssetAsync<GameObject>("Ranker").Completed += (handle) => OnAssetLoad(handle, ResourceType.UI);
        Addressables.LoadAssetAsync<GameObject>("Title").Completed += (handle) => OnAssetLoad(handle, ResourceType.UI);

        // SFX
        Addressables.LoadAssetAsync<GameObject>("None Swap").Completed += (handle) => OnAssetLoad(handle, ResourceType.SFX);
        Addressables.LoadAssetAsync<GameObject>("Pop").Completed += (handle) => OnAssetLoad(handle, ResourceType.SFX);
        Addressables.LoadAssetAsync<GameObject>("Pop2").Completed += (handle) => OnAssetLoad(handle, ResourceType.SFX);
        Addressables.LoadAssetAsync<GameObject>("Pop3").Completed += (handle) => OnAssetLoad(handle, ResourceType.SFX);

        // BGM
        Addressables.LoadAssetAsync<GameObject>("Stage1").Completed += (handle) => OnAssetLoad(handle, ResourceType.BGM);

        // Defult
        Addressables.LoadAssetAsync<GameObject>("Blue Element").Completed += (handle) => OnAssetLoad(handle, ResourceType.Default);
        Addressables.LoadAssetAsync<GameObject>("Green Element").Completed += (handle) => OnAssetLoad(handle, ResourceType.Default);
        Addressables.LoadAssetAsync<GameObject>("Yellow Element").Completed += (handle) => OnAssetLoad(handle, ResourceType.Default);
        Addressables.LoadAssetAsync<GameObject>("Red Element").Completed += (handle) => OnAssetLoad(handle, ResourceType.Default);
    }

    // ���� Load �ߴ� ��Ŀ��� ���Ǿ��� Load ���
    private void OnAssetLoad(AsyncOperationHandle<GameObject> handle, ResourceType type)
    {
        if(handle.Status == AsyncOperationStatus.Succeeded)
        {
            var loadedObject = handle.Result;
            if (!managementHandleList.ContainsKey(loadedObject.name))
            {
                HandleObject handleObject = new HandleObject();
                handleObject.handle = handle;
                handleObject.type = type;
                managementHandleList.Add(loadedObject.name, handleObject);
            }
            else
                return;

            if (!loadedResourceList.ContainsKey(type))
            {
                loadedResourceList.Add(type, new List<GameObject>());
                loadedResourceList[type].Add(loadedObject);
            }
            else
                loadedResourceList[type].Add(loadedObject);
        }
        else
        {
            Debug.LogError("Failed To Load, Error Code : Load Asset Name Error");
        }
    }
}

// Addressable Asset���� Group Type�̴�.
public enum ResourceType
{
    None,
    Default,
    Animation,
    BGM,
    SFX,
    UI,
    Material,
}

// ������ Object struct
public struct HandleObject
{
    public AsyncOperationHandle<GameObject> handle;
    public ResourceType type;
}