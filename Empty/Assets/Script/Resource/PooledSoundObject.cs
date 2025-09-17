using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sound Object Pool�� �ٽ� �ǵ����� �Լ�
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class PooledSoundObject : MonoBehaviour
{
    AudioSource audioSource;
    private SFX sfx;

    private void Awake()
    {
        // audio Source�� �ð���ŭ �帥 �� �ڵ����� Object Pool�� �ٽ� �ǵ�����.
        audioSource = this.GetComponent<AudioSource>();
        StartCoroutine(DisEnableObject(audioSource.clip.length));
    }

    public void SetSFX(SFX _sfx) => sfx = _sfx;
    public SFX GetSFX() => sfx;


    // active true �� ��
    private void OnEnable()
    {
        // �ٽ� true�Ǹ� Awake�� �����ϰ� �����ϵ��� �Ѵ�.
        StartCoroutine(DisEnableObject(audioSource.clip.length));
    }

    // active false �� ��
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
