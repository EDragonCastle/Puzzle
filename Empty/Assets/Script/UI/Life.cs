using UnityEngine;
using UnityEngine.UI;

public class Life : MonoBehaviour
{
    [SerializeField]
    private GameObject[] lifes;
    private int maxLife = 5;

    public Life(int _maxLife, GameObject[] _lifes, GameObject parent = null)
    {
        maxLife = _maxLife;
        lifes = _lifes;

        foreach(var life in lifes)
        {
            life.SetActive(true);
            life.transform.SetParent(parent.transform);
            // �̰��� ���� �� ������? �׳� Task�� ���� ������ �� ������?
            RectTransform rectTransform = life.GetComponent<RectTransform>();
            rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }

        for(int i = lifes.Length - 1; i > maxLife - 1; i--)
        {
            lifes[i].SetActive(false);
        }
    }

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
