using UnityEngine;
using Spine.Unity;

/// <summary>
/// Spine Animation �� CloudPot Animation Object
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
        // skeleton�� �ڵ忡�� ����� �� �ֵ��� �����´�.
        skeletonAnimation = GetComponent<SkeletonGraphic>();
        skeleton = skeletonAnimation.Skeleton;
        animationState = skeletonAnimation.AnimationState;

        // �ʱ� Animation�� ������ Animation Name
        cloudPotAnimator = "rain";
    }

    // EventManager ���
    private void OnEnable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Subscription(ChannelInfo.MatchSuccess, HandleEvent);
        eventManager.Subscription(ChannelInfo.MatchFail, HandleEvent);
    }

    // EventManager ����
    private void OnDisable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.MatchSuccess, HandleEvent);
        eventManager.Unsubscription(ChannelInfo.MatchFail, HandleEvent);
    }

    /// <summary>
    /// Event Manager�� ����� IChannel Interface
    /// </summary>
    /// <param name="channel">ä�� ����</param>
    /// <param name="information">����� Object ����</param>
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

    // Match �����Ҷ��� Add�� �ϰ� �����ϸ� Set�� ����.
    private void OnPlayingAnimationEnd(CloudAnimation animationName)
    {
        // Add Animation�� ������ �� �ְ� �Ѵ�.
        var trackEntry = AddAnimation(0, animationName, false, 0);
        
        // Animation�� ������ Idle�� �ǵ��ư���.
        trackEntry.Complete += OnPlayingAnimationComplete;

        // ���� ����ǰ� �ִ� Animation�� �����´�.
        var currentTrack = animationState.GetCurrent(0);

        // �̸��� �ٸ��� animation�� �ٲ���.
        if(currentTrack.Animation.Name != CloudAnimationToString(animationName))
        {
            // �ش� Animation���� �ٲٰ� �ٽ� Idle�� �ǵ�����.
            trackEntry = SetAnimation(0, animationName, false);
            trackEntry.Complete += OnPlayingAnimationComplete;
            return;
        }

        // Track Animation�� ��ȸ�ϸ鼭 Next�� �����ϸ� Idle�� �ǵ��ư��� Event�� �����ϰ� �������� �ʴٸ� event�� �������� �ʴ´�.
        while(currentTrack != null)
        {
            if(currentTrack.Next != null)
                currentTrack.Complete -= OnPlayingAnimationComplete;
            currentTrack = currentTrack.Next;
        }
    }

    // Animation�� �� ������ �� Idle�� �ٽ� �ǵ��� ���� �Ѵ�.
    private void OnPlayingAnimationComplete(Spine.TrackEntry trackEntry)
    {
        SetAnimation(0, CloudAnimation.Rain, true);
    }

    // Porting
    private Spine.TrackEntry SetAnimation(int track, CloudAnimation animationName, bool loop) => animationState.SetAnimation(track, CloudAnimationToString(animationName), loop);
    private Spine.TrackEntry AddAnimation(int track, CloudAnimation animationName, bool loop, float delay) => animationState.AddAnimation(track, CloudAnimationToString(animationName), loop, delay);

    /// <summary>
    /// Enum To Animation �̸�
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