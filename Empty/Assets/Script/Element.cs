using UnityEngine;
using UnityEngine.EventSystems;
using System;

// RectTransform�� �ݵ�� �־�� �ϴµ� �䱸�ϴ� �� �־ ���� �ʳ�?
[RequireComponent(typeof(RectTransform))]
public class Element : MonoBehaviour, IPointerDownHandler
{
    // Color�� Position�̶� isVisits�� �׳� �����ص� ��������
    // Rect Transform�� �ٷ� �����ؾ��� �� ����.
    private RectTransform uiPosition;

    // �̰� �̿��� ���̴�.
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