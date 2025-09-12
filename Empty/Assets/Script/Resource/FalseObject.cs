using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalseObject : MonoBehaviour
{
    [SerializeField]
    private float time = 1f;

    private void OnEnable()
    {
        StartCoroutine(ActiveFalse(time));
    }

    private IEnumerator ActiveFalse(float time)
    {
        yield return new WaitForSeconds(time);
        this.gameObject.SetActive(false);
    }
}
