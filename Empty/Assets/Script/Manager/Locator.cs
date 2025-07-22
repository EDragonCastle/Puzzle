public static class Locator
{
    // factory
    private static Factory factory;
    public static void ProvideFactory(Factory _factory) => factory = _factory;
    public static Factory GetFactory() => factory;

    // object pool
    // 근데 필요 없어보인다. 아직 Effect나 그런게 안 들어가서 필요 없어 보이는 것일지도 모른다.
    // 

    // observer

    // ...
}