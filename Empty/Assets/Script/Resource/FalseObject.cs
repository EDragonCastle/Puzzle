using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 사용 후 Object를 Active False 하는 함수.
/// </summary>
public class FalseObject : MonoBehaviour
{
    [SerializeField]
    private float time = 1f;

    private void OnEnable()
    {
        // 정해진 시간만큼 흐른 후 active false가 되는 함수.
        StartCoroutine(ActiveFalse(time));
    }

    private IEnumerator ActiveFalse(float time)
    {
        yield return new WaitForSeconds(time);
        this.gameObject.SetActive(false);
    }
}
