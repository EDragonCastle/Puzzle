using UnityEngine;

/// <summary>
/// Game Manager�� ó���� ����Ǿ�� �Ѵ�.
/// 0702 LYS
/// </summary>
[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private LogManager logManager;

    private void Awake() {
        // Locator�� Log Manager ���
        if(logManager != null) {
            Locator.ProvideLogManager(logManager);        
        }
        else {
            // logManager�� �����ϰ� �� �־ ���� ���� ���ݾ�.
            // �װ� ���� �ϱ� ���� ���� �ʿ��ѵ� ���߿� �߰�����.
        }
    }
}
