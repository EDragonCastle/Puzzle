using UnityEngine;

/// <summary>
/// Sound Object�� �����ϰ� �ִ� Manager
/// </summary>
public class SoundManager 
{
    // Sound Category
    private SoundCategory category;

    // ȿ�������� ������ Object Pool
    private ObjectPool<SFX, SoundCategory> objectPools;
    
    // BGM Sound
    private GameObject uniqueBGMObject;

    /// <summary>
    /// Sound Manager ������
    /// </summary>
    /// <param name="_category">Sound Category</param>
    /// <param name="parent">Sound�� �� ���� ���� ����</param>
    public SoundManager(SoundCategory _category, GameObject parent = null)
    {
        category = _category;
        objectPools = new ObjectPool<SFX, SoundCategory>(_category, parent);
    }

    /// <summary>
    /// BGM Sound�� �����Ѵ�.
    /// </summary>
    /// <param name="bgm">Enum���� �����ǰ� �ִ� bgm type</param>
    public void PlayBGM(BGM bgm)
    {
        // BGM Object�� null check�� �ؼ� ���ٸ� �׳� �����ϰ�, �ִٸ� ���� BGM�� �����ϰ� ���ο� BGM�� �����Ѵ�.
        if(uniqueBGMObject == null)
            uniqueBGMObject = GameObject.Instantiate(category.GetSound(bgm));
        else
        {
            DestoryBGM();
            uniqueBGMObject = GameObject.Instantiate(category.GetSound(bgm));
        }    
    }

    /// <summary>
    /// BGM Sound�� �����Ѵ�.
    /// </summary>
    public void DestoryBGM()
    {
        GameObject.Destroy(uniqueBGMObject);
    }

    /// <summary>
    /// SFX Sound�� �����Ѵ�.
    /// </summary>
    /// <param name="sfx">ȿ����</param>
    public void PlaySFX(SFX sfx)
    {
        objectPools.Get(sfx);
    }

    /// <summary>
    /// SFX Sound�� �ݳ��Ѵ�.
    /// </summary>
    /// <param name="sfxObject">ȿ������ ��� Object</param>
    public void ReturnSFX(GameObject sfxObject)
    {
        objectPools.Return(sfxObject);
    }
}
