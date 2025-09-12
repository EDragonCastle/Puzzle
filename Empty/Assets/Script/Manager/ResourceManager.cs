using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

public class ResourceManager
{
    // handle�� string�� �����ϴ� ���� �´� �� ����.
    private Dictionary<string, HandleObject> managementHandleList;

    // �׷��ٸ� Load�� Resource ������ ��� �ؾ��ұ�?
    private Dictionary<ResourceType, List<GameObject>> loadedResourceList;

    #region ResourceManager ������
    public ResourceManager()
    {
        managementHandleList = new Dictionary<string, HandleObject>();
        loadedResourceList = new Dictionary<ResourceType, List<GameObject>>();
    }
    #endregion
    
    
    // Addressable ���� Resource�� Load �ؾ� �ϴµ� �ݳ��� �Ű��� �Ѵ�.
    // �ٵ� GameObject�� �����س��� ������ �� ����. Handle�� �Ű澲�� �� �� ����.
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

    public GameObject GetResource(ResourceType type, string resourceName)
    {
        // ���ԵǾ� ������ �ϰ� ������ �ѱ��.
        if (loadedResourceList.ContainsKey(type))
        {
            var loadTypeResourceList = loadedResourceList[type];
        
            // Type ���� ������ Return
            foreach(var resourceObject in loadTypeResourceList)
            {
                if (resourceObject.name == resourceName)
                    return resourceObject;
            }
        }

        // ������ �ٸ� Type�� �ִ��� ��ȸ�ؾ� �Ѵ�.
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

        Debug.LogError($"Don't Exist {resourceName}");

        return null;
    }

    public async UniTask LoadResource(ResourceType type, string resourceName)
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(resourceName);
        var loadedObject = await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            if (!managementHandleList.ContainsKey(resourceName))
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
    }

    public void ReleaseResource(GameObject releaseObject)
    {
        if (managementHandleList.ContainsKey(releaseObject.name))
        {
            var handleObject = managementHandleList[releaseObject.name];
            var resourceList = loadedResourceList[handleObject.type];

            for(int i = 0; i < resourceList.Count; i++)
            {
                var resourceObject = resourceList[i];
                if(resourceObject == releaseObject)
                {
                    resourceList.RemoveAt(i);
                    break;
                }
            }

            // �޸𸮿� ��ϵǾ� �ִ� ���� �ݳ� �� Dictonary�� ����Ǿ� �ִ°� ����.
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

    // Async�� ��ٸ��� ���ݾ�. ���� �ɸ� �� ������ �ϴ� �ް� �����غ���?
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

    /// <summary>
    /// Asset�� Load�ϴ� �Լ�
    /// </summary>
    /// <param name="handle">LoadAsset Handle</param>
    /// <param name="type">Resourcetype is LoadAsset Labels</param>
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

public struct HandleObject
{
    public AsyncOperationHandle<GameObject> handle;
    public ResourceType type;
}