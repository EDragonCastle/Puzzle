public enum ChannelInfo
{
    Select,
    Score,
    ResetScore,
    End,
}

/// <summary>
/// object가 value면 is keyword 사용 ref면 as keyword 사용
/// </summary>
public interface IChannel
{
    public void HandleEvent(ChannelInfo channel, object information = null);
}
