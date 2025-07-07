using UnityEngine;

/// <summary>
/// �ش� ��ġ�� �ִ� Object�� Screen Size�� ���� ��Ȯ�� ��ġ�� �־�� �ϱ� ���� ī�޶� �����ؾ��Ѵ�.
/// 0702 LYS
/// </summary>
public class CameraResolution : MonoBehaviour 
{
    private void Awake() {
        Camera cam = GetComponent<Camera>();

        // cam�� Null�� �ƴ϶�� �����Ѵ�.
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
