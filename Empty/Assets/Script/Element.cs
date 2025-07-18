using UnityEngine;
using UnityEngine.EventSystems;
using System;

// RectTransform을 반드시 넣어야 하는데 요구하는 거 넣어도 되지 않나?
[RequireComponent(typeof(RectTransform))]
public class Element : MonoBehaviour, IPointerDownHandler
{
    // Color랑 Position이랑 isVisits는 그냥 적용해도 괜찮은데
    // Rect Transform은 바로 적용해야할 것 같다.
    private RectTransform uiPosition;

    // 이걸 이용할 것이다.
    [SerializeField]
    private ElementInfo elementInfo;

    #region Property RectTransform
    public RectTransform GetUIPosition() => uiPosition;
    public void SetUIPosition(RectTransform _uiPosition) => uiPosition = _uiPosition;
    #endregion

    #region Property ElementInfo
    public ElementInfo GetElementInfo() => elementInfo;
    public void SetElementInfo(ElementInfo info) => elementInfo = info;
    #endregion


    private void Awake()
    {
        uiPosition = this.GetComponent<RectTransform>();
    }

    public static event Action<GameObject> onClickElement;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"{elementInfo.position.x} {elementInfo.position.y}");
        onClickElement?.Invoke(this.gameObject);
    }
}

[System.Serializable]
public struct ElementInfo
{
    public Vector2 position;
    public int color;
    public bool isVisits;
}