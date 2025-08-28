using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager
{
    // handle은 string로 관리하는 것이 맞는 것 같다.
    private Dictionary<string, HandleObject> managementHandleList;

    // 그렇다면 Load한 Resource 관리는 어떻게 해야할까?
    private Dictionary<ResourceType, List<GameObject>> loadedResourceList;

    public ResourceManager()
    {
        managementHandleList = new Dictionary<string, HandleObject>();
        loadedResourceList = new Dictionary<ResourceType, List<GameObject>>();
        Load();
    }
    
    
    // Addressable 에서 Resource를 Load 해야 하는데 반납도 신경써야 한다.
    // 근데 GameObject로 통일해놔서 괜찮을 것 같다. Handle만 신경쓰면 될 것 같다.
    private void Load()
    {
        LoadResource(ResourceType.Animation, "SkeletonGraphic (celestial-circus-pro)");
        LoadResource(ResourceType.Animation, "SkeletonGraphic (cloud-pot)");

        LoadResource(ResourceType.UI, "Game Board");
        LoadResource(ResourceType.UI, "Game Over");
        LoadResource(ResourceType.UI, "Life");
        LoadResource(ResourceType.UI, "Ranker");
        LoadResource(ResourceType.UI, "Title");

        LoadResource(ResourceType.SFX, "None Swap");
        LoadResource(ResourceType.SFX, "Pop");
        LoadResource(ResourceType.SFX, "Pop2");
        LoadResource(ResourceType.SFX, "Pop3");

        LoadResource(ResourceType.BGM, "Stage1");

        LoadResource(ResourceType.Default, "Blue Element");
        LoadResource(ResourceType.Default, "Green Element");
        LoadResource(ResourceType.Default, "Yellow Element");
        LoadResource(ResourceType.Default, "Red Element");
    }

    public GameObject GetResource(ResourceType type, string resourceName)
    {
        // 포함되어 있으면 하고 없으면 넘긴다.
        if (loadedResourceList.ContainsKey(type))
        {
            var loadTypeResourceList = loadedResourceList[type];
        
            // Type 내에 있으면 Return
            foreach(var resourceObject in loadTypeResourceList)
            {
                if (resourceObject.name == resourceName)
                    return resourceObject;
            }
        }

        // 없으면 다른 Type에 있는지 순회해야 한다.
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

    public void LoadResource(ResourceType type, string resourceName)
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(resourceName);
        handle.WaitForCompletion();

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var loadedObject = handle.Result;

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

            // 메모리에 등록되어 있던 것을 반납 후 Dictonary에 저장되어 있는거 삭제.
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

    // Async도 기다리는 거잖아. 오래 걸릴 거 같은데 일단 받고 생각해볼까?
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
    /// Asset을 Load하는 함수
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