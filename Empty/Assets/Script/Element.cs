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
        // 하지만 Board Class랑 지금 이 Class랑 Decoupling 되어야 한다.
        // Board Class에 있는 private void SelectElement(GameObject _selectObject)에다 매개 변수로 this.GameObject를 넣을 것이다.
        Debug.Log($"{index.x} {index.y}");
        onClickElement?.Invoke(this.gameObject);
    }
}