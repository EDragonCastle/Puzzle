using System.Collections.Generic;
using System;

/// <summary>
/// Event�� �����ϰ� �ִ� Manager
/// Observer Pattern�� �̿��ؼ� ���� Manager��.
/// </summary>
public class EventManager 
{
    // Key : Enum, Value : Action List
    private Dictionary<ChannelInfo, List<Action<ChannelInfo, object>>> channels;

    // EventManager ������
    public EventManager()
    {
        channels = new Dictionary<ChannelInfo, List<Action<ChannelInfo, object>>>();
    }

    /// <summary>
    /// Event ����(���)
    /// </summary>
    /// <param name="channelType">Enum Type</param>
    /// <param name="channel">ä��, object ���� ���� �Լ��� ���</param>
    public void Subscription(ChannelInfo channelType, Action<ChannelInfo, object> channel) 
    {
        // enum type ã�� ������ Value List�� �߰�, ������ List ����
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

    /// <summary>
    /// Event ��������
    /// Enum Type�� ���õ� ��� ������ �����Ѵ�.
    /// </summary>
    /// <param name="channelType">Enum Type</param>
    public void Unsubscription(ChannelInfo channelType)
    {
        // type�� ��ü ����
        if(channels.ContainsKey(channelType))
        {
            channels.Remove(channelType);
        }
    }

    /// <summary>
    /// Event ���� ���� Override 
    /// Enum Type�� �ִ� �Լ��� ���� ����
    /// </summary>
    /// <param name="channelType">Enum Type</param>
    /// <param name="channel">�ش� �Լ���</param>
    public void Unsubscription(ChannelInfo channelType, Action<ChannelInfo, object> channel)
    {
        // type �ȿ��� ��ȸ�ϸ鼭 ���� �Լ��� ������ ����
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
    
    /// <summary>
    /// ������ �� ����鿡�� �˸��� �Լ�
    /// Enum Type�� ���� �ִ� ����鿡�� Object ���� �ٲ���ٰ� �˷��ش�.
    /// </summary>
    /// <param name="channelType">Enum Type</param>
    /// <param name="eventInfo">�ٲ� Object ��</param>
    public void Notify(ChannelInfo channelType, object eventInfo = null)
    {
        // Type�� ���õ� �Լ��� ��ȸ�ϸ鼭 ���� �ٲ۴�.
        if(channels.ContainsKey(channelType))
        {
            foreach(var channel in channels[channelType])
            {
                channel?.Invoke(channelType, eventInfo);
            }
        }
    }
}

