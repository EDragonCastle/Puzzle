using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;

/// <summary>
/// Puzzle�� ���� ���� ���� Direction ��ġ
/// </summary>
public enum Direction
{
    Vertical,
    Horizontal,
    None,
}

/// <summary>
/// ������ �ٽ��� ��� �ִ� Board Class
/// </summary>
public class Board : MonoBehaviour, IChannel
{
    [SerializeField]
    private int width = 6;

    [SerializeField]
    private int height = 8;

    // ������ ����ϰ� �ִ�.
    private readonly int cellSize = 110;

    private bool isProcessing;

    // Animation Move�� �� �ɸ��� �ð�
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

    private int swapCount = 0;

    // swap�� �ʿ��� object
    private IUIElement swapObject;

    // Manager
    private Factory objectFactory;
    private EventManager eventManager;
    private UIManager uiManager;

    // Life Setting
    [SerializeField]
    private GameObject lifeParent;

    private Life life;
    private GameObject[] lifes;
    private int maxLife;

    private void Awake()
    {
        objectFactory = Locator<Factory>.Get();
        eventManager = Locator<EventManager>.Get();
        uiManager = Locator<UIManager>.Get();
        Initalize();
    }

    // �̺�Ʈ ��� �� Board �ʱ�ȭ
    private void OnEnable()
    {
        Enable();
        eventManager.Subscription(ChannelInfo.Select, HandleEvent);
    }

    // �̺�Ʈ ���� �� Board ����
    private void OnDisable()
    {
        DisEnable();
        eventManager.Unsubscription(ChannelInfo.Select, HandleEvent);
    }

    /// <summary>
    /// Event Manager�� ����� IChannel Interface
    /// </summary>
    /// <param name="channel">ä�� ����</param>
    /// <param name="information">����� Object ����</param>
    public void HandleEvent(ChannelInfo channel, object _information)
    {
        switch(channel)
        {
            case ChannelInfo.Select:
                GameObject gameObject = _information as GameObject;
                if(gameObject != null)
                {
                    var objectIUIElement = gameObject.GetComponent<IUIElement>();
                    SelectElement(objectIUIElement);
                }
            break;
        }
    }

    /// <summary>
    /// Board �ʱ� ����
    /// </summary>
    private void Initalize()
    {
        // ���� x, y ��ġ ����
        startX = -((width * 0.5f - 0.5f) * cellSize);
        startY = ((height * 0.5f - 0.5f) * cellSize);

        // ���� �ʱ�ȭ
        uiElements = new IUIElement[width, height];
        tourIndex = new List<(int x, int y)>();
        saveIndex = new List<(int x, int y)>();
        removeIndex = new HashSet<(int x, int y)>();

        // Life ����
        maxLife = MaxLifeSetting(Level.Easy);
        lifes = new GameObject[maxLife];
        var lifePrefab = uiManager.GetUIPrefab(UIPrefab.Life);
        
        for(int i = 0; i < maxLife; i++)
        {
            var lifeObject = GameObject.Instantiate(lifePrefab);
            lifes[i] = lifeObject;
        }
    }

