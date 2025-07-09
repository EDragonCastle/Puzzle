using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UIElements;

public class ElementData : MonoBehaviour
{
    public ElementType elementType;

    private int xIndex;
    private int yIndex;

    public bool isMatched;
    private Vector2 currentPos;
    private Vector2 targetPos;

    public bool isMoving;

    // lamda로 할 수 있는지 확인해보자.
    public ElementData(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;
    }

    public Vector2Int GetPosition() => new Vector2Int(xIndex, yIndex);

    public void SetPosition(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;
    }

    // Couroutine인데 Job System을 이용해서 할 수 있을까?
    public void MoveToTarget(Vector3 targetPosition)
    {
        StartCoroutine(MoveCoroutine(targetPosition));
    }

    // UI Test
    public void UIMoveToTarget(Vector2 targetPosition)
    {
        StartCoroutine(UIMoveCoroutine(targetPosition));
    }

    // Coroutine
    private IEnumerator MoveCoroutine(Vector3 targetPosition)
    {
        isMoving = true;
        float duration = 0.2f;

        Vector2 startPosition = new Vector2 (transform.position.x, transform.position.y);
        float elaspedTime = 0f;

        while(elaspedTime < duration)
        {
            float t = elaspedTime / duration;
            transform.position = Vector2.Lerp(startPosition, targetPosition, t);
            elaspedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }

    // UI 전용 Coroutine
    private IEnumerator UIMoveCoroutine(Vector2 targetPosition)
    {
        isMoving = true;
        float duration = 0.2f;

        //Vector2 startPosition = new Vector2(transform.position.x, transform.position.y);

        var rectTransform = this.gameObject.GetComponent<RectTransform>();
        Vector2 startPosition = rectTransform.anchoredPosition;
        float elaspedTime = 0f;

        while (elaspedTime < duration)
        {
            float t = elaspedTime / duration;
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            elaspedTime += Time.deltaTime;
            yield return null;
        }


        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = targetPosition;
            rectTransform.anchorMin = Vector2.one * 0.5f;
            rectTransform.anchorMax = Vector2.one * 0.5f;
            rectTransform.pivot = Vector2.one * 0.5f;
        }
        isMoving = false;
    }
}

public enum ElementType
{
    Red, Blue, Green, Yellow, None,
}
