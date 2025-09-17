using UnityEngine;

/// <summary>
/// Life 정보를 담은 Class다.
/// </summary>
public class Life : MonoBehaviour
{
    [SerializeField]
    private GameObject[] lifes;
    // 최대 목숨
    private int maxLife = 5;

    /// <summary>
    /// Life 생성자
    /// </summary>
    /// <param name="_maxLife">최대 목숨</param>
    /// <param name="_lifes">life 정보가 담긴 Array</param>
    /// <param name="parent">Life object를 담을 공간</param>
    public Life(int _maxLife, GameObject[] _lifes, GameObject parent = null)
    {
        maxLife = _maxLife;
        lifes = _lifes;

        // Lifes를 순회한다.
        foreach(var life in lifes)
        {
            // life를 활성화 시키고 부모에 넣는다.
            life.SetActive(true);
            life.transform.SetParent(parent.transform);
            // RectTransform의 크기도 재설정한다.
            RectTransform rectTransform = life.GetComponent<RectTransform>();
            rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }

        // 역으로 순회하면서 life를 잠시 꺼둔다.
        for(int i = lifes.Length - 1; i > maxLife - 1; i--)
        {
            lifes[i].SetActive(false);
        }
    }

    /// <summary>
    /// Life가 얼마나 남았는지 확인한다.
    /// </summary>
    /// <param name="_count">개수</param>
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
