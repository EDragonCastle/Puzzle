using UnityEngine;

/// <summary>
/// Direction Light 정보를 관리하고 있는 Class
/// </summary>
[RequireComponent(typeof(Light))]
public class LightRotation : MonoBehaviour
{
    private Light light;
    private Quaternion previousRotation;

    private void Start()
    {
        light = this.GetComponent<Light>();
        previousRotation = this.transform.rotation;
    }

    void Update()
    {
        // Rotion 정보를 관리하고 있다. -> 매 Frame마다 확인하고 있어서 좋지 않다.
        if(previousRotation != this.transform.rotation)
        {
            var eventManager = Locator<EventManager>.Get();
            var lightInfo = new LightInfo()
            {
                color = light.color,
                direction = -light.transform.forward
            };
            
            previousRotation = this.transform.rotation;
            eventManager.Notify(ChannelInfo.LightInfo, lightInfo);
        }
    }
}

/// <summary>
/// Light 정보를 담고 있는 Struct
/// </summary>
public struct LightInfo
{
    public Vector4 color;
    public Vector4 direction;
}
