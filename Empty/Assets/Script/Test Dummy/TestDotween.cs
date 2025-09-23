using UnityEngine;
using DG.Tweening;

public class TestDotween : MonoBehaviour
{
    [SerializeField]
    private GameObject testObject;

    void Start()
    {
        // paremeter 1 ���� ����, 2 ���� �� ��� ���� ��Ȳ�� �ڵ����� ó�� 3. ���� �޽��� ���
        // Capacity Tweener ����, Seqence ���� ����, �ѵ� �ʰ��ص� �ڵ����� �����.

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
