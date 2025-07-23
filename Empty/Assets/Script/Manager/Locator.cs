
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

public static class Locator<T>
{
    private static T manager;
    public static void Provide(T _manager) => manager = _manager;
    public static T Get() => manager;
}