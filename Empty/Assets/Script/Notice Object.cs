using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class NoticeObject : MonoBehaviour, IPointerDownHandler
{

    public void OnPointerDown(PointerEventData eventData)
    {
        var log = Locator.GetLogManager();
        log.Info("Clicked Image " + this.gameObject.name);

        // Board�� �ִ� SelectElement �Լ��� �� Object�� �־�� �Ѵ�.
    }
}
