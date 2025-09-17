using Unity.Jobs;

/// <summary>
/// Match�� �������� �� �����ϴ� Job System�̴�.
/// </summary>
public struct MatchSuccessJob : IJob
{
    public void Execute()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.MatchSuccess);
    }
}

/// <summary>
/// Match�� �������� �� �����ϴ� Job System�̴�.
/// </summary>
public struct MatchFailJob : IJob
{
    public void Execute()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.MatchFail);
    }
}