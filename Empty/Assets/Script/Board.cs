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

    // �߾ӿ� ��ġ�ؾ� �� object��.
    private float startX;
    private float startY;

    public GameObject[] prefab;
    public GameObject board;

    // Board Object��.
    private int[,] colors;
    private GameObject[,] elements;

    // DFS�� �ʿ��� Index ������ ����
    private List<(int x, int y)> tourIndex;
    private List<(int x, int y)> saveIndex;
    private HashSet<(int x, int y)> removeIndex;

    // swap�� �ʿ��� object
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
                
                var objectElement = newElement.GetComponent<Element>();
                objectElement.SetPosition(x, y);
            }
        }

        CheckBoard();
    }

    // Swap�� ������ ���ʴ�.
    private void SelectElement(GameObject _selectObject)
    {
        // � object�� �����߾�.
        if (swapObject == null)
        {
            swapObject = _selectObject;
            Debug.Log($"{swapObject} �����ߴ�.");
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
        // ���� swap���� Ȯ���� �� �ִ�.
        // �ش� image�� Ŭ���ؼ� �ش� ����� index x, y ���� �˾ƾ� ��.
        // ��Ҹ� ������!
        // �� ���� previous.xy - current.xy�� ���� ���� 1�̿��� swap�� �����ϴ�. �ƴϸ� swapObject = null
        // �� ������ ���������� �� �� Swap�� �Ѵ�.
        var firstObjectIndex = swapObject.GetComponent<Element>().GetPosition();
        var secondObjectIndex = targetObject.GetComponent<Element>().GetPosition();

        return Mathf.Abs(firstObjectIndex.x - secondObjectIndex.x) + 
            Mathf.Abs(firstObjectIndex.y - secondObjectIndex.y) == 1 ? true : false;
    }

    private IEnumerator SwapElement(GameObject changeObject)
    {
        // �ϴ� Rect Transform�� �޾ƿ�.
        var firstObjectRectTransform = swapObject.GetComponent<RectTransform>();
        var secondObjectRectTransform = changeObject.GetComponent<RectTransform>();

        // �ʱ� Swap�� Vector ��ġ
        var firstObjectVector = new Vector2(firstObjectRectTransform.anchoredPosition.x, firstObjectRectTransform.anchoredPosition.y);
        var secondObjectVector = new Vector2(secondObjectRectTransform.anchoredPosition.x, secondObjectRectTransform.anchoredPosition.y);

        // ���� object�� ������ start�� end�� ���缭 �̵��ȴ�.
        var moveList = new List<(Vector2 start, Vector2 end, RectTransform target)>();
        moveList.Add((firstObjectVector, secondObjectVector, firstObjectRectTransform));
        moveList.Add((secondObjectVector, firstObjectVector, secondObjectRectTransform));

        yield return StartCoroutine(AnimateMovement(moveList));

        // elements ���� �ٲ���Ѵ�.
        var tempElement = elements[(int)firstObjectVector.x, (int)firstObjectVector.y];
        elements[(int)firstObjectVector.x, (int)firstObjectVector.y] = elements[(int)secondObjectVector.x, (int)secondObjectVector.y];
        elements[(int)secondObjectVector.x, (int)secondObjectVector.y] = tempElement;

        // Color�� �ٲ����.
        var tempColor = colors[(int)firstObjectVector.x, (int)firstObjectVector.y];
        colors[(int)firstObjectVector.x, (int)firstObjectVector.y] = colors[(int)secondObjectVector.x, (int)secondObjectVector.y];
        colors[(int)secondObjectVector.x, (int)secondObjectVector.y] = tempColor;

        // Element Index�� ��ü�ؾ���.
        var swapIndex = swapObject.GetComponent<Element>();
        var changeIndex = changeObject.GetComponent<Element>();

        var tempIndex = swapIndex.GetPosition();
        swapIndex.SetPosition(changeIndex.GetPosition());
        changeIndex.SetPosition(tempIndex);

        // �� ������ Object�� �ٲ��ָ� ���ΰ�?
        var temp = swapObject;
        swapObject = changeObject;
        changeObject = temp;

        // DFS�� ���� Ȯ���ؾ� �ϴµ� Ȯ���� ���� ����.
    }

    #region Refill
    private IEnumerator AnimationReFill()
    {
        // ���⿡ blank count�� max Depth�� �� �ִ�.
        var blankList = BlankCheckBoard(removeIndex);

        // �� List ����ϴ� �� index�� x�κ��̿��� 0���� ��ȸ�ؾ� �ϳ�? 
        // ����ü�� �ٲٸ� �޸𸮸� �� ������? �� ���߿� ����ؾ��� �κ��̰� ������ index�� ����.
        bool isRunning = CheckBlank(blankList);

        while (!isRunning)
        {
            var tourDict = new Dictionary<RectTransform, (Vector2 start, Vector2 end)>();

            for (int x = 0; x < blankList.Count; x++)
            {
                var value = blankList[x];

                if (value.blankCount <= 0)
                    continue;

                // ���� Instaniate�� �ؾ߰ڴ�!
                var randIndex = Random.Range(0, prefab.Length);
                var newElement = Instantiate(prefab[randIndex]);
                newElement.transform.SetParent(board.transform, false);

                // ���� ������ newElement�� �׻� [remove.x, 0]�� ��ġ�Ѵ�.
                var rectTransform = newElement.GetComponent<RectTransform>();
                var upperStartPos = new Vector2(startX + x * cellSize, startY - (-1) * cellSize);
                var upperEndPos = new Vector2(startX + x * cellSize, startY - 0 * cellSize);

                rectTransform.anchoredPosition = upperStartPos;
                tourDict[rectTransform] = (upperStartPos, upperEndPos);

                // �̷� ������ �����ؾ߰ڳ�? �̷��� �� �����ؼ� ������ ���ܼ� remove.y�� �ش��ϴ� �ֵ��� ���� �Űܾ���.
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
        float duration = 0.2f; // �̵� �ð� (���� ����)

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
    #endregion
}
