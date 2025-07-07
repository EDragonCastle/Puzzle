/// <summary>
/// 인스턴스에 쉽게 접근하기 위해 사용한 중재자 패턴이다.
/// 0702 LYS
/// </summary>
public static class Locator 
{
    // Log Manager
    private static LogManager log;
    public static LogManager GetLogManager() => log;
    public static void ProvideLogManager(LogManager _log) => log = _log;
}
