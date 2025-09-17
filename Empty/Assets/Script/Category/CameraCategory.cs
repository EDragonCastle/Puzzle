using UnityEngine;

/// <summary>
/// 모든 Camera 정보를 담고 있는 Class
/// </summary>
public class CameraCategory : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    /// <summary>
    /// Enum To Camera
    /// </summary>
    /// <param name="_category">카메라 정보</param>
    /// <returns>Camera</returns>
    public Camera GetCamera(CameraSetting _category)
    {
        Camera camera = null;
        switch(_category)
        {
            case CameraSetting.Main:
                camera = mainCamera;
                break;
            case CameraSetting.End:
                Debug.LogError("None Camera");
                break;
            default:
                Debug.LogError("None Camera");
                break;
        }
        return camera;
    }
}

/// <summary>
/// Camera Setting을 담고 있는 Enum
/// </summary>
public enum CameraSetting
{
    Main,
    End,
}