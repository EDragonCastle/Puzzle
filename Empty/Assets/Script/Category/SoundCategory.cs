using UnityEngine;
using System.Collections.Generic;

public class SoundCategory : MonoBehaviour
{
    private List<GameObject> bgmList;
    private List<GameObject> sfxList;

    public SoundCategory(List<GameObject> _bgmList, List<GameObject> _sfxList)
    {
        bgmList = _bgmList;
        sfxList = _sfxList;
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