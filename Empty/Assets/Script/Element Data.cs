using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ElementData : MonoBehaviour
{
    public ElementType elementType;

    private int xIndex;
    private int yIndex;

    public bool isMatched;
    private Vector2 currentPos;
    private Vector2 targetPos;

    public bool isMoving;

    // lamda�� �� �� �ִ��� Ȯ���غ���.
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

    // Couroutine�ε� Job System�� �̿��ؼ� �� �� ������?
    public void MoveToTarget(Vector3 targetPosition)
    {
        StartCoroutine(MoveCoroutine(targetPosition));
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
}

public enum ElementType
{
    Red, Blue, Green, Yellow, None,
}
