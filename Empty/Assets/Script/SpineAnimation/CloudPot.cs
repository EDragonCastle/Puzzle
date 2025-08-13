using UnityEngine;
using Spine.Unity;

public class CloudPot : MonoBehaviour, IChannel
{
    [SpineAnimation]
    public string cloudPotAnimator;

    SkeletonGraphic skeletonAnimation;
    Spine.AnimationState animationState;
    Spine.Skeleton skeleton;

    private void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonGraphic>();
        skeleton = skeletonAnimation.Skeleton;
        animationState = skeletonAnimation.AnimationState;
        cloudPotAnimator = "rain";
    }

    private void OnEnable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Subscription(ChannelInfo.MatchSuccess, HandleEvent);
        eventManager.Subscription(ChannelInfo.MatchFail, HandleEvent);
    }

    private void OnDisable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.MatchSuccess, HandleEvent);
        eventManager.Unsubscription(ChannelInfo.MatchFail, HandleEvent);
    }

    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch(channel)
        {
            case ChannelInfo.MatchSuccess:
                // Match ������ ������ Animation�� �߰��ȴ�. ���������� Rain���� �ٽ� �ǵ��� ����.
                OnPlayingAnimationEnd(CloudAnimation.PlayingRain);
                break;
            case ChannelInfo.MatchFail:
                // Match �����Ҷ����� Animation�� �߰��ȴ�. ���������� �ٽ� Rain���� ���ư���.
                OnPlayingAnimationEnd(CloudAnimation.PotFollowRain);
                break;
        }
    }

    // �̷��ϱ� �ʹ� ���ο��� �����Ҷ��� Add�� �ϰ� �����ϸ� Set�� ����.
    private void OnPlayingAnimationEnd(CloudAnimation animationName)
    {
        var trackEntry = AddAnimation(0, animationName, false, 0);
        trackEntry.Complete += OnPlayingAnimationComplete;

        var currentTrack = animationState.GetCurrent(0);

        // �̸��� �ٸ��� animation�� �ٲ���.
        if(currentTrack.Animation.Name != CloudAnimationToString(animationName))
        {
            trackEntry = SetAnimation(0, animationName, false);
            trackEntry.Complete += OnPlayingAnimationComplete;
            return;
        }

        while(currentTrack != null)
        {
            if(currentTrack.Next != null)
                currentTrack.Complete -= OnPlayingAnimationComplete;
            currentTrack = currentTrack.Next;
        }
    }

    private void OnPlayingAnimationComplete(Spine.TrackEntry trackEntry)
    {
        SetAnimation(0, CloudAnimation.Rain, true);
    }

    private Spine.TrackEntry SetAnimation(int track, CloudAnimation animationName, bool loop) => animationState.SetAnimation(track, CloudAnimationToString(animationName), loop);
    private Spine.TrackEntry AddAnimation(int track, CloudAnimation animationName, bool loop, float delay) => animationState.AddAnimation(track, CloudAnimationToString(animationName), loop, delay);

    private string CloudAnimationToString(CloudAnimation cloudAnimation)
    {
        switch(cloudAnimation)
        {
            case CloudAnimation.Rain:
                cloudPotAnimator = "rain";
                break;
            case CloudAnimation.PotFollowRain:
                cloudPotAnimator = "pot-moving-followed-by-rain";
                break;
            case CloudAnimation.PlayingRain:
                cloudPotAnimator = "playing-in-the-rain";
                break;
        }

        return cloudPotAnimator;
    }
}

public enum CloudAnimation
{
    Rain,
    PotFollowRain,
    PlayingRain,
}