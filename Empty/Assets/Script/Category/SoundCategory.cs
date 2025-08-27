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
        // key�� Group Name�� �ƴ϶� �Ƹ� Label�� Object Name�� ���̴�. Group Name�� �ƴ� ���̴�! (����) �´�.
        // Material�� �� �۵��� �ƴ�. ���ϱ� Label�� Material�� �־���.
        // LoadAssetsAsync�� ���� ������ �� ���� Release �� ���ϰ�, LoadAssetAsync�� �ϳ��� Object�� Release�� �� �Ѵ�.
        // ������ ��ü�ϰ� ������ ���Ϸ� �����ϴ� ���� �°�, ��ü�� ��ü�ؾ� �Ѵٸ� ��ü�� �����ϴ� ���� �´�.
        bgmList = new List<GameObject>();
        sfxList = new List<GameObject>();

        // Sound Label Load
        var handle = Addressables.LoadAssetsAsync<GameObject>("BGM", null);
        handle.Completed += (ophandle) => OnAssetsLoaded(ophandle, "BGM");

        var handle2 = Addressables.LoadAssetsAsync<GameObject>("SFX", null);
        handle2.Completed += (ophandle) => OnAssetsLoaded(ophandle, "SFX");
    }

    // Prefab ���ĸ� �ָ� �ȴ�.
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
