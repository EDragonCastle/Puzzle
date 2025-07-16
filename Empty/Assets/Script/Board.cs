using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private bool isProcessing;

    private float startX;
    private float startY;

    public GameObject[] prefab;
    public GameObject board;

    private int[,] colors;
    private GameObject[,] elements;

    // DFS에 필요한 Index 저장할 변수
    List<(int x, int y)> tourIndex;
    List<(int x, int y)> saveIndex;
    HashSet<(int x, int y)> removeIndex;


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
            Initalize();
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
    }

    private IEnumerator DestoryElement()
    {
        isProcessing = true;

        Debug.Log("Destory Element");
        // Destory
        foreach (var remove in removeIndex)
        {
            if (elements[remove.x, remove.y] != null)
            {
                Destroy(elements[remove.x, remove.y]);
                elements[remove.x, remove.y] = null;
            }
        }

        yield return new WaitForSeconds(0.3f);
        StartCoroutine(AnimationReFill());
    }

    private IEnumerator AnimationReFill()
    {
        // 여기에 blank count랑 max Depth가 들어가 있다.
        var blankList = BlankCheckBoard(removeIndex);

        // Animation 처럼 흘러 가야 한다.

        // while문을 탈출하는 방법은 무엇일까?
        // blankList에서 모든 blankCount에서 0이 되어야 한다.

        // 아 List 사용하는 데 index가 x부분이여서 0부터 순회해야 하네? 
        // 구조체로 바꾸면 메모리를 더 먹을까? 는 나중에 고려해야할 부분이고 지금은 index로 하자.
        bool isRunning = CheckBlank(blankList);

        while (!isRunning)
        {
            var tourDict = new Dictionary<RectTransform, (Vector2 start, Vector2 end)>();

            for (int x = 0; x < blankList.Count; x++)
            {
                var value = blankList[x];

                if (value.blankCount <= 0)
                    continue;

                // 먼저 Instaniate를 해야겠다!
                var randIndex = Random.Range(0, prefab.Length);
                var newElement = Instantiate(prefab[randIndex]);
                newElement.transform.SetParent(board.transform, false);

                // 새로 생성한 newElement은 항상 [remove.x, 0]에 위치한다.
                var rectTransform = newElement.GetComponent<RectTransform>();
                var upperStartPos = new Vector2(startX + x * cellSize, startY - (-1) * cellSize);
                var upperEndPos = new Vector2(startX + x * cellSize, startY - 0 * cellSize);

                rectTransform.anchoredPosition = upperStartPos;
                tourDict[rectTransform] = (upperStartPos, upperEndPos);

                // 이런 식으로 진행해야겠네? 이러면 한 번만해서 공백이 생겨서 remove.y에 해당하는 애들을 전부 옮겨야해.
                for (int y = value.maxDepth; y > 0; y--)
                {
                    if(elements[x, y - 1] != null)
                    {
                        var moveObject = elements[x, y - 1];
                        var objectRectTransform = moveObject.GetComponent<RectTransform>();

                        Vector2 startPos = objectRectTransform.anchoredPosition;
                        Vector2 endPos = new Vector2(startX + x * cellSize, startY - y * cellSize);

                        tourDict[objectRectTransform] = (startPos, endPos);

                    }
                    elements[x, y] = elements[x, y - 1];
                    colors[x, y] = colors[x, y - 1];
                }

                elements[x, 0] = newElement;
                colors[x, 0] = randIndex;

                if (elements[x, value.maxDepth] != null)
                    blankList[x] = ResearchMaxDepth(value, x);
            }

            var finalList = new List<(Vector2 start, Vector2 end, RectTransform target)>();
            foreach(var entry in tourDict)
            {
                finalList.Add((entry.Value.start, entry.Value.end, entry.Key));
            }

            if (finalList.Count > 0)
            {
                yield return StartCoroutine(AnimateMovement(finalList));
            }

            isRunning = CheckBlank(blankList);
        }

        removeIndex.Clear();
        CheckBoard();
        isProcessing = false;
    }

    // 왜 참조가 되는거지?
    private IEnumerator AnimateMovement(List<(Vector2 start, Vector2 end, RectTransform target)> moveList)
    {
        Debug.Log("Animation Movement");
        float elapsed = 0f;
        float duration = 0.2f; // 이동 시간 (조절 가능)

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            foreach (var move in moveList)
            {
                move.target.anchoredPosition = Vector2.Lerp(move.start, move.end, t);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 마지막에 정확히 도착 지점으로 고정
        foreach (var move in moveList)
        {
            move.target.anchoredPosition = move.end;
        }
    }

    private (int blankCount, int maxDepth) ResearchMaxDepth((int blankCount, int maxDepth) _value, int index)
    {
        // 잘 작동이 되지 않는 이유는 일단 maxDepth 부분을 초기화 해야 한다는 점이다.
        // 어떻게 초기화 해야할 지 생각해야겠다. 
        // Max Depth 부분이 채워졌어. 그러면 count를 확인해서 blankCount가 1이면 끝.
        // 근데 2이상이다? 그러면 element에서 위로 돌면서 확인해야 하고 maxDepth를 확인하면 되겠다.
        // 또한 blankCount--를 해주자.
        // 그리고 검사할 때는 max -> init으로 확인하자.
        // 장소를 옮기고 다시 하자.

        _value.blankCount--;

        if (_value.blankCount <= 0)
        {
            return _value;
        }

        // 이 부분에서 문제가 생긴듯 하다.
        for (int y = _value.maxDepth; y > 0; y--)
        {
            if (elements[index, y] == null)
            {
                _value.maxDepth = y;
                return _value;
            }
        }

        // 위으 조건문을 통과 했다는 의미가 위 list에 null이 없다는 거니까 다 찼다는 뜻이다.
        _value.blankCount = 0;
        return _value;
    }

    private bool CheckBlank(List<(int blankCount, int maxDepth)> _blankList)
    {
        var isEmptyBlank = true;
        foreach(var value in _blankList)
        {
            if (value.blankCount > 0)
            {
                isEmptyBlank = false;
                return isEmptyBlank;
            }
        }
        return isEmptyBlank;
    }

    private List<(int blankCount, int maxDepth)> BlankCheckBoard(HashSet<(int x, int y)> _removeList)
    {
        var blankList = new List<(int blankCount, int maxDepth)>();

        // Initailze 흠.. 이 부분도 마음에 안 들긴해.
        for(int i = 0; i < width; i++)
        {
            blankList.Add((0, 0));
        }

        // 비어 있는 곳의 x를 확인해서 빈 공간을 Check 한다.
        // 최대 깊이도 확인하면 더 좋을 것 같다.
        foreach(var remove in _removeList)
        {
            var listValue = blankList[remove.x];
            listValue.blankCount++;

            if (listValue.maxDepth < remove.y)
                listValue.maxDepth = remove.y;

            blankList[remove.x] = listValue;
        }

        return blankList;
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

        if (removeIndex.Count > 0)
            StartCoroutine(DestoryElement());
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
