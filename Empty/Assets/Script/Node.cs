using UnityEngine;

public class Node 
{
    // 실제 사용할 것인지 사용하지 않을 것인지
    public bool isUsable;
    public GameObject element;

    public Node(bool _isUsable, GameObject _element)
    {
        isUsable = _isUsable;
        element = _element;
    }
}
