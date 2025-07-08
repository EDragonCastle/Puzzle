using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class NoticeObject : MonoBehaviour, IPointerDownHandler
{

    public void OnPointerDown(PointerEventData eventData)
    {
        var log = Locator.GetLogManager();
        log.Info("Clicked Image " + this.gameObject.name);

        // Board에 있는 SelectElement 함수에 이 Object를 넣어야 한다.
    }
}
