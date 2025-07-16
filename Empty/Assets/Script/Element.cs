using UnityEngine;
using UnityEngine.EventSystems;
using System;
public class Element : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private Vector2 index;
    public void SetPosition(Vector2 _value) => index = _value;
    public void SetPosition(int _x, int _y) => index = new Vector2(_x, _y);
    public Vector2 GetPosition() => index;

    public static event Action<GameObject> onClickElement;

    public void OnPointerDown(PointerEventData eventData)
    {
        // ������ Board Class�� ���� �� Class�� Decoupling �Ǿ�� �Ѵ�.
        // Board Class�� �ִ� private void SelectElement(GameObject _selectObject)���� �Ű� ������ this.GameObject�� ���� ���̴�.
        Debug.Log($"{index.x} {index.y}");
        onClickElement?.Invoke(this.gameObject);
    }
}