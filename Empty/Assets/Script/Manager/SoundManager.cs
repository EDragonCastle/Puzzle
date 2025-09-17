using UnityEngine;

/// <summary>
/// Sound Object를 관리하고 있는 Manager
/// </summary>
public class SoundManager 
{
    // Sound Category
    private SoundCategory category;

    // 효과음들을 관리할 Object Pool
    private ObjectPool<SFX, SoundCategory> objectPools;
    
    // BGM Sound
    private GameObject uniqueBGMObject;

    /// <summary>
    /// Sound Manager 생성자
    /// </summary>
    /// <param name="_category">Sound Category</param>
    /// <param name="parent">Sound를 한 곳에 담을 공간</param>
    public SoundManager(SoundCategory _category, GameObject parent = null)
    {
        category = _category;
        objectPools = new ObjectPool<SFX, SoundCategory>(_category, parent);
    }

    /// <summary>
    /// BGM Sound를 실행한다.
    /// </summary>
    /// <param name="bgm">Enum으로 관리되고 있는 bgm type</param>
    public void PlayBGM(BGM bgm)
    {
        // BGM Object의 null check를 해서 없다면 그냥 실행하고, 있다면 기존 BGM을 삭제하고 새로운 BGM을 실행한다.
        if(uniqueBGMObject == null)
            uniqueBGMObject = GameObject.Instantiate(category.GetSound(bgm));
        else
        {
            DestoryBGM();
            uniqueBGMObject = GameObject.Instantiate(category.GetSound(bgm));
        }    
    }

    /// <summary>
    /// BGM Sound를 제거한다.
    /// </summary>
    public void DestoryBGM()
    {
        GameObject.Destroy(uniqueBGMObject);
    }

    /// <summary>
    /// SFX Sound를 실행한다.
    /// </summary>
    /// <param name="sfx">효과음</param>
    public void PlaySFX(SFX sfx)
    {
        objectPools.Get(sfx);
    }

    /// <summary>
    /// SFX Sound를 반납한다.
    /// </summary>
    /// <param name="sfxObject">효과음이 담긴 Object</param>
    public void ReturnSFX(GameObject sfxObject)
    {
        objectPools.Return(sfxObject);
    }
}
