
/* Legacy Lacator
public static class Locator
{
    // factory
    private static Factory factory;
    public static void ProvideFactory(Factory _factory) => factory = _factory;
    public static Factory GetFactory() => factory;

    // observer
    private static EventManager eventManager;
    public static void ProvideEventManager(EventManager _eventManager) => eventManager = _eventManager;
    public static EventManager GetEventManager() => eventManager;
}
*/

/// <summary>
/// Locator Pattern���� Singleton Pattern�� ������ ���� ������ �����ϴ�.
/// ��� Manager�� ������ �ִ� ��ü��.
/// </summary>
/// <typeparam name="T">Manager Type�� ����ؾ� �Ѵ�.</typeparam>
public static class Locator<T>
{
    private static T manager;
    // T Manager ���
    public static void Provide(T _manager) => manager = _manager;
    // Manager ��������
    public static T Get() => manager;
}