    /// <summary>
    /// ���� �ʱ�ȭ
    /// </summary>
    private void Enable()
    {
        // list �ʱ�ȭ
        tourIndex.Clear();
        saveIndex.Clear();
        removeIndex.Clear();

        maxLife = MaxLifeSetting(uiManager.GetDegree());
        life = new Life(maxLife, lifes, lifeParent);

        // ���̿� �ʺ� �°� Object ��ġ
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Random�� �̿��� �پ��� Color�� ������ ����
                var randIndex = UnityEngine.Random.Range(0, prefab.Length);
                var position = new Vector2(startX + x * cellSize, startY - y * cellSize);

                // Object ���� �� ��ġ
                var newElement = objectFactory.CreateUIObject((ElementColor)randIndex, position, Quaternion.identity, Vector3.one, board.transform);
                var info = newElement.GetElementInfo();
                info.position = new Vector2(x, y);
                newElement.SetElementInfo(info);

                uiElements[x, y] = newElement;
            }
        }

        InitCheckBoard();
    }

    /// <summary>
    /// Board Object ����
    /// </summary>
    private void DisEnable()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (uiElements[x, y] != null)
                {
                    objectFactory.DestoryUIObject(uiElements[x, y].GetGameObject());
                    uiElements[x, y] = null;
                }
            }
        }
    }

    /// <summary>
    /// ���̵��� ���� Life ����
    /// </summary>
    /// <param name="_degree">���̵�</param>
    /// <returns>Life ����</returns>
    private int MaxLifeSetting(Level _degree)
    {
        return ((int)(Level.Hard) - (int)_degree + 1) * 2 - 1;
    }

    #region Swap
    /// <summary>
    /// �����ϴ� Object
    /// </summary>
    /// <param name="_selectObject">������ Object</param>
    private void SelectElement(IUIElement _selectObject)
    {
        // �������̸� �������� �� �ϰ� �Ѵ�.
        if (isProcessing)
            return;

        var materialManager = Locator<MaterialManager>.Get();

        // ���� Swap object�� Ȯ���Ѵ�.
        if (swapObject == null)
        {
            // ���� object�� �����ߴٰ� Outline�� Ȱ��ȭ�ϰ� swapObject�� ������ object�� �Ѵ�.
            swapObject = _selectObject;
            materialManager.IsEnableOutline(swapObject.GetGameObject(), true);
        }
        else
        {
            // swapObject�� outline�� ��Ȱ��ȭ �Ѵ�.
            materialManager.IsEnableOutline(swapObject.GetGameObject(), false);

            // ���� ���� �����ϴ��� Ȯ���Ѵ�.
            if (IsExistRangeElement(_selectObject))
                StartCoroutine(SwapElement(swapObject, _selectObject, false));
            else
                swapObject = null;
        }
    }

    /// <summary>
    /// Target object�� index ���̸� Ȯ���Ѵ�.
    /// </summary>
    /// <param name="targetObject">�� �� object</param>
    /// <returns>Range���� ����</returns>
    private bool IsExistRangeElement(IUIElement targetObject)
    {
        var firstObjectIndex = swapObject.GetElementInfo().position;
        var secondObjectIndex = targetObject.GetElementInfo().position;

        return Mathf.Abs(firstObjectIndex.x - secondObjectIndex.x) +
            Mathf.Abs(firstObjectIndex.y - secondObjectIndex.y) == 1 ? true : false;
    }

    /// <summary>
    /// Swap�ϴ� �޼���
    /// </summary>
    /// <param name="_swapObject">Swap�� Object</param>
    /// <param name="changeObject">�ٲ� object</param>
    /// <param name="isReturn">Match ���� ���� ����</param>
    private IEnumerator SwapElement(IUIElement _swapObject, IUIElement changeObject, bool isReturn)
    {
        // ���� ���̶�� �����Ѵ�.
        isProcessing = true;

        // Element ������ ������ �´�.
        var originObjectElementInfo = _swapObject.GetElementInfo();
        var changeObjectElementInfo = changeObject.GetElementInfo();

        // UI�� ��ǥ�� �����´�.
        var firstelementUIPos = _swapObject.GetRectTransform();
        var secondelementUIPos = changeObject.GetRectTransform();

        // �ʱ� Swap�� Vector ��ġ�� �����Ѵ�.
        Vector2 firstObjectVector = new Vector2(firstelementUIPos.anchoredPosition.x, firstelementUIPos.anchoredPosition.y);
        Vector2 secondObjectVector = new Vector2(secondelementUIPos.anchoredPosition.x, secondelementUIPos.anchoredPosition.y);

        // Animation Refill�� �ʿ��� parameter�� �����Ѵ�.
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
            // ù��° ��ġ���� DFS�� Ȯ���ؼ� ���� �Ǵ� Object�� �ִ��� Ȯ���Ѵ�.
            SwapDFS(firstIndex.x, firstIndex.y, changeObjectElementInfo.color, Direction.None, 1);
            if (saveIndex.Count >= 3)
            {
                foreach (var save in saveIndex)
                {
                    removeIndex.Add(save);
                }
            }
            tourIndex.Clear();

            // �ι�° ��ġ���� DFS�� Ȯ���ؼ� ���� �Ǵ� Object�� �ִ��� Ȯ���Ѵ�.
            SwapDFS(secondIndex.x, secondIndex.y, originObjectElementInfo.color, Direction.None, 1);
            if (saveIndex.Count >= 3)
            {
                foreach (var save in saveIndex)
                {
                    removeIndex.Add(save);
                }
            }
            tourIndex.Clear();

            // Match�� �������θ� Ȯ���� �ٽ� ���� �ڸ��� �ǵ��� ������ ������ ������ ���Ѵ�.
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

    /// <summary>
    /// Swap�� �������� �� ���Ǵ� �޼���
    /// </summary>
    private void FailSwap()
    {
        // �� ������ Job System�� ����ϴ� �ž�.
        var matchFailJob = new MatchFailJob();
        JobHandle jobHandle = matchFailJob.Schedule();

        var soundManager = Locator<SoundManager>.Get();
        soundManager.PlaySFX(SFX.NoneSwap);

        swapCount++;
        life.DestoryLife(swapCount);

        if (!life.DestoryLife(swapCount))
        {
            Debug.Log("game over");

            // ���� UI ����
            DisEnable();

            swapCount = 0;
            eventManager.Notify(ChannelInfo.ResetScore);
            var uiManager = Locator<UIManager>.Get();
            var gameOver = uiManager.GetUIPrefabObject(UIPrefab.Gameover);
            gameOver.SetActive(true);
            var uiBoard = uiManager.GetUIPrefabObject(UIPrefab.Board);
            uiBoard.SetActive(false);
        }

        jobHandle.Complete();
    }

    /// <summary>
    /// �������� Ȯ���ϴ� DFS
    /// </summary>
    /// <param name="x">X ��ǥ</param>
    /// <param name="y">Y ��ǥ</param>
    /// <param name="_color">����</param>
    /// <param name="_direction">����</param>
    /// <param name="count">������ Object ����</param>
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
        // �湮�ߴٰ� �˸���.
        elementInfo.isVisits = true;
        uiElements[x, y].SetElementInfo(elementInfo);

        tourIndex.Add((x, y));

        if (tourIndex.Count >= 3)
        {
            saveIndex = tourIndex;
        }

        // ���⿡ �°� �ٽ� SWAPDFS�� ����.
        switch (_direction)
        {
            case Direction.Horizontal:
                SwapDFS(x + 1, y, _color, Direction.Horizontal, count);
                SwapDFS(x - 1, y, _color, Direction.Horizontal, count);
                break;
            case Direction.Vertical:
                SwapDFS(x, y + 1, _color, Direction.Vertical, count);
                SwapDFS(x, y - 1, _color, Direction.Vertical, count);
                break;
            case Direction.None:
                SwapDFS(x, y + 1, _color, Direction.Vertical, count);
                SwapDFS(x, y - 1, _color, Direction.Vertical, count);
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
                SwapDFS(x + 1, y, _color, Direction.Horizontal, count);
                SwapDFS(x - 1, y, _color, Direction.Horizontal, count);
                break;
        }
        // BackTracking���� �湮���θ� �����Ѵ�.
        elementInfo.isVisits = false;
        uiElements[x, y].SetElementInfo(elementInfo);
    }
    #endregion

    #region Refill
    /// <summary>
    /// ������ Object�� ���� Frame�� ������ �������� �ϴ� �޼���
    /// </summary>
    /// <returns></returns>
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
    }

    /// <summary>
    /// �̵��� �ڿ������� �ϵ��� �ϴ� �޼���
    /// </summary>
    /// <param name="moveList">�ʱ� ��ġ, ������ ��ġ, target UI Transform</param>
    /// <returns></returns>
    private IEnumerator AnimateMovement(List<(Vector2 start, Vector2 end, RectTransform target)> moveList)
    {
        float elapsed = 0f;

        // Duration ���� Ŭ ������ ������Ű�鼭 start -> end ��ġ�� �̵��Ѵ�.
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

        // ������ ������ ��ġ�� ������ ��ġ�� ������Ų��.
        foreach (var move in moveList)
        {
            move.target.anchoredPosition = move.end;
        }
    }

    /// <summary>
    /// MaxDepth�� �ٽ� �����Ѵ�.
    /// </summary>
    /// <param name="_value">��ĭ ������ �ִ� ���̸� ��� �ִ� List</param>
    /// <param name="index">X ��ġ</param>
    /// <returns>��ĭ ������ �ִ� ���̸� ���� List</returns>
    private (int blankCount, int maxDepth) ResearchMaxDepth((int blankCount, int maxDepth) _value, int index)
    {
        _value.blankCount--;

        // ��ĭ ������ 0�� ���϶�� return �Ѵ�.
        if (_value.blankCount <= 0)
        {
            return _value;
        }

        // depth�� Ȯ���ϸ鼭 null �˻縦 �ؼ� �ִٸ� maxDepth�� �����Ѵ�.
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

    /// <summary>
    /// �ִ� ���̿� ��ĭ ������ Ȯ���ؼ� ��ĭ�� �ִ��� ������ Ȯ���Ѵ�.
    /// </summary>
    /// <param name="_blankList">��ĭ ������ �ִ� ���̸� �����ص� List</param>
    /// <returns>��ĭ ����</returns>
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

    /// <summary>
    /// removeList ��ǥ���� Board�� ��ĭ�� �ִ��� Ȯ���Ѵ�.
    /// </summary>
    /// <param name="_removeList">RemoveList</param>
    /// <returns>��ĭ ������ ���̸� �����ص� List</returns>
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
    /// <summary>
    /// Match�� ������ Object�� �����Ѵ�.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DestoryElement()
    {
        // ���⼭ Job System ����Ѵ�.
        var matchSuccessJob = new MatchSuccessJob();

        // Match Sucess Job�� ����ǰ� �ִ�.
        JobHandle jobHandle = matchSuccessJob.Schedule();

        isProcessing = true;
        eventManager.Notify(ChannelInfo.Score, removeIndex.Count);

        var soundManager = Locator<SoundManager>.Get();
        soundManager.PlaySFX(SFX.Pop);

        // Destory
        foreach (var remove in removeIndex)
        {
            if (uiElements[remove.x, remove.y] != null)
            {
                objectFactory.DestoryUIObject(uiElements[remove.x, remove.y].GetGameObject());
                uiElements[remove.x, remove.y] = null;
            }
        }

        yield return new WaitForSeconds(duration);
        // �ٸ� Job�� ���������� ��ٸ���.
        jobHandle.Complete();

        // ���������� �ٽ� ä���.
        StartCoroutine(AnimationReFill());
    }
    #endregion

    #region CheckBoard
    /// <summary>
    /// ��ȸ�ϸ鼭 Match�� �������θ� Ȯ���Ѵ�.
    /// </summary>
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
        else
            isProcessing = false;
    }
    #endregion

    #region Initalize Setting Board
    /// <summary>
    /// �ʱ� ������ Board���� Match�� �ƴ��� Ȯ���ϴ� �޼���
    /// </summary>
    private void InitCheckBoard()
    {
        // ��ȸ�ϸ鼭 Ȯ���Ѵ�.
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                var color = uiElements[i, j].GetElementInfo().color;
                DFS(i, j, color, Direction.None, 1);
            }
        }

        // remove�� �����Ұ� ������ �����Ѵ�.
        if (removeIndex.Count > 0)
            InitDestroyElement();   
    }

    /// <summary>
    /// Match�� ���������� �����Ѵ�.
    /// </summary>
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

    /// <summary>
    /// ������ Object�� �ٽ� ä���.
    /// </summary>
    private void Refill()
    {
        // ��ĭ�� �ִ��� Ȯ���Ѵ�.
        var blankList = BlankCheckBoard(removeIndex);
        bool isRunning = CheckBlank(blankList);

        // while�� ���鼭 Ȯ���Ѵ�.
        while (!isRunning)
        {
            // BlankList�� ��ȸ�Ѵ�.
            for (int x = 0; x < blankList.Count; x++)
            {
                var value = blankList[x];

                // ��ĭ�� ���ٸ� �ѱ��.
                if (value.blankCount <= 0)
                    continue;
                
                // ��ĭ�� ���� object�� �غ��Ѵ�.
                var randIndex = UnityEngine.Random.Range(0, prefab.Length);
                Vector2 position = new Vector2(startX + x * cellSize, startY - 0 * cellSize);
                var newElement = objectFactory.CreateUIObject((ElementColor)randIndex, position, Quaternion.identity, Vector3.one, board.transform);

                // Element ������ �غ��Ѵ�.
                var newElementInfo = newElement.GetElementInfo();
                newElementInfo.position = new Vector2(x, 0);
                newElement.SetElementInfo(newElementInfo);

                // Max Depth�� Ȯ���ϸ鼭 object�� �Ʒ��� �� ���� ������.
                for (int y = value.maxDepth; y > 0; y--)
                {
                    // ������� �ʴٸ� �Ʒ��� �ش� Object�� �Ʒ��� ������.
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

                // �׸��� maxDepth�� ��� �ִ��� Ȯ���Ѵ�.
                if (uiElements[x, value.maxDepth] != null)
                    blankList[x] = ResearchMaxDepth(value, x);
            }

            isRunning = CheckBlank(blankList);
        }

        // ��ĭ �˻� �� �ٽ� Match�� �����ߴ��� Ȯ���Ѵ�.
        removeIndex.Clear();
        InitCheckBoard();
    }

    /// <summary>
    /// Board�� Match�� Ȯ���ϴ� �ٽ� �޼���
    /// </summary>
    /// <param name="x">x ��ǥ</param>
    /// <param name="y">Y ��ǥ</param>
    /// <param name="_color">����</param>
    /// <param name="_direction">����</param>
    /// <param name="count">����� ������ ����</param>
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

        // Element ������ ������ �ͼ� Color ��
        int currentColor = uiElements[x, y].GetElementInfo().color;
        
        // Color�� �ٸ��ٸ� return�Ѵ�.
        if (currentColor != _color)
        {
            // count�� 3�Ѿ�� �����Ѵ�.
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
        // �ش� ������ �����صд�.
        tourIndex.Add((x, y));

        // tourIndex�� 3�� �̻��̸� saveIndex�� �����صд�.
        if (tourIndex.Count >= 3)
        {
            saveIndex = tourIndex;
        }

        // ���⿡ �°� �̵��Ѵ�.
        switch (_direction)
        {
            // ������ Ȯ���Ѵ�.
            case Direction.Horizontal:
                DFS(x + 1, y, _color, Direction.Horizontal, count);
                break;
            // ������ Ȯ���Ѵ�.
            case Direction.Vertical:
                DFS(x, y + 1, _color, Direction.Vertical, count);
                break;
            // ������ Ȯ���ϰ� �ʱ�ȭ ��Ų �� ������ Ȯ���Ѵ�.
            case Direction.None:
                DFS(x, y + 1, _color, Direction.Vertical, count);

                tourIndex.Add((x, y));
                if (tourIndex.Count >= 3)
                {
                    saveIndex = tourIndex;
                    foreach (var save in saveIndex)
                    {
                        removeIndex.Add(save);
                    }
                }

                DFS(x + 1, y, _color, Direction.Horizontal, count);
                break;
        }
    }
    #endregion
}
