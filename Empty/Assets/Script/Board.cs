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

    private float startX;
    private float startY;

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

    }

    private void Initalize()
    {
        startX = -((width * 0.5f - 0.5f) * cellSize);
        startY = ((height * 0.5f - 0.5f) * cellSize);

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

        // Board Setting
        while(removeIndex.Count > 0)
        {
            DestoryElement();
            ReFillElement();
            CheckBoard();
        }

        Debug.Log("Game Start");
    }

    private void DestoryElement()
    {
        Debug.Log("Destory Element");
        // Destory
        foreach (var remove in removeIndex)
        {
            Debug.Log($"Remove Data : {remove.x} {remove.y}");

            if (elements[remove.x, remove.y] != null)
            {
                Destroy(elements[remove.x, remove.y]);
                elements[remove.x, remove.y] = null;
            }
        }
    }

    private void ReFillElement()
    {
        Debug.Log("ReFill Element");
        var reFillIndex = new HashSet<(int x, int y)>();

        while(removeIndex.Count > 0)
        {
            // ������ ���������� remove index�غ���.
            foreach (var remove in removeIndex)
            {
                // ���� Instaniate�� �ؾ߰ڴ�!
                var randIndex = Random.Range(0, prefab.Length);
                var newElement = Instantiate(prefab[randIndex]);
                newElement.transform.SetParent(board.transform, false);

                // ���� ������ newElement�� �׻� [remove.x, 0]�� ��ġ�Ѵ�.
                var rectTransform = newElement.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(startX + remove.x * cellSize, startY - 0 * cellSize);

                // �̷������� �����ؾ߰ڳ�? �̷��� �� �����ؼ� ������ ���ܼ� remove.y�� �ش��ϴ� �ֵ��� ���� �Űܾ���.
                for(int i = remove.y; i > 0; i--)
                {
                    // ��ġ �̵��� ���Ѿ� �Ѵ�.
                    elements[remove.x, i] = elements[remove.x, i - 1];
                    colors[remove.x, i] = colors[remove.x, i - 1];
                    if(elements[remove.x, i - 1] != null)
                    {
                        var objectRectTransform = elements[remove.x, i - 1].GetComponent<RectTransform>();
                        objectRectTransform.anchoredPosition = new Vector2(startX + remove.x * cellSize, startY - i * cellSize);
                    }
                }

                // ���� �ٽ� �����������
                elements[remove.x, 0] = newElement;
                colors[remove.x, 0] = randIndex;
            
                // null�� ���� �ٽ� �˻縦 �ؾ���!
                if (elements[remove.x, remove.y] == null)
                    reFillIndex.Add(remove);
            }

            removeIndex.Clear();

            foreach(var refill in reFillIndex)
            {
                removeIndex.Add(refill);
            }
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
