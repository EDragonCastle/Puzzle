using Unity.Jobs;

public struct MatchSuccessJob : IJob
{
    public void Execute()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.MatchSuccess);
    }
}

public struct MatchFailJob : IJob
{
    public void Execute()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.MatchFail);
    }
}