using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 모든 Sound 정보를 담고 있는 Class
/// </summary>
public class SoundCategory : MonoBehaviour
{
    private List<GameObject> bgmList;
    private List<GameObject> sfxList;

    /// <summary>
    /// SoundCategory 생성자
    /// </summary>
    /// <param name="_bgmList">bgm을 담고 있는 list</param>
    /// <param name="_sfxList">sfx를 담고 있는 list</param>
    public SoundCategory(List<GameObject> _bgmList, List<GameObject> _sfxList)
    {
        bgmList = _bgmList;
        sfxList = _sfxList;
    }

    /// <summary>
    /// BGM 정보로 BGMSound를 출력한다.
    /// </summary>
    /// <param name="bgm">BGM 정보</param>
    /// <returns>BGM Sound</returns>
    public GameObject GetSound(BGM bgm)
    {
        GameObject newSoundObject = null;
        string bgmName = BGMToString(bgm);
        newSoundObject = StringToBGMSound(bgmName);
        return newSoundObject;
    }

    /// <summary>
    /// SFX 정보로 SFXSound를 출력한다.
    /// </summary>
    /// <param name="sfx">SFX 정보</param>
    /// <returns>SFX Sound</returns>
    public GameObject GetSound(SFX sfx)
    {
        GameObject newSoundObject = null;
        string sfxName = SFXToString(sfx);
        newSoundObject = StringToSFXSound(sfxName);
        return newSoundObject;
    }

    /// <summary>
    /// BGM 이름으로 BGMSound 출력
    /// </summary>
    /// <param name="bgm">BGM 이름</param>
    /// <returns>BGM Sound</returns>
    private GameObject StringToBGMSound(string bgm)
    {
        foreach(var obj in bgmList)
        {
            if (obj.name == bgm)
                return obj;
        }

        return null;
    }

    /// <summary>
    /// SFX 이름으로 SFXSound 출력
    /// </summary>
    /// <param name="sfx">SFX 이름</param>
    /// <returns>SFX Sound</returns>
    private GameObject StringToSFXSound(string sfx)
    {
        foreach(var obj in sfxList)
        {
            if (obj.name == sfx)
                return obj;
        }
        return null;
    }

    /// <summary>
    /// BGM 정보를 BGM 이름으로 변경하는 메서드
    /// </summary>
    /// <param name="bgm">BGM 정보</param>
    /// <returns>BGM 이름</returns>
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

    /// <summary>
    /// SFX 정보를 SFX 이름으로 변경하는 메서드
    /// </summary>
    /// <param name="sfx">SFX 정보</param>
    /// <returns>SFX 이름</returns>
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

/// <summary>
/// BGM 정보를 담고 있는 Enum
/// </summary>
public enum BGM
{
    Title,
}

/// <summary>
/// SFX 정보를 담고 있는 Enum
/// </summary>
public enum SFX
{
    NoneSwap,
    Pop,
    Pop2,
    Pop3,
}