using UnityEngine;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(RectTransform))]
public class Element : MonoBehaviour, IPointerDownHandler, IUIElement
{
    [SerializeField]
    private ElementInfo elementInfo;
    private RectTransform rectTransform;

    #region IUIElement Interface Methord
    public ElementInfo GetElementInfo() => elementInfo;
    public void SetElementInfo(ElementInfo info) => elementInfo = info;
    public RectTransform GetRectTransform() => rectTransform;
    public GameObject GetGameObject() => this.gameObject;
    #endregion

    public static event Action<IUIElement> onClickElement;

    public void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"{elementInfo.position.x} {elementInfo.position.y}");
        onClickElement?.Invoke(this.gameObject.GetComponent<IUIElement>());
    }
}

[System.Serializable]
public struct ElementInfo
{
    public Vector2 position;
    public int color;
    public bool isVisits;
}
