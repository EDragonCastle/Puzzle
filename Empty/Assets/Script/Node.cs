using UnityEngine;

public class Node 
{
    // ���� ����� ������ ������� ���� ������
    public bool isUsable;
    public GameObject element;

    public Node(bool _isUsable, GameObject _element)
    {
        isUsable = _isUsable;
        element = _element;
    }
}
