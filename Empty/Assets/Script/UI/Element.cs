using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Puzzle Element를 담고 있는 Class
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class Element : MonoBehaviour, IPointerDownHandler, IUIElement, IChannel
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

    public void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
    }

    // Event 등록
    private void OnEnable()
    {
        var eventManager = Locator<EventManager>.Get();
        var materialManager = Locator<MaterialManager>.Get();
        materialManager.ChangeCustomDirectionLightInfo(this.gameObject, new LightInfo() { color = Vector4.one, direction = Vector4.zero });
        eventManager.Subscription(ChannelInfo.LightInfo, HandleEvent);
    }

    // Event 해지
    private void OnDisable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.LightInfo, HandleEvent);
    }

    /// <summary>
    /// Object를 Click 했을 때 나오는 메서드
    /// </summary>
    /// <param name="eventData">click 했을 때</param>
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"{elementInfo.position.x} {elementInfo.position.y}");
        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.Select, this.gameObject);
        var soundManager = Locator<SoundManager>.Get();
        soundManager.PlaySFX(SFX.Pop3);
    }

    /// <summary>
    /// Event Manager에 사용할 IChannel Interface
    /// </summary>
    /// <param name="channel">채널 정보</param>
    /// <param name="information">사용할 Object 내용</param>
    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch(channel)
        {
            case ChannelInfo.LightInfo:
                // object type을 LightInfo로 변경
                if(information is LightInfo lightInfo)
                    ChangeLightMaterial(lightInfo);
                break;
        }
        
    }

    /// <summary>
    /// Material의 정보를 변경하는 메서드
    /// </summary>
    /// <param name="lightInfo">Light 정보</param>
    private void ChangeLightMaterial(LightInfo lightInfo)
    {
        var materialManager = Locator<MaterialManager>.Get();
        materialManager.ChangeCustomDirectionLightInfo(this.gameObject, lightInfo);
    }
}

/// <summary>
/// 위치와 색상과 방문 여부를 담고 있는 Struct
/// </summary>
[System.Serializable]
public struct ElementInfo
{
    public Vector2 position;
    public int color;
    public bool isVisits;
}
