using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestResource : MonoBehaviour
{
    public void StartButton()
    {
        var uiManager = Locator<UIManager>.Get();
        var title = uiManager.GetUIPrefabObject(UIPrefab.Title);
        title.SetActive(true);
        var soundManager = Locator<SoundManager>.Get();
        soundManager.PlayBGM(BGM.Title);
        Debug.Log("Complete Title Setting");
    }
}
