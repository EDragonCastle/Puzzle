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
                // Match 성공할 때마다 Animation이 추가된다. 마지막에는 Rain으로 다시 되돌아 간다.
                OnPlayingAnimationEnd(CloudAnimation.PlayingRain);
                break;
            case ChannelInfo.MatchFail:
                // Match 실패할때마다 Animation이 추가된다. 마지막에는 다시 Rain으로 돌아간다.
                OnPlayingAnimationEnd(CloudAnimation.PotFollowRain);
                break;
        }
    }

    // 이러니까 너무 별로여서 성공할때만 Add를 하고 실패하면 Set을 하자.
    private void OnPlayingAnimationEnd(CloudAnimation animationName)
    {
        var trackEntry = AddAnimation(0, animationName, false, 0);
        trackEntry.Complete += OnPlayingAnimationComplete;

        var currentTrack = animationState.GetCurrent(0);

        // 이름이 다르면 animation을 바꾸자.
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