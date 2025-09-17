using System.Collections;
using UnityEngine;

/// <summary>
/// �������� �� Coroutine�� ����� �ڵ����� �����ϴ� Class
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class DestorySoundObject : MonoBehaviour
{
    private void Start()
    {
        // Audio Source�� �ð���ŭ �帥 �Ŀ� �����ϴ� �Լ�
        var audiosource = GetComponent<AudioSource>();
        StartCoroutine(DestoryObject(audiosource.clip.length));
    }

    private IEnumerator DestoryObject(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}
