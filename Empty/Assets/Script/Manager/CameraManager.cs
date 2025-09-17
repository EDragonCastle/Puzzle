using UnityEngine;

/// <summary>
/// 카메라를 관리하고 있는 Maanger (실제 사용하지는 않고 있다.) 
/// </summary>
public class CameraManager 
{
    private CameraCategory category;
    #region CameraManager Structor
    public CameraManager(CameraCategory _category) => category = _category;
    #endregion
    public Camera GetCamera(CameraSetting cameraSetting) => category.GetCamera(cameraSetting);
}
