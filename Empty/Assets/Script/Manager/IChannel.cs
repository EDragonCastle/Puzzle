public enum ChannelInfo
{
    Select,
    Score,
    End,
}

public interface IChannel
{
    public void HandleEvent(ChannelInfo channel, object information = null);
}
