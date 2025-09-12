using UnityEngine;

public class SoundManager 
{
    private SoundCategory category;
    private ObjectPool<SFX, SoundCategory> objectPools;
    private GameObject uniqueBGMObject;

    public SoundManager(SoundCategory _category, GameObject parent = null)
    {
        category = _category;
        objectPools = new ObjectPool<SFX, SoundCategory>(_category, parent);
    }

    public void PlayBGM(BGM bgm)
    {
        if(uniqueBGMObject == null)
            uniqueBGMObject = GameObject.Instantiate(category.GetSound(bgm));
        else
        {
            DestoryBGM();
            uniqueBGMObject = GameObject.Instantiate(category.GetSound(bgm));
        }    
    }

    public void DestoryBGM()
    {
        GameObject.Destroy(uniqueBGMObject);
    }

    public void PlaySFX(SFX sfx)
    {
        objectPools.Get(sfx);
    }

    public void ReturnSFX(GameObject sfxObject)
    {
        objectPools.Return(sfxObject);
    }
}
