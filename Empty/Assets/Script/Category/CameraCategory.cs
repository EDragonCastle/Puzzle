using UnityEngine;

public class CameraCategory : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

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

public enum CameraSetting
{
    Main,
    End,
}