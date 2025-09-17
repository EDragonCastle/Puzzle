using UnityEngine;
using Spine.Unity;

/// <summary>
/// Spine Animation 중 CloudPot Animation Object
/// </summary>
public class CloudPot : MonoBehaviour, IChannel
{
    [SpineAnimation]
    public string cloudPotAnimator;

    SkeletonGraphic skeletonAnimation;
    Spine.AnimationState animationState;
    Spine.Skeleton skeleton;

    private void Awake()
    {
        // skeleton을 코드에서 사용할 수 있도록 가져온다.
        skeletonAnimation = GetComponent<SkeletonGraphic>();
        skeleton = skeletonAnimation.Skeleton;
        animationState = skeletonAnimation.AnimationState;

        // 초기 Animation을 진행할 Animation Name
        cloudPotAnimator = "rain";
    }

    // EventManager 등록
    private void OnEnable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Subscription(ChannelInfo.MatchSuccess, HandleEvent);
        eventManager.Subscription(ChannelInfo.MatchFail, HandleEvent);
    }

    // EventManager 해지
    private void OnDisable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.MatchSuccess, HandleEvent);
        eventManager.Unsubscription(ChannelInfo.MatchFail, HandleEvent);
    }

    /// <summary>
    /// Event Manager에 사용할 IChannel Interface
    /// </summary>
    /// <param name="channel">채널 정보</param>
    /// <param name="information">사용할 Object 내용</param>
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

    // Match 성공할때만 Add를 하고 실패하면 Set을 하자.
    private void OnPlayingAnimationEnd(CloudAnimation animationName)
    {
        // Add Animation을 실행할 수 있게 한다.
        var trackEntry = AddAnimation(0, animationName, false, 0);
        
        // Animation이 끝나면 Idle로 되돌아간다.
        trackEntry.Complete += OnPlayingAnimationComplete;

        // 현재 실행되고 있는 Animation을 가져온다.
        var currentTrack = animationState.GetCurrent(0);

        // 이름이 다르면 animation을 바꾸자.
        if(currentTrack.Animation.Name != CloudAnimationToString(animationName))
        {
            // 해당 Animation으로 바꾸고 다시 Idle로 되돌린다.
            trackEntry = SetAnimation(0, animationName, false);
            trackEntry.Complete += OnPlayingAnimationComplete;
            return;
        }

        // Track Animation을 순회하면서 Next가 존재하면 Idle도 되돌아가는 Event를 제거하고 존재하지 않다면 event를 제거하지 않는다.
        while(currentTrack != null)
        {
            if(currentTrack.Next != null)
                currentTrack.Complete -= OnPlayingAnimationComplete;
            currentTrack = currentTrack.Next;
        }
    }

    // Animation이 다 끝났을 때 Idle로 다시 되돌아 가게 한다.
    private void OnPlayingAnimationComplete(Spine.TrackEntry trackEntry)
    {
        SetAnimation(0, CloudAnimation.Rain, true);
    }

    // Porting
    private Spine.TrackEntry SetAnimation(int track, CloudAnimation animationName, bool loop) => animationState.SetAnimation(track, CloudAnimationToString(animationName), loop);
    private Spine.TrackEntry AddAnimation(int track, CloudAnimation animationName, bool loop, float delay) => animationState.AddAnimation(track, CloudAnimationToString(animationName), loop, delay);

    /// <summary>
    /// Enum To Animation 이름
    /// </summary>
    /// <param name="cloudAnimation">Animation Type</param>
    /// <returns></returns>
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

/// <summary>
/// Cloud Animation Enum Information
/// </summary>
public enum CloudAnimation
{
    Rain,
    PotFollowRain,
    PlayingRain,
}