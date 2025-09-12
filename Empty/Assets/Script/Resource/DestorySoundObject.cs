using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DestorySoundObject : MonoBehaviour
{
    private void Start()
    {
        var audiosource = GetComponent<AudioSource>();
        StartCoroutine(DestoryObject(audiosource.clip.length));
    }

    private IEnumerator DestoryObject(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}
