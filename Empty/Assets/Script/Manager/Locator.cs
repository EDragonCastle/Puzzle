public static class Locator
{
    // factory
    private static Factory factory;
    public static void ProvideFactory(Factory _factory) => factory = _factory;
    public static Factory GetFactory() => factory;

    // object pool
    // �ٵ� �ʿ� ����δ�. ���� Effect�� �׷��� �� ���� �ʿ� ���� ���̴� �������� �𸥴�.
    // 

    // observer

    // ...
}