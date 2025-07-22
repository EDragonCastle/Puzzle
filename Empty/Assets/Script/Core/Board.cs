using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    private float duration = 0.2f;

    // �߾ӿ� ��ġ�ؾ� �� object��.
    private float startX;
    private float startY;

    public GameObject[] prefab;
    public GameObject board;

    // Board Object��.
    private IUIElement[,] uiElements;

    // DFS�� �ʿ��� Index ������ ����
    private List<(int x, int y)> tourIndex;
    private List<(int x, int y)> saveIndex;
    private HashSet<(int x, int y)> removeIndex;

    // return�� Score�� ������ ���� �ʿ��� event��.
    public static event Action<int> scoreValue;

    private int swapCount = 0;

    // swap�� �ʿ��� object
    private IUIElement swapObject;

    // Factory
    private Factory objectFactory;

    private void Start()
    {
        Initalize();
    }

    private void OnEnable()
    {
        Element.onClickElement += SelectElement;
    }

    private void OnDisable()
    {
        Element.onClickElement -= SelectElement;
    }

    private void Initalize()
    {
        startX = -((width * 0.5f - 0.5f) * cellSize);
        startY = ((height * 0.5f - 0.5f) * cellSize);

        uiElements = new IUIElement[width, height];
        tourIndex = new List<(int x, int y)>();
        saveIndex = new List<(int x, int y)>();
        removeIndex = new HashSet<(int x, int y)>();

        objectFactory = Locator.GetFactory();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var randIndex = UnityEngine.Random.Range(0, prefab.Length);
                var position = new Vector2(startX + x * cellSize, startY - y * cellSize);

                var newElement = objectFactory.CreateUIObject((ElementColor)randIndex, position, Quaternion.identity, Vector3.one, board.transform);
                var info = newElement.GetElementInfo();
                info.position = new Vector2(x, y);
                newElement.SetElementInfo(info);

                uiElements[x, y] = newElement;
            }
        }

        InitCheckBoard();
    }

    #region Swap
    // Swap�� ������ ���ʴ�.
    private void SelectElement(IUIElement _selectObject)
    {
        if (isProcessing)
            return;

        // � object�� �����߾�.
        if (swapObject == null)
        {
            swapObject = _selectObject;
        }
        else
        {
            if (IsExistRangeElement(_selectObject))
                StartCoroutine(SwapElement(swapObject, _selectObject, false));
            else
                swapObject = null;
        }
    }

    private bool IsExistRangeElement(IUIElement targetObject)
    {
        var firstObjectIndex = swapObject.GetElementInfo().position;
        var secondObjectIndex = targetObject.GetElementInfo().position;

        return Mathf.Abs(firstObjectIndex.x - secondObjectIndex.x) +
            Mathf.Abs(firstObjectIndex.y - secondObjectIndex.y) == 1 ? true : false;
    }

    private IEnumerator SwapElement(IUIElement _swapObject, IUIElement changeObject, bool isReturn)
    {
        isProcessing = true;

        var originObjectElementInfo = _swapObject.GetElementInfo();
        var changeObjectElementInfo = changeObject.GetElementInfo();

        var firstelementUIPos = _swapObject.GetRectTransform();
        var secondelementUIPos = changeObject.GetRectTransform();

        // �ʱ� Swap�� Vector ��ġ
        Vector2 firstObjectVector = new Vector2(firstelementUIPos.anchoredPosition.x, firstelementUIPos.anchoredPosition.y);
        Vector2 secondObjectVector = new Vector2(secondelementUIPos.anchoredPosition.x, secondelementUIPos.anchoredPosition.y);

        // Animation Refill�� �ʿ��� parameter�� ����
        var moveList = new List<(Vector2 start, Vector2 end, RectTransform target)>();
        moveList.Add((firstObjectVector, secondObjectVector, firstelementUIPos));
        moveList.Add((secondObjectVector, firstObjectVector, secondelementUIPos));

        yield return StartCoroutine(AnimateMovement(moveList));

        // Position�� �����´�.
        Vector2Int firstIndex = new Vector2Int((int)originObjectElementInfo.position.x, (int)originObjectElementInfo.position.y);
        Vector2Int secondIndex = new Vector2Int((int)changeObjectElementInfo.position.x, (int)changeObjectElementInfo.position.y);

        // index ���� �ٲ��ش�.
        var tempIndex = originObjectElementInfo.position;
        originObjectElementInfo.position = changeObjectElementInfo.position;
        changeObjectElementInfo.position = tempIndex;

        // ������ ��ģ��.
        _swapObject.SetElementInfo(originObjectElementInfo);
        changeObject.SetElementInfo(changeObjectElementInfo);

        // elements ���� �ٲ���Ѵ�.
        var tempElement = uiElements[firstIndex.x, firstIndex.y];
        uiElements[firstIndex.x, firstIndex.y] = uiElements[secondIndex.x, secondIndex.y];
        uiElements[secondIndex.x, secondIndex.y] = tempElement;
        
        // �� origine�� �ƴϰ� change�ĸ� SetElemeint�� Ȯ���߱� �����̴�.
        // DFS�� ���� Ȯ���ؾ� �ϴµ� �� ���� object�� Ȯ���ϸ� �ȴ�.
        if (!isReturn)
        {
            SwapDFS(firstIndex.x, firstIndex.y, changeObjectElementInfo.color, Direction.None, 1);
            if (saveIndex.Count >= 3)
            {
                foreach (var save in saveIndex)
                {
                    removeIndex.Add(save);
                }
            }
            tourIndex.Clear();

            SwapDFS(secondIndex.x, secondIndex.y, originObjectElementInfo.color, Direction.None, 1);
            if (saveIndex.Count >= 3)
            {
                foreach (var save in saveIndex)
                {
                    removeIndex.Add(save);
                }
            }
            tourIndex.Clear();
            if (removeIndex.Count > 0)
            {
                StartCoroutine(DestoryElement());
            }
            else
            {
                yield return StartCoroutine(SwapElement(changeObject, _swapObject, true));
                isProcessing = false;
                FailSwap();
            }
            swapObject = null;
        }
    }

    private void FailSwap()
    {
        swapCount++;
        if(swapCount >= 10)
        {
            Debug.Log("game over");

            // ���� UI ����
            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    if(uiElements[x, y] != null)
                    {
                        Destroy(uiElements[x, y].GetGameObject());
                        uiElements[x, y] = null;
                    }
                }
            }
            swapCount = 0;

            // Resource���� GameOver Prefab ȣ���ϰ� ����� �ؾ� �Ѵ�.
            Debug.Log("game start");
            Initalize();
        }
    }

    // �������� Ȯ���ؾ� �Ѵ�.
    private void SwapDFS(int x, int y, int _color, Direction _direction, int count)
    {
        // ���� �Ѿ�� ���ư���.
        if (x < 0 || width <= x || y < 0 || height <= y)
        {
            return;
        }

        var elementInfo = uiElements[x, y].GetElementInfo();
       
        // �湮�ߴ� �� Ȯ�� �� ������ �ٸ��� Ȯ���Ѵ�.
        if (elementInfo.isVisits || elementInfo.color != _color)
        {
            return;
        }

        count++;
        elementInfo.isVisits = true;
        uiElements[x, y].SetElementInfo(elementInfo);

        tourIndex.Add((x, y));

        if (tourIndex.Count >= 3)
        {
            saveIndex = tourIndex;
        }

        switch (_direction)
        {
            case Direction.Horizontal:
                SwapDFS(x, y + 1, _color, Direction.Horizontal, count);
                SwapDFS(x, y - 1, _color, Direction.Horizontal, count);
                break;
            case Direction.Vertical:
                SwapDFS(x + 1, y, _color, Direction.Vertical, count);
                SwapDFS(x - 1, y, _color, Direction.Vertical, count);
                break;
            case Direction.None:
                SwapDFS(x + 1, y, _color, Direction.Vertical, count);
                SwapDFS(x - 1, y, _color, Direction.Vertical, count);
                if (tourIndex.Count >= 3)
                {
                    saveIndex = tourIndex;
                    foreach (var save in saveIndex)
                    {
                        removeIndex.Add(save);
                    }
                }
                tourIndex.Clear();
                tourIndex.Add((x, y));
                SwapDFS(x, y + 1, _color, Direction.Horizontal, count);
                SwapDFS(x, y - 1, _color, Direction.Horizontal, count);
                break;
        }
        elementInfo.isVisits = false;
        uiElements[x, y].SetElementInfo(elementInfo);
    }
    #endregion

    #region Refill
    private IEnumerator AnimationReFill()
    {
        var blankList = BlankCheckBoard(removeIndex);
        bool isRunning = CheckBlank(blankList);

        while (!isRunning)
        {
            var tourDict = new Dictionary<RectTransform, (Vector2 start, Vector2 end)>();

            for (int x = 0; x < blankList.Count; x++)
            {
                var value = blankList[x];

                if (value.blankCount <= 0)
                    continue;

                // StartPos, EndPos Setting
                var randIndex = UnityEngine.Random.Range(0, prefab.Length);
                Vector2 upperStartPos = new Vector2(startX + x * cellSize, startY - (-1) * cellSize);
                Vector2 upperEndPos = new Vector2(startX + x * cellSize, startY - 0 * cellSize);

                var newElement = objectFactory.CreateUIObject((ElementColor)randIndex, upperStartPos, Quaternion.identity, Vector3.one, board.transform);
                var newElementUIPos = newElement.GetRectTransform();
                tourDict[newElementUIPos] = (upperStartPos, upperEndPos);

                var newElementInfo = newElement.GetElementInfo();
                newElementInfo.position = new Vector2(x, 0);
                newElement.SetElementInfo(newElementInfo);

                // �̷� ������ �����ؾ߰ڳ�? �̷��� �� �����ؼ� ������ ���ܼ� remove.y�� �ش��ϴ� �ֵ��� ���� �Űܾ���.
                for (int y = value.maxDepth; y > 0; y--)
                {
                    if (uiElements[x, y - 1] != null)
                    {
                        var moveObject = uiElements[x, y - 1];

                        var elementInfo = moveObject.GetElementInfo();
                        var elementUIPos = moveObject.GetRectTransform();

                        elementInfo.position = new Vector2(x, y);
                        moveObject.SetElementInfo(elementInfo);

                        Vector2 startPos = elementUIPos.anchoredPosition;
                        Vector2 endPos = new Vector2(startX + x * cellSize, startY - y * cellSize);

                        tourDict[elementUIPos] = (startPos, endPos);
                    }
                    
                    uiElements[x, y] = uiElements[x, y - 1];
                }

                uiElements[x, 0] = newElement;

                if (uiElements[x, value.maxDepth] != null)
                    blankList[x] = ResearchMaxDepth(value, x);
            }

            var finalList = new List<(Vector2 start, Vector2 end, RectTransform target)>();
            foreach (var entry in tourDict)
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
            if (uiElements[index, y] == null)
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
        foreach (var value in _blankList)
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

        for (int i = 0; i < width; i++)
        {
            blankList.Add((0, 0));
        }

        foreach (var remove in _removeList)
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
        scoreValue?.Invoke(removeIndex.Count);
        
        // Destory
        foreach (var remove in removeIndex)
        {
            if (uiElements[remove.x, remove.y] != null)
            {
                Destroy(uiElements[remove.x, remove.y].GetGameObject());
                uiElements[remove.x, remove.y] = null;
            }
        }

        yield return new WaitForSeconds(duration);
        StartCoroutine(AnimationReFill());
    }
    #endregion

    #region CheckBoard
    private void CheckBoard()
    {
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                var color = uiElements[i, j].GetElementInfo().color;
                DFS(i, j, color, Direction.None, 1);
            }
        }

        if (removeIndex.Count > 0)
            StartCoroutine(DestoryElement());
    }
    #endregion

    #region Initalize Setting Board
    private void InitCheckBoard()
    {
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                var color = uiElements[i, j].GetElementInfo().color;
                DFS(i, j, color, Direction.None, 1);
            }
        }

        // Coroutine�� �ƴ� �����Լ��� �ٷ� ������.
        if (removeIndex.Count > 0)
            InitDestroyElement();   
    }

    private void InitDestroyElement()
    {
        foreach (var remove in removeIndex)
        {
            if (uiElements[remove.x, remove.y] != null)
            {
                objectFactory.DestoryUIObject(uiElements[remove.x, remove.y].GetGameObject());
                uiElements[remove.x, remove.y] = null;
            }
        }

        Refill();
    }

    private void Refill()
    {
        var blankList = BlankCheckBoard(removeIndex);
        bool isRunning = CheckBlank(blankList);

        while (!isRunning)
        {
            for (int x = 0; x < blankList.Count; x++)
            {
                var value = blankList[x];

                if (value.blankCount <= 0)
                    continue;
                
                var randIndex = UnityEngine.Random.Range(0, prefab.Length);
                Vector2 position = new Vector2(startX + x * cellSize, startY - 0 * cellSize);
                var newElement = objectFactory.CreateUIObject((ElementColor)randIndex, position, Quaternion.identity, Vector3.one, board.transform);

                var newElementInfo = newElement.GetElementInfo();
                newElementInfo.position = new Vector2(x, 0);
                newElement.SetElementInfo(newElementInfo);

                for (int y = value.maxDepth; y > 0; y--)
                {
                    if (uiElements[x, y - 1] != null)
                    {
                        var moveObject = uiElements[x, y - 1];

                        var elementInfo = moveObject.GetElementInfo();
                        var elementUIPos = moveObject.GetRectTransform();

                        elementInfo.position = new Vector2(x, y);
                        moveObject.SetElementInfo(elementInfo);

                        elementUIPos.anchoredPosition = new Vector2(startX + x * cellSize, startY - y * cellSize);
                    }

                    uiElements[x, y] = uiElements[x, y - 1];
                }

                uiElements[x, 0] = newElement;

                if (uiElements[x, value.maxDepth] != null)
                    blankList[x] = ResearchMaxDepth(value, x);
            }

            isRunning = CheckBlank(blankList);
        }

        removeIndex.Clear();
        InitCheckBoard();
        isProcessing = false;
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

        int currentColor = uiElements[x, y].GetElementInfo().color;
        if (currentColor != _color)
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

        count++;

        tourIndex.Add((x, y));

        if (tourIndex.Count >= 3)
        {
            saveIndex = tourIndex;
        }

        switch (_direction)
        {
            case Direction.Horizontal:
                DFS(x, y + 1, _color, Direction.Horizontal, count);
                break;
            case Direction.Vertical:
                DFS(x + 1, y, _color, Direction.Vertical, count);
                break;
            case Direction.None:
                DFS(x + 1, y, _color, Direction.Vertical, count);

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
