using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sound Object Pool로 다시 되돌리는 함수
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class PooledSoundObject : MonoBehaviour
{
    AudioSource audioSource;
    private SFX sfx;

    private void Awake()
    {
        // audio Source의 시간만큼 흐른 후 자동으로 Object Pool로 다시 되돌린다.
        audioSource = this.GetComponent<AudioSource>();
        StartCoroutine(DisEnableObject(audioSource.clip.length));
    }

    public void SetSFX(SFX _sfx) => sfx = _sfx;
    public SFX GetSFX() => sfx;


    // active true 일 때
    private void OnEnable()
    {
        // 다시 true되면 Awake랑 동일하게 진행하도록 한다.
        StartCoroutine(DisEnableObject(audioSource.clip.length));
    }

    // active false 일 때
    private void OnDisable()
    {
        var soundManager = Locator<SoundManager>.Get();
        soundManager.ReturnSFX(this.gameObject);
    }

    private IEnumerator DisEnableObject(float time)
    {
        yield return new WaitForSeconds(time);
        this.gameObject.SetActive(false);
    }
}
