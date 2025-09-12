using System.Collections.Generic;
using System;

public class EventManager 
{
    private Dictionary<ChannelInfo, List<Action<ChannelInfo, object>>> channels;

    public EventManager()
    {
        channels = new Dictionary<ChannelInfo, List<Action<ChannelInfo, object>>>();
    }

    public void Subscription(ChannelInfo channelType, Action<ChannelInfo, object> channel) 
    {
        if(channels.ContainsKey(channelType))
        {
            channels[channelType].Add(channel);
        }
        else
        {
            var channelList = new List<Action<ChannelInfo, object>>();
            channelList.Add(channel);
            channels.Add(channelType, channelList);
        }
    }

    public void Unsubscription(ChannelInfo channelType)
    {
        if(channels.ContainsKey(channelType))
        {
            channels.Remove(channelType);
        }
    }

    public void Unsubscription(ChannelInfo channelType, Action<ChannelInfo, object> channel)
    {
        if(channels.ContainsKey(channelType))
        {
            var channelList = channels[channelType];
            for(int i = 0; i < channelList.Count; i++)
            {
                if (channelList[i] == channel)
                {
                    channelList.RemoveAt(i);
                    return;
                }
            }
        }
    }

    public void Notify(ChannelInfo channelType, object eventInfo = null)
    {
        if(channels.ContainsKey(channelType))
        {
            foreach(var channel in channels[channelType])
            {
                channel?.Invoke(channelType, eventInfo);
            }
        }
    }
}

