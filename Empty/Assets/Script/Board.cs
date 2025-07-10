using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;


public enum Direction
{
    Vertical, 
    Horizontal,
    None,
}


public class Board : MonoBehaviour
{
    [SerializeField]
    private int width = 6;
    
    [SerializeField]
    private int height = 8;

    private readonly int cellSize = 110;

    public GameObject[] prefab;
    public GameObject board;

    private int[,] colors;
    private GameObject[,] elements;

    // DFS�� �ʿ��� Index ������ ����
    List<(int x, int y)> tourIndex;
    List<(int x, int y)> saveIndex;
    HashSet<(int x, int y)> removeIndex;


    void Start()
    {
        Initalize();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            foreach (var remove in removeIndex)
            {
                if (elements[remove.x, remove.y].activeSelf)
                    elements[remove.x, remove.y].SetActive(false);
                else
                    elements[remove.x, remove.y].SetActive(true);
            }
        }
    }

    private void Initalize()
    {
        float startX = -((width * 0.5f - 0.5f) * cellSize);
        float startY = ((height * 0.5f - 0.5f) * cellSize);

        colors = new int[width, height];
        elements = new GameObject[width, height];
        tourIndex = new List<(int x, int y)>();
        saveIndex = new List<(int x, int y)>();
        removeIndex = new HashSet<(int x, int y)>();

        for (int y = 0; y < height; y++)
        {
            // �̰� left -> right�� �����ϰ� �ְ�,
            for (int x = 0; x < width; x++)
            {
                // prefab �������� random �� ��� �� object ������ �θ� ����
                var randIndex = Random.Range(0, prefab.Length);
                var newElement = Instantiate(prefab[randIndex]);
                newElement.transform.SetParent(board.transform, false);

                // Object���� Rect Transform�� �޾ƿ���, ��ġ ����
                var rectTransform = newElement.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(startX + x * cellSize, startY - y * cellSize);
                
                colors[x, y] = randIndex;
                elements[x, y] = newElement;
            }
        }

        CheckBoard();

        foreach(var remove in removeIndex)
        {
            Debug.Log($"Remove Data : {remove.x} {remove.y}");
        }
    }

    private void CheckBoard()
    {
        for(int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                DFS(i, j, colors[i, j], Direction.None, 1);
            }
        }
    }

    private void DFS(int x, int y, int _color, Direction _direction, int count)
    {
        // 1. ������ �Ѿ�ٸ� return
        if (x < 0 || width <= x || y < 0 || height <= y)
        {
            if (saveIndex.Count >= 3)
            {
                foreach (var save in saveIndex)
                {
                    removeIndex.Add(save);
                }
            }
            tourIndex.Clear();
            return;
        }

        // 3. ���� �ٸ��ٸ� return 
        if (colors[x, y] != _color)
        {
            if (saveIndex.Count >= 3)
            {
                foreach (var save in saveIndex)
                {
                    removeIndex.Add(save);
                }
            }
            tourIndex.Clear();
            return;
        }

        // ���� ���ǵ��� �������� �ʰ� ����ߴٸ� 
        // ���� ���� ���̴�.
        // count++�� �÷��߰ڴ�. �׸��� Vertical���� Horizontal���� Ȯ���ؾ� �� �� ������?
        count++;

        tourIndex.Add((x, y));
        // count�� 3�̻��̸� ���� ���̴�. ���� ������ �� �ƴѵ� ��� �ؾ��ұ�?

        // �������� �����. �� ó�� None�� �� vertical, Horizontal �Ѵ� �˻��ؼ� vertical���� count 3�� �̻��̿��� ������ ������, horizontal���� 3�� �̻��̸� ���� �ִ� data�� ����� �� �ִ�.
        if (tourIndex.Count >= 3)
        {
            // ref type�̶� tourIndex�� Clear �Ǹ� ���� �������.
            saveIndex = tourIndex;
        }

        // direction�� ���� ����?
        // vertical�̸� vertical�� ����, horizontal�̸� horizontal�� �����ؾ� �ϱ� �����̴�.
        // �ٵ� �� ó������ None���� ���ٵ�? None�� ���� ���� �� ������ �ϴµ�?

        // �׷��� �˻絵 �����ʰ� �Ʒ��� �˻��ص� �� �� ������ ���� 4������ �˻��ؾ� �� �ʿ並 �� �����ڴ�.
        // �ֳ��ϸ� �� ó�� �˻��� ���� ����ϴ� ���̱� �����̴�. ���߿� Swap �� ���� �����¿츦 ���� Ȯ���ؾ� �� �� ������
        // ������ �ʿ���� ���ϴ�.
        switch (_direction)
        {
            case Direction.Horizontal:
                // DFS Function ... Horizontal
                //DFS() ���� Y+1�� ������.
                DFS(x, y + 1, _color, Direction.Horizontal, count);
                break;
            case Direction.Vertical:
                // DFS Function ... Vertical
                // DFS() ���� x+1�� ������.
                DFS(x + 1, y, _color, Direction.Vertical, count);
                break;
            case Direction.None:
                // Horizontal, Vertical ���� �̰� for������ �ѹ��� �˻��Ϸ� �ߴµ� �׳� �ϳ��� �ִ°� �� ������ ���δ�.
                // DFS() X+1, Y+1�� ������.
                DFS(x + 1, y, _color, Direction.Vertical, count);

                // ���� �� �̹� ������� ������ �߰����ִ� ���̴�.
                tourIndex.Add((x, y));
                if (tourIndex.Count >= 3)
                {
                    saveIndex = tourIndex;
                    foreach (var save in saveIndex)
                    {
                        removeIndex.Add(save);
                    }
                }

                DFS(x, y + 1, _color, Direction.Horizontal, count);
                break;
        }
    }
}
