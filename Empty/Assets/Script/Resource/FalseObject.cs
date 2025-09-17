using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��� �� Object�� Active False �ϴ� �Լ�.
/// </summary>
public class FalseObject : MonoBehaviour
{
    [SerializeField]
    private float time = 1f;

    private void OnEnable()
    {
        // ������ �ð���ŭ �帥 �� active false�� �Ǵ� �Լ�.
        StartCoroutine(ActiveFalse(time));
    }

    private IEnumerator ActiveFalse(float time)
    {
        yield return new WaitForSeconds(time);
        this.gameObject.SetActive(false);
    }
}
