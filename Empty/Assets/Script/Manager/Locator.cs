
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
/// Locator Pattern으로 Singleton Pattern의 장점인 전역 접근이 가능하다.
/// 모든 Manager를 가지고 있는 객체다.
/// </summary>
/// <typeparam name="T">Manager Type만 등록해야 한다.</typeparam>
public static class Locator<T>
{
    private static T manager;
    // T Manager 등록
    public static void Provide(T _manager) => manager = _manager;
    // Manager 가져오기
    public static T Get() => manager;
}