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

    // DFS에 필요한 Index 저장할 변수
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
            // 이건 left -> right로 생성하고 있고,
            for (int x = 0; x < width; x++)
            {
                // prefab 개수에서 random 값 출력 및 object 생성과 부모 설정
                var randIndex = Random.Range(0, prefab.Length);
                var newElement = Instantiate(prefab[randIndex]);
                newElement.transform.SetParent(board.transform, false);

                // Object에서 Rect Transform를 받아오고, 위치 조절
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
        // 1. 범위에 넘어갔다면 return
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

        // 3. 색이 다르다면 return 
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

        // 위에 조건들을 만족하지 않고 통과했다면 
        // 색이 같은 것이다.
        // count++를 늘려야겠다. 그리고 Vertical인지 Horizontal인지 확인해야 할 것 같은데?
        count++;

        tourIndex.Add((x, y));
        // count가 3이상이면 깨질 블럭이다. 지금 깨지는 건 아닌데 어떻게 해야할까?

        // 문제점이 생겼다. 맨 처음 None일 때 vertical, Horizontal 둘다 검사해서 vertical에서 count 3개 이상이여서 저장이 됐지만, horizontal에서 3개 이상이면 전에 있던 data가 사라질 수 있다.
        if (tourIndex.Count >= 3)
        {
            // ref type이라 tourIndex가 Clear 되면 같이 사라진다.
            saveIndex = tourIndex;
        }

        // direction을 만든 이유?
        // vertical이면 vertical로 가고, horizontal이면 horizontal로 조사해야 하기 때문이다.
        // 근데 맨 처음에는 None으로 들어갈텐데? None일 때는 양쪽 다 들어가봐야 하는데?

        // 그러면 검사도 오른쪽과 아래만 검사해도 될 것 같은데 굳이 4방향을 검사해야 할 필요를 못 느끼겠다.
        // 왜냐하면 맨 처음 검사할 때만 사용하는 것이기 때문이다. 나중에 Swap 할 때는 상하좌우를 전부 확인해야 할 것 같지만
        // 지금은 필요없는 듯하다.
        switch (_direction)
        {
            case Direction.Horizontal:
                // DFS Function ... Horizontal
                //DFS() 에서 Y+1을 해주자.
                DFS(x, y + 1, _color, Direction.Horizontal, count);
                break;
            case Direction.Vertical:
                // DFS Function ... Vertical
                // DFS() 에서 x+1을 해주자.
                DFS(x + 1, y, _color, Direction.Vertical, count);
                break;
            case Direction.None:
                // Horizontal, Vertical 각각 이건 for문으로 한번에 검사하려 했는데 그냥 하나씩 넣는게 더 괜찮아 보인다.
                // DFS() X+1, Y+1을 해주자.
                DFS(x + 1, y, _color, Direction.Vertical, count);

                // 나갈 때 이미 사라졌기 때문에 추가해주는 것이다.
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
