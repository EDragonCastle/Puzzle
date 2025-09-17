using System.Collections;
using UnityEngine;

/// <summary>
/// 생성했을 때 Coroutine을 사용해 자동으로 삭제하는 Class
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class DestorySoundObject : MonoBehaviour
{
    private void Start()
    {
        // Audio Source의 시간만큼 흐른 후에 삭제하는 함수
        var audiosource = GetComponent<AudioSource>();
        StartCoroutine(DestoryObject(audiosource.clip.length));
    }

    private IEnumerator DestoryObject(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}
