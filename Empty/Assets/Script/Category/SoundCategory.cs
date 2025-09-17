using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ��� Sound ������ ��� �ִ� Class
/// </summary>
public class SoundCategory : MonoBehaviour
{
    private List<GameObject> bgmList;
    private List<GameObject> sfxList;

    /// <summary>
    /// SoundCategory ������
    /// </summary>
    /// <param name="_bgmList">bgm�� ��� �ִ� list</param>
    /// <param name="_sfxList">sfx�� ��� �ִ� list</param>
    public SoundCategory(List<GameObject> _bgmList, List<GameObject> _sfxList)
    {
        bgmList = _bgmList;
        sfxList = _sfxList;
    }

    /// <summary>
    /// BGM ������ BGMSound�� ����Ѵ�.
    /// </summary>
    /// <param name="bgm">BGM ����</param>
    /// <returns>BGM Sound</returns>
    public GameObject GetSound(BGM bgm)
    {
        GameObject newSoundObject = null;
        string bgmName = BGMToString(bgm);
        newSoundObject = StringToBGMSound(bgmName);
        return newSoundObject;
    }

    /// <summary>
    /// SFX ������ SFXSound�� ����Ѵ�.
    /// </summary>
    /// <param name="sfx">SFX ����</param>
    /// <returns>SFX Sound</returns>
    public GameObject GetSound(SFX sfx)
    {
        GameObject newSoundObject = null;
        string sfxName = SFXToString(sfx);
        newSoundObject = StringToSFXSound(sfxName);
        return newSoundObject;
    }

    /// <summary>
    /// BGM �̸����� BGMSound ���
    /// </summary>
    /// <param name="bgm">BGM �̸�</param>
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
    /// SFX �̸����� SFXSound ���
    /// </summary>
    /// <param name="sfx">SFX �̸�</param>
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
    /// BGM ������ BGM �̸����� �����ϴ� �޼���
    /// </summary>
    /// <param name="bgm">BGM ����</param>
    /// <returns>BGM �̸�</returns>
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
    /// SFX ������ SFX �̸����� �����ϴ� �޼���
    /// </summary>
    /// <param name="sfx">SFX ����</param>
    /// <returns>SFX �̸�</returns>
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
/// BGM ������ ��� �ִ� Enum
/// </summary>
public enum BGM
{
    Title,
}

/// <summary>
/// SFX ������ ��� �ִ� Enum
/// </summary>
public enum SFX
{
    NoneSwap,
    Pop,
    Pop2,
    Pop3,
}