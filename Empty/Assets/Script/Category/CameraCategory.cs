using UnityEngine;

/// <summary>
/// ��� Camera ������ ��� �ִ� Class
/// </summary>
public class CameraCategory : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    /// <summary>
    /// Enum To Camera
    /// </summary>
    /// <param name="_category">ī�޶� ����</param>
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
/// Camera Setting�� ��� �ִ� Enum
/// </summary>
public enum CameraSetting
{
    Main,
    End,
}