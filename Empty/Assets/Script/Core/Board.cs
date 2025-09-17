using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;

/// <summary>
/// Puzzle이 어디로 갈지 정할 Direction 위치
/// </summary>
public enum Direction
{
    Vertical,
    Horizontal,
    None,
}

/// <summary>
/// 게임의 핵심을 담고 있는 Board Class
/// </summary>
public class Board : MonoBehaviour, IChannel
{
    [SerializeField]
    private int width = 6;

    [SerializeField]
    private int height = 8;

    // 공백을 담당하고 있다.
    private readonly int cellSize = 110;

    private bool isProcessing;

    // Animation Move할 때 걸리는 시간
    private float duration = 0.2f;

    // 중앙에 위치해야 할 object다.
    private float startX;
    private float startY;

    public GameObject[] prefab;
    public GameObject board;

    // Board Object다.
    private IUIElement[,] uiElements;

    // DFS에 필요한 Index 저장할 변수
    private List<(int x, int y)> tourIndex;
    private List<(int x, int y)> saveIndex;
    private HashSet<(int x, int y)> removeIndex;

    private int swapCount = 0;

    // swap에 필요할 object
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

    // 이벤트 등록 및 Board 초기화
    private void OnEnable()
    {
        Enable();
        eventManager.Subscription(ChannelInfo.Select, HandleEvent);
    }

    // 이벤트 해지 및 Board 정리
    private void OnDisable()
    {
        DisEnable();
        eventManager.Unsubscription(ChannelInfo.Select, HandleEvent);
    }

