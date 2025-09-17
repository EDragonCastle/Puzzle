using Unity.Jobs;

/// <summary>
/// Match에 성공했을 때 실행하는 Job System이다.
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
/// Match에 실패했을 때 실행하는 Job System이다.
/// </summary>
public struct MatchFailJob : IJob
{
    public void Execute()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.MatchFail);
    }
}