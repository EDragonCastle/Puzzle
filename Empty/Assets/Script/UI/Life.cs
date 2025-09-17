using UnityEngine;

/// <summary>
/// Life ������ ���� Class��.
/// </summary>
public class Life : MonoBehaviour
{
    [SerializeField]
    private GameObject[] lifes;
    // �ִ� ���
    private int maxLife = 5;

    /// <summary>
    /// Life ������
    /// </summary>
    /// <param name="_maxLife">�ִ� ���</param>
    /// <param name="_lifes">life ������ ��� Array</param>
    /// <param name="parent">Life object�� ���� ����</param>
    public Life(int _maxLife, GameObject[] _lifes, GameObject parent = null)
    {
        maxLife = _maxLife;
        lifes = _lifes;

        // Lifes�� ��ȸ�Ѵ�.
        foreach(var life in lifes)
        {
            // life�� Ȱ��ȭ ��Ű�� �θ� �ִ´�.
            life.SetActive(true);
            life.transform.SetParent(parent.transform);
            // RectTransform�� ũ�⵵ �缳���Ѵ�.
            RectTransform rectTransform = life.GetComponent<RectTransform>();
            rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }

        // ������ ��ȸ�ϸ鼭 life�� ��� ���д�.
        for(int i = lifes.Length - 1; i > maxLife - 1; i--)
        {
            lifes[i].SetActive(false);
        }
    }

    /// <summary>
    /// Life�� �󸶳� ���Ҵ��� Ȯ���Ѵ�.
    /// </summary>
    /// <param name="_count">����</param>
    /// <returns>bool type return</returns>
    public bool DestoryLife(int _count)
    {
        int length = maxLife - _count;

        if (length > 0)
        {
            lifes[length].SetActive(false);
            return true;
        }
        else
            return false;
    }
}
