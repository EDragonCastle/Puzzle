using UnityEngine;

/// <summary>
/// Unity���� Log Level�� ��� ���� Manager��.
/// 07-02 LYS
/// </summary>
public class LogManager : MonoBehaviour
{
    // 1. Color�� Custom �� �� ������ ���ڴ�.
    // 2. �׸��� Debug������ ��������� ���ھ�.
    // 3. Release������ ����Ǽ��� �� ��!
    // 4. �׷��ٰ� Resource Folder�� ���� �ʿ�� ���� �� ����.
    // 4 Reason ���� �������� �ʿ���� ����̱� ��������!
    // 5. Editor �󿡼��� �ʿ��� ����̾�.

    public void Fatal(string message) => Debug.LogError($"<color=red>[FATAL]</color> {message}");
    public void Error(string message) => Debug.LogError($"<color=red>[ERROR]</color> {message}");
    public void Warning(string message) => Debug.LogWarning($"<color=yellow>[WARNING]</color> {message}");
    public void Info(string message) => Debug.Log($"<color=white>[INFO]</color> {message}");
}
