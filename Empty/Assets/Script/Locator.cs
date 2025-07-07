/// <summary>
/// �ν��Ͻ��� ���� �����ϱ� ���� ����� ������ �����̴�.
/// 0702 LYS
/// </summary>
public static class Locator 
{
    // Log Manager
    private static LogManager log;
    public static LogManager GetLogManager() => log;
    public static void ProvideLogManager(LogManager _log) => log = _log;
}
