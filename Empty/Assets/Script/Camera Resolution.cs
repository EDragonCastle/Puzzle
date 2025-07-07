using UnityEngine;

/// <summary>
/// 해당 위치에 있는 Object를 Screen Size에 따라 정확한 위치에 있어야 하기 위한 카메라 조절해야한다.
/// 0702 LYS
/// </summary>
public class CameraResolution : MonoBehaviour 
{
    private void Awake() {
        Camera cam = GetComponent<Camera>();

        // cam이 Null이 아니라면 실행한다.
        if(cam?.rect != null) {
            Rect rect = cam.rect;
            float windowHeight = ((float)Screen.width / Screen.height) / ((float)9 / 16);
            float windowWidth = 1f / windowHeight;

            if(windowHeight < 1) {
                rect.height = windowHeight;
                rect.y = (1f - windowHeight) / 2f;
            }
            else {
                rect.width = windowWidth;
                rect.x = (1f - windowWidth) / 2f;
            }

            cam.rect = rect;
        }
        else {
            var log = Locator.GetLogManager();
            log.Error($"{this.name} is None Camera Component.");
        }
    }
}
