using UnityEngine;
using Spine.Unity;

public class SwingGirl : MonoBehaviour, IChannel
{
    [SpineAnimation]
    public string swingGirlAnimator;

    SkeletonGraphic skeletonAnimation;
    Spine.AnimationState animationState;
    Spine.Skeleton skeleton;

    private void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonGraphic>();
        skeleton = skeletonAnimation.Skeleton;
        animationState = skeletonAnimation.AnimationState;
        SetAnimation(0, SwingGirlAnimation.Swing, true);
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

    private Spine.TrackEntry SetAnimation(int track, SwingGirlAnimation animationName, bool loop) => animationState.SetAnimation(track, SwingGirlAnimationToString(animationName), loop);
    private Spine.TrackEntry AddAnimation(int track, SwingGirlAnimation animationName, bool loop, float delay) => animationState.AddAnimation(track, SwingGirlAnimationToString(animationName), loop, delay);

    private void SetAnimationIfDifferent(int track, SwingGirlAnimation animationName, bool loop)
    {
        Spine.TrackEntry currentTrackEntry = animationState.GetCurrent(track);

        if (currentTrackEntry == null || currentTrackEntry.Animation.Name != SwingGirlAnimationToString(animationName))
            SetAnimation(track, animationName, loop);
    }

    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch (channel)
        {
            case ChannelInfo.MatchSuccess:
                // Animation 실행
                SetAnimationIfDifferent(0, SwingGirlAnimation.Swing, true);
                SetAnimationIfDifferent(1, SwingGirlAnimation.EyeBlink_Long, true);
                SetAnimationIfDifferent(2, SwingGirlAnimation.Stars, true);
                SetAnimationIfDifferent(3, SwingGirlAnimation.Wing_And_Feet, true);
                break;
            case ChannelInfo.MatchFail:
                // Animation 실행
                SetAnimationIfDifferent(0, SwingGirlAnimation.Wind_Idle, true);
                SetAnimationIfDifferent(1, SwingGirlAnimation.EyeBlink, true);
                SetAnimationIfDifferent(2, SwingGirlAnimation.Stars, false);
                SetAnimationIfDifferent(3, SwingGirlAnimation.Wing_Flap, true);
                break;
        }
    }

    private string SwingGirlAnimationToString(SwingGirlAnimation swingGirlAnimation)
    {
        switch(swingGirlAnimation)
        {
            case SwingGirlAnimation.EyeBlink:
                swingGirlAnimator = "eyeblink";
                break;
            case SwingGirlAnimation.EyeBlink_Long:
                swingGirlAnimator = "eyeblink-long";
                break;
            case SwingGirlAnimation.Stars:
                swingGirlAnimator = "stars";
                break;
            case SwingGirlAnimation.Swing:
                swingGirlAnimator = "swing";
                break;
            case SwingGirlAnimation.Wind_Idle:
                swingGirlAnimator = "wind-idle";
                break;
            case SwingGirlAnimation.Wing_And_Feet:
                swingGirlAnimator = "wings-and-feet";
                break;
            case SwingGirlAnimation.Wing_Flap:
                swingGirlAnimator = "wing-flap";
                break;
        }

        return swingGirlAnimator;
    }

}

public enum SwingGirlAnimation
{
    EyeBlink,
    EyeBlink_Long,
    Stars,
    Swing,
    Wind_Idle,
    Wing_Flap,
    Wing_And_Feet,
}
