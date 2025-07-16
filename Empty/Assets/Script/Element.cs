using UnityEngine;

public class Element : MonoBehaviour
{
    [SerializeField]
    private Vector2 index;
    public void SetPosition(Vector2 _value) => index = _value;
    public void SetPosition(int _x, int _y) => index = new Vector2(_x, _y);
    public Vector2 GetPosition() => index;

}