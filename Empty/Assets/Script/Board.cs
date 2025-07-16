using System.Collections;
using System.Collections.Generic;
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

    // 중앙에 위치해야 할 object다.
    private float startX;
    private float startY;

    public GameObject[] prefab;
    public GameObject board;

    // Board Object다.
    private int[,] colors;
    private GameObject[,] elements;

    // DFS에 필요한 Index 저장할 변수
    private List<(int x, int y)> tourIndex;
    private List<(int x, int y)> saveIndex;
    private HashSet<(int x, int y)> removeIndex;

    // swap에 필요할 object
    private GameObject swapObject;

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
                
                var objectElement = newElement.GetComponent<Element>();
                objectElement.SetPosition(x, y);
            }
        }

        CheckBoard();
    }

    // Swap을 구현할 차례다.
    private void SelectElement(GameObject _selectObject)
    {
        // 어떤 object를 선택했어.
        if (swapObject == null)
        {
            swapObject = _selectObject;
            Debug.Log($"{swapObject} 선택했다.");
        }
        else
        {
            if (IsExistRangeElement(_selectObject))
                StartCoroutine(SwapElement(_selectObject));
            else
                swapObject = null;
        }
    }

    private bool IsExistRangeElement(GameObject targetObject)
    {
        // 먼저 swap전에 확인할 게 있다.
        // 해당 image를 클릭해서 해당 요소의 index x, y 값을 알아야 해.
        // 요소를 만들자!
        // 그 다음 previous.xy - current.xy의 절대 값이 1이여야 swap이 가능하다. 아니면 swapObject = null
        // 위 조건을 만족했으면 그 때 Swap을 한다.
        var firstObjectIndex = swapObject.GetComponent<Element>().GetPosition();
        var secondObjectIndex = targetObject.GetComponent<Element>().GetPosition();

        return Mathf.Abs(firstObjectIndex.x - secondObjectIndex.x) + 
            Mathf.Abs(firstObjectIndex.y - secondObjectIndex.y) == 1 ? true : false;
    }

    private IEnumerator SwapElement(GameObject changeObject)
    {
        // 일단 Rect Transform을 받아와.
        var firstObjectRectTransform = swapObject.GetComponent<RectTransform>();
        var secondObjectRectTransform = changeObject.GetComponent<RectTransform>();

        // 초기 Swap할 Vector 위치
        var firstObjectVector = new Vector2(firstObjectRectTransform.anchoredPosition.x, firstObjectRectTransform.anchoredPosition.y);
        var secondObjectVector = new Vector2(secondObjectRectTransform.anchoredPosition.x, secondObjectRectTransform.anchoredPosition.y);

        // 각자 object가 정해진 start와 end에 맞춰서 이동된다.
        var moveList = new List<(Vector2 start, Vector2 end, RectTransform target)>();
        moveList.Add((firstObjectVector, secondObjectVector, firstObjectRectTransform));
        moveList.Add((secondObjectVector, firstObjectVector, secondObjectRectTransform));

        yield return StartCoroutine(AnimateMovement(moveList));

        // elements 값도 바꿔야한다.
        var tempElement = elements[(int)firstObjectVector.x, (int)firstObjectVector.y];
        elements[(int)firstObjectVector.x, (int)firstObjectVector.y] = elements[(int)secondObjectVector.x, (int)secondObjectVector.y];
        elements[(int)secondObjectVector.x, (int)secondObjectVector.y] = tempElement;

        // Color도 바꿔야해.
        var tempColor = colors[(int)firstObjectVector.x, (int)firstObjectVector.y];
        colors[(int)firstObjectVector.x, (int)firstObjectVector.y] = colors[(int)secondObjectVector.x, (int)secondObjectVector.y];
        colors[(int)secondObjectVector.x, (int)secondObjectVector.y] = tempColor;

        // Element Index도 교체해야해.
        var swapIndex = swapObject.GetComponent<Element>();
        var changeIndex = changeObject.GetComponent<Element>();

        var tempIndex = swapIndex.GetPosition();
        swapIndex.SetPosition(changeIndex.GetPosition());
        changeIndex.SetPosition(tempIndex);

        // 그 다음에 Object를 바꿔주면 끝인가?
        var temp = swapObject;
        swapObject = changeObject;
        changeObject = temp;

        // DFS로 가서 확인해야 하는데 확인을 먼저 하자.
    }

    #region Refill
    private IEnumerator AnimationReFill()
    {
        // 여기에 blank count랑 max Depth가 들어가 있다.
        var blankList = BlankCheckBoard(removeIndex);

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
                        
                        var moveIndex = moveObject.GetComponent<Element>();
                        moveIndex.SetPosition(x, y);
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

        foreach (var move in moveList)
        {
            move.target.anchoredPosition = move.end;
        }
    }

    private (int blankCount, int maxDepth) ResearchMaxDepth((int blankCount, int maxDepth) _value, int index)
    {
        _value.blankCount--;

        if (_value.blankCount <= 0)
        {
            return _value;
        }

        for (int y = _value.maxDepth; y > 0; y--)
        {
            if (elements[index, y] == null)
            {
                _value.maxDepth = y;
                return _value;
            }
        }

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

        for(int i = 0; i < width; i++)
        {
            blankList.Add((0, 0));
        }
        
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
    #endregion

    #region Destory 
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
    #endregion

    #region CheckBoard
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
    #endregion
}
