using UnityEngine;

/// <summary>
/// Mobile 상태에 따라 행동을 하게 관리하는 Class
/// </summary>
public class MobileStatus : MonoBehaviour
{
    /// <summary>
    /// App이 종료됐을 때 App을 종료한다.
    /// </summary>
    private void OnApplicationQuit()
    {
        Application.Quit();
    }
}
