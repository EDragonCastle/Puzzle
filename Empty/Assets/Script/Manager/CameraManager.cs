using UnityEngine;

public class CameraManager 
{
    private CameraCategory category;
    #region CameraManager Structor
    public CameraManager(CameraCategory _category) => category = _category;
    #endregion
    public Camera GetCamera(CameraSetting cameraSetting) => category.GetCamera(cameraSetting);
}
