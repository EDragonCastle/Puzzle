using UnityEngine;
using DG.Tweening;

public class TestDotween : MonoBehaviour
{
    [SerializeField]
    private GameObject testObject;

    void Start()
    {
        // paremeter 1 재사용 여부, 2 삭제 될 경우 에외 상황을 자동으로 처리 3. 오류 메시지 기록
        // Capacity Tweener 개수, Seqence 개수 설정, 한도 초과해도 자동으로 만든다.

        DOTween.Init(false, true, LogBehaviour.Verbose).SetCapacity(200, 50);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            testObject.transform.DOScale(new Vector3(2f, 2f, 2f), 2)
                                .SetEase(Ease.OutQuint)
                                .SetLoops(4)
                                .OnComplete(() => { testObject.transform.DOScale(new Vector3(4f, 4f, 4f), 2).From(true); });
          
            
        }
    }
}
