using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PooledSoundObject : MonoBehaviour
{
    AudioSource audioSource;

    private void Awake()
    {
        audioSource = this.GetComponent<AudioSource>();
        StartCoroutine(DisEnableObject(audioSource.clip.length));
    }

    // active true 일 때
    private void OnEnable()
    {
        StartCoroutine(DisEnableObject(audioSource.clip.length));
    }

    // active false 일 때
    private void OnDisable()
    {
         
    }

    private IEnumerator DisEnableObject(float time)
    {
        yield return new WaitForSeconds(time);
        this.gameObject.SetActive(false);
    }
}
