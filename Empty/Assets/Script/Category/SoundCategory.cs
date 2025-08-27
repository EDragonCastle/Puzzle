using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class SoundCategory : MonoBehaviour
{
    private List<GameObject> bgmList;
    private List<GameObject> sfxList;

    private void Awake()
    {
        // key는 Group Name이 아니라 아마 Label과 Object Name일 것이다. Group Name이 아닐 것이다! (가정) 맞다.
        // Material은 잘 작동이 됐다. 보니까 Label에 Material이 있었다.
        // LoadAssetsAsync는 여러 에셋을 한 번에 Release 할 때하고, LoadAssetAsync는 하나의 Object를 Release할 때 한다.
        // 개별로 교체하고 싶으면 단일로 관리하는 것이 맞고, 단체로 교체해야 한다면 단체로 관리하는 것이 맞다.
        bgmList = new List<GameObject>();
        sfxList = new List<GameObject>();

        // Sound Label Load
        var handle = Addressables.LoadAssetsAsync<GameObject>("BGM", null);
        handle.Completed += (ophandle) => OnAssetsLoaded(ophandle, "BGM");

        var handle2 = Addressables.LoadAssetsAsync<GameObject>("SFX", null);
        handle2.Completed += (ophandle) => OnAssetsLoaded(ophandle, "SFX");
    }

    // Prefab 형식만 주면 된다.
    public GameObject GetSound(BGM bgm)
    {
        GameObject newSoundObject = null;
        string bgmName = BGMToString(bgm);
        newSoundObject = StringToBGMSound(bgmName);
        return newSoundObject;
    }

    public GameObject GetSound(SFX sfx)
    {
        GameObject newSoundObject = null;
        string sfxName = SFXToString(sfx);
        newSoundObject = StringToSFXSound(sfxName);
        return newSoundObject;
    }

    
    private void OnAssetsLoaded(AsyncOperationHandle<IList<GameObject>> handle, string name)
    {
        if(handle.Status == AsyncOperationStatus.Succeeded)
        {
            var loadedObjects = handle.Result;
            
            switch(name)
            {
                case "BGM":
                    foreach (var obj in loadedObjects)
                    {
                        Debug.Log($"Load Object : {obj.name}");
                        bgmList.Add(obj);
                    }
                    break;
                case "SFX":
                    foreach (var obj in loadedObjects)
                    {
                        Debug.Log($"Load Object : {obj.name}");
                        sfxList.Add(obj);
                    }
                    break;
            }
        }
        else
        {
            Debug.LogError($"Failed To Load Assets From {name} Group");
        }
    }

    private GameObject StringToBGMSound(string bgm)
    {
        foreach(var obj in bgmList)
        {
            if (obj.name == bgm)
                return obj;
        }

        return null;
    }

    private GameObject StringToSFXSound(string sfx)
    {
        foreach(var obj in sfxList)
        {
            if (obj.name == sfx)
                return obj;
        }
        return null;
    }

    private string BGMToString(BGM bgm)
    {
        string bgmName = default;

        switch(bgm)
        {
            case BGM.Title:
                bgmName = "Stage1";
                break;
        }

        return bgmName;
    }

    private string SFXToString(SFX sfx)
    {
        string sfxName = default;

        switch(sfx)
        {
            case SFX.Pop:
                sfxName = "Pop";
                break;
            case SFX.Pop2:
                sfxName = "Pop2";
                break;
            case SFX.Pop3:
                sfxName = "Pop3";
                break;
            case SFX.NoneSwap:
                sfxName = "None Swap";
                break;
        }

        return sfxName;
    }
}

public enum BGM
{
    Title,
}

public enum SFX
{
    NoneSwap,
    Pop,
    Pop2,
    Pop3,
}
