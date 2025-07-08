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
}

public enum ElementType
{
    Red, Blue, Green, Yellow, None,
}
