using UnityEngine;
using Spine.Unity;

/// <summary>
/// Spine Animatioin �� SwingGirl Animation Object
/// </summary>
public class SwingGirl : MonoBehaviour, IChannel
{
    [SpineAnimation]
    public string swingGirlAnimator;

    [SerializeField]
    private bool isTesting = false;

    SkeletonGraphic skeletonAnimation;
    Spine.AnimationState animationState;
    Spine.Skeleton skeleton;

    private void Awake()
    {
        // skeleton�� �ڵ忡�� ����� �� �ֵ��� �����´�.
        skeletonAnimation = GetComponent<SkeletonGraphic>();
        skeleton = skeletonAnimation.Skeleton;
        animationState = skeletonAnimation.AnimationState;
        
        // �ʱ� Animation Setting
        SetAnimation(0, SwingGirlAnimation.Swing, true);
    }

    // EventManager ���
    private void OnEnable()
    {
        if(!isTesting)
        {
            var eventManager = Locator<EventManager>.Get();
            eventManager.Subscription(ChannelInfo.MatchSuccess, HandleEvent);
            eventManager.Subscription(ChannelInfo.MatchFail, HandleEvent);
        }
    }

    // EventManager ����
    private void OnDisable()
    {
        if(!isTesting)
        {
            var eventManager = Locator<EventManager>.Get();
            eventManager.Unsubscription(ChannelInfo.MatchSuccess, HandleEvent);
            eventManager.Unsubscription(ChannelInfo.MatchFail, HandleEvent);
        }
    }

    // Porting
    private Spine.TrackEntry SetAnimation(int track, SwingGirlAnimation animationName, bool loop) => animationState.SetAnimation(track, SwingGirlAnimationToString(animationName), loop);
    private Spine.TrackEntry AddAnimation(int track, SwingGirlAnimation animationName, bool loop, float delay) => animationState.AddAnimation(track, SwingGirlAnimationToString(animationName), loop, delay);

    /// <summary>
    /// Animation�� �ٸ� ���� Animation���� �ٲ�� �Լ�
    /// </summary>
    /// <param name="track">���� Track</param>
    /// <param name="animationName">Animation �̸�</param>
    /// <param name="loop">Loop ����</param>
    private void SetAnimationIfDifferent(int track, SwingGirlAnimation animationName, bool loop)
    {
        Spine.TrackEntry currentTrackEntry = animationState.GetCurrent(track);

        if (currentTrackEntry == null || currentTrackEntry.Animation.Name != SwingGirlAnimationToString(animationName))
            SetAnimation(track, animationName, loop);
    }

    /// <summary>
    /// Event Manager�� ����� IChannel Interface
    /// </summary>
    /// <param name="channel">ä�� ����</param>
    /// <param name="information">����� Object ����</param>
    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch (channel)
        {
            case ChannelInfo.MatchSuccess:
                // Animation ������ ����
                SetAnimationIfDifferent(0, SwingGirlAnimation.Swing, true);
                SetAnimationIfDifferent(1, SwingGirlAnimation.EyeBlink_Long, true);
                SetAnimationIfDifferent(2, SwingGirlAnimation.Stars, true);
                SetAnimationIfDifferent(3, SwingGirlAnimation.Wing_And_Feet, true);
                break;
            case ChannelInfo.MatchFail:
                // Animation ���н� ����
                SetAnimationIfDifferent(0, SwingGirlAnimation.Wind_Idle, true);
                SetAnimationIfDifferent(1, SwingGirlAnimation.EyeBlink, true);
                SetAnimationIfDifferent(2, SwingGirlAnimation.Stars, false);
                SetAnimationIfDifferent(3, SwingGirlAnimation.Wing_Flap, true);
                break;
        }
    }

    /// <summary>
    /// Enum To Animation �̸�
    /// </summary>
    /// <param name="swingGirlAnimation">Animation Type</param>
    /// <returns></returns>
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

/// <summary>
/// SwingGirl Animation Enum Information
/// </summary>
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
