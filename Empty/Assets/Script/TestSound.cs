using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSound : MonoBehaviour
{
    public SoundCategory category;

    private void Awake()
    {
        SoundManager manager = new SoundManager(category);
        Locator<SoundManager>.Provide(manager);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var soundManager = Locator<SoundManager>.Get();
            soundManager.PlaySFX(SFX.Pop);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            var soundManager = Locator<SoundManager>.Get();
            soundManager.PlaySFX(SFX.Pop2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            var soundManager = Locator<SoundManager>.Get();
            soundManager.PlaySFX(SFX.Pop3);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            var soundManager = Locator<SoundManager>.Get();
            soundManager.PlaySFX(SFX.NoneSwap);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            var soundManager = Locator<SoundManager>.Get();
            soundManager.PlayBGM(BGM.Title);
        }
    }
}
