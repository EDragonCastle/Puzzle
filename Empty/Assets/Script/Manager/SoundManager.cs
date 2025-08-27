using UnityEngine;

public class SoundManager 
{
    private SoundCategory category;

    private GameObject uniqueBGMSound;

    public SoundManager(SoundCategory _category)
    {
        category = _category;
    }

    public void PlayBGM(BGM bgm)
    {
        if(uniqueBGMSound == null)
            uniqueBGMSound = GameObject.Instantiate(category.GetSound(bgm));
    }

    public void DestoryBGM()
    {
        GameObject.Destroy(uniqueBGMSound);
    }

    public void PlaySFX(SFX sfx)
    {
        GameObject.Instantiate(category.GetSound(sfx));
    }
}