    /// <summary>
    /// Event Manager에 사용할 IChannel Interface
    /// </summary>
    /// <param name="channel">채널 정보</param>
    /// <param name="information">사용할 Object 내용</param>
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
    /// Board 초기 생성
    /// </summary>
    private void Initalize()
    {
        // 시작 x, y 위치 세팅
        startX = -((width * 0.5f - 0.5f) * cellSize);
        startY = ((height * 0.5f - 0.5f) * cellSize);

        // 변수 초기화
        uiElements = new IUIElement[width, height];
        tourIndex = new List<(int x, int y)>();
        saveIndex = new List<(int x, int y)>();
        removeIndex = new HashSet<(int x, int y)>();

        // Life 생성
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
    /// 보드 초기화
    /// </summary>
    private void Enable()
    {
        // list 초기화
        tourIndex.Clear();
        saveIndex.Clear();
        removeIndex.Clear();

        maxLife = MaxLifeSetting(uiManager.GetDegree());
        life = new Life(maxLife, lifes, lifeParent);

        // 높이와 너비에 맞게 Object 설치
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Random을 이용해 다양한 Color가 나오게 설정
                var randIndex = UnityEngine.Random.Range(0, prefab.Length);
                var position = new Vector2(startX + x * cellSize, startY - y * cellSize);

                // Object 생성 후 배치
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
    /// Board Object 제거
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
    /// 난이도에 따른 Life 개수
    /// </summary>
    /// <param name="_degree">난이도</param>
    /// <returns>Life 개수</returns>
    private int MaxLifeSetting(Level _degree)
    {
        return ((int)(Level.Hard) - (int)_degree + 1) * 2 - 1;
    }

    #region Swap
    /// <summary>
    /// 선택하는 Object
    /// </summary>
    /// <param name="_selectObject">선택한 Object</param>
    private void SelectElement(IUIElement _selectObject)
    {
        // 진행중이면 선택하지 못 하게 한다.
        if (isProcessing)
            return;

        var materialManager = Locator<MaterialManager>.Get();

        // 현재 Swap object를 확인한다.
        if (swapObject == null)
        {
            // 현재 object를 선택했다고 Outline을 활성화하고 swapObject를 선택한 object로 한다.
            swapObject = _selectObject;
            materialManager.IsEnableOutline(swapObject.GetGameObject(), true);
        }
        else
        {
            // swapObject의 outline을 비활성화 한다.
            materialManager.IsEnableOutline(swapObject.GetGameObject(), false);

            // 범위 내에 존재하는지 확인한다.
            if (IsExistRangeElement(_selectObject))
                StartCoroutine(SwapElement(swapObject, _selectObject, false));
            else
                swapObject = null;
        }
    }

    /// <summary>
    /// Target object와 index 차이를 확인한다.
    /// </summary>
    /// <param name="targetObject">비교 할 object</param>
    /// <returns>Range내의 여부</returns>
    private bool IsExistRangeElement(IUIElement targetObject)
    {
        var firstObjectIndex = swapObject.GetElementInfo().position;
        var secondObjectIndex = targetObject.GetElementInfo().position;

        return Mathf.Abs(firstObjectIndex.x - secondObjectIndex.x) +
            Mathf.Abs(firstObjectIndex.y - secondObjectIndex.y) == 1 ? true : false;
    }

    /// <summary>
    /// Swap하는 메서드
    /// </summary>
    /// <param name="_swapObject">Swap할 Object</param>
    /// <param name="changeObject">바뀔 object</param>
    /// <param name="isReturn">Match 성공 실패 여부</param>
    private IEnumerator SwapElement(IUIElement _swapObject, IUIElement changeObject, bool isReturn)
    {
        // 진행 중이라고 설정한다.
        isProcessing = true;

        // Element 정보를 가지고 온다.
        var originObjectElementInfo = _swapObject.GetElementInfo();
        var changeObjectElementInfo = changeObject.GetElementInfo();

        // UI상 좌표를 가져온다.
        var firstelementUIPos = _swapObject.GetRectTransform();
        var secondelementUIPos = changeObject.GetRectTransform();

        // 초기 Swap할 Vector 위치를 설정한다.
        Vector2 firstObjectVector = new Vector2(firstelementUIPos.anchoredPosition.x, firstelementUIPos.anchoredPosition.y);
        Vector2 secondObjectVector = new Vector2(secondelementUIPos.anchoredPosition.x, secondelementUIPos.anchoredPosition.y);

        // Animation Refill에 필요한 parameter값 지정한다.
        var moveList = new List<(Vector2 start, Vector2 end, RectTransform target)>();
        moveList.Add((firstObjectVector, secondObjectVector, firstelementUIPos));
        moveList.Add((secondObjectVector, firstObjectVector, secondelementUIPos));

        yield return StartCoroutine(AnimateMovement(moveList));

        // Position을 가져온다.
        Vector2Int firstIndex = new Vector2Int((int)originObjectElementInfo.position.x, (int)originObjectElementInfo.position.y);
        Vector2Int secondIndex = new Vector2Int((int)changeObjectElementInfo.position.x, (int)changeObjectElementInfo.position.y);

        // index 값도 바꿔준다.
        var tempIndex = originObjectElementInfo.position;
        originObjectElementInfo.position = changeObjectElementInfo.position;
        changeObjectElementInfo.position = tempIndex;

        // 설정을 마친다.
        _swapObject.SetElementInfo(originObjectElementInfo);
        changeObject.SetElementInfo(changeObjectElementInfo);

        // elements 값도 바꿔야한다.
        var tempElement = uiElements[firstIndex.x, firstIndex.y];
        uiElements[firstIndex.x, firstIndex.y] = uiElements[secondIndex.x, secondIndex.y];
        uiElements[secondIndex.x, secondIndex.y] = tempElement;
        
        // 왜 origine이 아니고 change냐면 SetElemeint로 확정했기 때문이다.
        // DFS로 가서 확인해야 하는데 두 개의 object를 확인하면 된다.
        if (!isReturn)
        {
            // 첫번째 위치에서 DFS를 확인해서 삭제 되는 Object가 있는지 확인한다.
            SwapDFS(firstIndex.x, firstIndex.y, changeObjectElementInfo.color, Direction.None, 1);
            if (saveIndex.Count >= 3)
            {
                foreach (var save in saveIndex)
                {
                    removeIndex.Add(save);
                }
            }
            tourIndex.Clear();

            // 두번째 위치에서 DFS를 확인해서 삭제 되는 Object가 있는지 확인한다.
            SwapDFS(secondIndex.x, secondIndex.y, originObjectElementInfo.color, Direction.None, 1);
            if (saveIndex.Count >= 3)
            {
                foreach (var save in saveIndex)
                {
                    removeIndex.Add(save);
                }
            }
            tourIndex.Clear();

            // Match의 성공여부를 확인해 다시 원래 자리로 되돌릴 것인지 삭제할 것인지 정한다.
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
    /// Swap에 실패했을 때 사용되는 메서드
    /// </summary>
    private void FailSwap()
    {
        // 이 곳에서 Job System을 사용하는 거야.
        var matchFailJob = new MatchFailJob();
        JobHandle jobHandle = matchFailJob.Schedule();

        var soundManager = Locator<SoundManager>.Get();
        soundManager.PlaySFX(SFX.NoneSwap);

        swapCount++;
        life.DestoryLife(swapCount);

        if (!life.DestoryLife(swapCount))
        {
            Debug.Log("game over");

            // 위에 UI 생성
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
    /// 전방향을 확인하는 DFS
    /// </summary>
    /// <param name="x">X 좌표</param>
    /// <param name="y">Y 좌표</param>
    /// <param name="_color">색상</param>
    /// <param name="_direction">방향</param>
    /// <param name="count">동일한 Object 개수</param>
    private void SwapDFS(int x, int y, int _color, Direction _direction, int count)
    {
        // 범위 넘어가면 돌아가자.
        if (x < 0 || width <= x || y < 0 || height <= y)
        {
            return;
        }

        var elementInfo = uiElements[x, y].GetElementInfo();
       
        // 방문했는 지 확인 후 색깔이 다른지 확인한다.
        if (elementInfo.isVisits || elementInfo.color != _color)
        {
            return;
        }

        count++;
        // 방문했다고 알린다.
        elementInfo.isVisits = true;
        uiElements[x, y].SetElementInfo(elementInfo);

        tourIndex.Add((x, y));

        if (tourIndex.Count >= 3)
        {
            saveIndex = tourIndex;
        }

        // 방향에 맞게 다시 SWAPDFS로 들어간다.
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
        // BackTracking으로 방문여부를 해제한다.
        elementInfo.isVisits = false;
        uiElements[x, y].SetElementInfo(elementInfo);
    }
    #endregion

    #region Refill
    /// <summary>
    /// 삭제된 Object를 여러 Frame에 나눠서 내려오게 하는 메서드
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

                // 이런 식으로 진행해야겠네? 이러면 한 번만해서 공백이 생겨서 remove.y에 해당하는 애들을 전부 옮겨야해.
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
    /// 이동을 자연스럽게 하도록 하는 메서드
    /// </summary>
    /// <param name="moveList">초기 위치, 마지막 위치, target UI Transform</param>
    /// <returns></returns>
    private IEnumerator AnimateMovement(List<(Vector2 start, Vector2 end, RectTransform target)> moveList)
    {
        float elapsed = 0f;

        // Duration 보다 클 때까지 증가시키면서 start -> end 위치로 이동한다.
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

        // 끝나면 마지막 위치에 마지막 위치로 고정시킨다.
        foreach (var move in moveList)
        {
            move.target.anchoredPosition = move.end;
        }
    }

    /// <summary>
    /// MaxDepth를 다시 설정한다.
    /// </summary>
    /// <param name="_value">빈칸 개수와 최대 깊이를 담고 있는 List</param>
    /// <param name="index">X 위치</param>
    /// <returns>빈칸 개수와 최대 깊이를 담은 List</returns>
    private (int blankCount, int maxDepth) ResearchMaxDepth((int blankCount, int maxDepth) _value, int index)
    {
        _value.blankCount--;

        // 빈칸 개수가 0개 이하라면 return 한다.
        if (_value.blankCount <= 0)
        {
            return _value;
        }

        // depth를 확인하면서 null 검사를 해서 있다면 maxDepth로 설정한다.
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
    /// 최대 깊이와 빈칸 개수를 확인해서 빈칸이 있는지 없는지 확인한다.
    /// </summary>
    /// <param name="_blankList">빈칸 개수와 최대 깊이를 저장해둔 List</param>
    /// <returns>빈칸 여부</returns>
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
    /// removeList 좌표에서 Board에 빈칸이 있는지 확인한다.
    /// </summary>
    /// <param name="_removeList">RemoveList</param>
    /// <returns>빈칸 개수와 깊이를 저장해둔 List</returns>
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
    /// Match에 성공한 Object를 삭제한다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DestoryElement()
    {
        // 여기서 Job System 사용한다.
        var matchSuccessJob = new MatchSuccessJob();

        // Match Sucess Job이 실행되고 있다.
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
        // 다른 Job이 끝날때까지 기다린다.
        jobHandle.Complete();

        // 삭제했으면 다시 채운다.
        StartCoroutine(AnimationReFill());
    }
    #endregion

    #region CheckBoard
    /// <summary>
    /// 순회하면서 Match의 성공여부를 확인한다.
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
    /// 초기 생성한 Board에서 Match가 됐는지 확인하는 메서드
    /// </summary>
    private void InitCheckBoard()
    {
        // 순회하면서 확인한다.
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                var color = uiElements[i, j].GetElementInfo().color;
                DFS(i, j, color, Direction.None, 1);
            }
        }

        // remove에 삭제할게 있으면 삭제한다.
        if (removeIndex.Count > 0)
            InitDestroyElement();   
    }

    /// <summary>
    /// Match에 성공했으면 삭제한다.
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
    /// 삭제한 Object는 다시 채운다.
    /// </summary>
    private void Refill()
    {
        // 빈칸이 있는지 확인한다.
        var blankList = BlankCheckBoard(removeIndex);
        bool isRunning = CheckBlank(blankList);

        // while로 돌면서 확인한다.
        while (!isRunning)
        {
            // BlankList를 순회한다.
            for (int x = 0; x < blankList.Count; x++)
            {
                var value = blankList[x];

                // 빈칸이 없다면 넘긴다.
                if (value.blankCount <= 0)
                    continue;
                
                // 빈칸에 넣을 object를 준비한다.
                var randIndex = UnityEngine.Random.Range(0, prefab.Length);
                Vector2 position = new Vector2(startX + x * cellSize, startY - 0 * cellSize);
                var newElement = objectFactory.CreateUIObject((ElementColor)randIndex, position, Quaternion.identity, Vector3.one, board.transform);

                // Element 정보도 준비한다.
                var newElementInfo = newElement.GetElementInfo();
                newElementInfo.position = new Vector2(x, 0);
                newElement.SetElementInfo(newElementInfo);

                // Max Depth를 확인하면서 object를 아래로 한 번씩 내린다.
                for (int y = value.maxDepth; y > 0; y--)
                {
                    // 비어있지 않다면 아래로 해당 Object를 아래로 내린다.
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

                // 그리고 maxDepth에 비어 있는지 확인한다.
                if (uiElements[x, value.maxDepth] != null)
                    blankList[x] = ResearchMaxDepth(value, x);
            }

            isRunning = CheckBlank(blankList);
        }

        // 빈칸 검사 후 다시 Match에 성공했는지 확인한다.
        removeIndex.Clear();
        InitCheckBoard();
    }

    /// <summary>
    /// Board의 Match를 확인하는 핵심 메서드
    /// </summary>
    /// <param name="x">x 좌표</param>
    /// <param name="y">Y 좌표</param>
    /// <param name="_color">색상</param>
    /// <param name="_direction">방향</param>
    /// <param name="count">색상과 동일한 개수</param>
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

        // Element 정보를 가지고 와서 Color 비교
        int currentColor = uiElements[x, y].GetElementInfo().color;
        
        // Color와 다르다면 return한다.
        if (currentColor != _color)
        {
            // count가 3넘어가면 제거한다.
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
        // 해당 정보를 저장해둔다.
        tourIndex.Add((x, y));

        // tourIndex가 3개 이상이면 saveIndex에 저장해둔다.
        if (tourIndex.Count >= 3)
        {
            saveIndex = tourIndex;
        }

        // 방향에 맞게 이동한다.
        switch (_direction)
        {
            // 수평을 확인한다.
            case Direction.Horizontal:
                DFS(x + 1, y, _color, Direction.Horizontal, count);
                break;
            // 수직을 확인한다.
            case Direction.Vertical:
                DFS(x, y + 1, _color, Direction.Vertical, count);
                break;
            // 수직을 확인하고 초기화 시킨 후 수평을 확인한다.
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
