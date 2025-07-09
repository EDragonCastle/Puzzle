using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Board : MonoBehaviour
{
    // board size
    public int width = 6;
    public int height = 8;

    // board spacing 범위
    public float spacingX;
    public float spacingY;

    // element만 담긴 Prefab
    public GameObject[] elementPrefabs;

    // Node 정보가 담긴 Prefab
    private Node[,] elementBoard;
    public GameObject elementBoardGO;

    // Layout Array
    public ArrayLayout layout;

    // Element Destory
    public List<GameObject> elementToDestory = new();
    
    // 선택된 Element
    [SerializeField]
    private ElementData selectElement;
    // 차단 역할
    [SerializeField]
    private bool isProcessing;

    private List<ElementData> removeData = new();

    private LogManager log;

    // event 설정
    private void OnNoticeObjectClicked(object sender, NoticeObject.ClickEventArgs e)
    {
        ClickObject(e.ClikedObject);
    }


    private void Start()
    {
        log = Locator.GetLogManager();
        NoticeObject.Click += OnNoticeObjectClicked;
        Initalize();
    }

   
    private void Initalize()
    {
        DestoryElement();
        // row col 
        elementBoard = new Node[width, height];
        spacingX = (float)(width - 1) / 2;
        spacingY = (float)(height - 1) / 2;

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                var position = new Vector2((x - spacingX) * 110, (y - spacingY) * 110);

                if (layout.rows[y].row[x]) {
                    elementBoard[x, y] = new Node(false, null);
                }
                else {
                    int randomIndex = Random.Range(0, (int)ElementType.None);

                    // Object 생성 -> 나중에 Object Pool로 바꿔주면 된다.
                    GameObject element = Instantiate(elementPrefabs[randomIndex], position, Quaternion.identity);
                    element.transform.SetParent(elementBoardGO.transform, false);

                    var rectTransform = element.GetComponent<RectTransform>();

                    // Canvas 기준인 Rect Transform을 이용해야 한다.
                    if (rectTransform != null) {
                        rectTransform.anchoredPosition = position;
                        rectTransform.anchorMin = Vector2.one * 0.5f;
                        rectTransform.anchorMax = Vector2.one * 0.5f;
                        rectTransform.pivot = Vector2.one * 0.5f;
                    }

                    element.GetComponent<ElementData>().SetPosition(x, y);
                    elementBoard[x, y] = new Node(true, element);
                    elementToDestory.Add(element);
                }
            }
        }

        if(CheckBoard())
        {
            Initalize();
        }
    }


    private void ClickObject(GameObject targetObject)
    {
        if (isProcessing)
            return;

        if (targetObject.GetComponent<ElementData>() != null)
        {
            var element = targetObject.GetComponent<ElementData>();
            SelectElement(element);
        }
    }

    private void DestoryElement()
    {
        if(elementToDestory != null) {
            log.Info("Destory Match");

            foreach(var element in elementToDestory) {
                Destroy(element);
            }
            elementToDestory.Clear();
        }
    }

    private bool CheckBoard()
    { 

        bool hasMatched = false;
        removeData.Clear();

        foreach(var node in elementBoard)
        {
            if(node.element != null)
            {
                node.element.GetComponent<ElementData>().isMatched = false;
            }
        }

        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                if (elementBoard[x, y].isUsable) {
                    var element = elementBoard[x, y].element.GetComponent<ElementData>();

                    if(!element.isMatched) {
                        var matchedElement = IsConnected(element);

                        if(matchedElement.GetElementList().Count >= 3) {
                            // complex matching
                            MatchResult superMatchElement = SuperMatch(matchedElement);
                            
                            removeData.AddRange(superMatchElement.GetElementList());

                            foreach(var matchElement in superMatchElement.GetElementList()) {
                                matchElement.isMatched = true;
                            }
                            hasMatched = true;
                        }
                    }
                }
            }
        }

        return hasMatched;
    }

    private IEnumerator ProcessTurnOnMatchBoard(bool subtrackMove)
    {
        foreach(var element in removeData)
        {
            element.isMatched = false;
        }

        RemoveAndRefill(removeData);
        yield return new WaitForSeconds(0.4f);

        if(CheckBoard())
        {
            StartCoroutine(ProcessTurnOnMatchBoard(false));
        }
    }

    private void RemoveAndRefill(List<ElementData> removeData)
    {
        foreach (var element in removeData)
        {
            var elementVector = element.GetPosition();
            int x = elementVector.x;
            int y = elementVector.y;

            Destroy(element.gameObject);
            elementBoard[x, y] = new Node(true, null);
        }

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                if (elementBoard[x, y].element == null)
                {
                    log.Info("The Location" + x + " " + y);
                    RefillElement(x, y);
                }
            }
        }
    }

    private void RefillElement(int x, int y)
    {
        int yOffset = 1;

        while (y + yOffset < height && elementBoard[x, y + yOffset].element == null) {
            yOffset++;
        }

        if (y + yOffset < height && elementBoard[x, y + yOffset].element != null)
        {
            ElementData element = elementBoard[x, y + yOffset].element.GetComponent<ElementData>();

            var targetPos = new Vector2((x - spacingX) * 110, (y - spacingY) * 110);
            
            element.UIMoveToTarget(targetPos);
            element.SetPosition(x, y);
            elementBoard[x, y] = elementBoard[x, y + yOffset];

            elementBoard[x, y + yOffset] = new Node(true, null);
        }

        if(y + yOffset == height)
        {
            SpwnElementAtTop(x);
        }
    }

    private void SpwnElementAtTop(int x)
    {
        int index = FindIndexOfLowerNull(x);
        // 8은 max size를 말한다.
        int locationToMove = 8 - index;
        int randomIndex = Random.Range(0, (int)ElementType.None);

        var position = new Vector2((x - spacingX) * 110, (height - 1 - spacingY) * 110);
        GameObject newElement = Instantiate(elementPrefabs[randomIndex], position, Quaternion.identity);
        newElement.transform.SetParent(elementBoardGO.transform, false);
        
        var rectTransform = newElement.GetComponent<RectTransform>();

        rectTransform.anchoredPosition = position;

        newElement.GetComponent<ElementData>().SetPosition(x, index);
        elementBoard[x, index] = new Node(true, newElement);

        var targetPos = new Vector2(newElement.transform.position.x, (newElement.transform.position.y - locationToMove));
        //var targetPos = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y - locationToMove);
        
        newElement.GetComponent<ElementData>().MoveToTarget(targetPos);
        //newElement.GetComponent<ElementData>().UIMoveToTarget(targetPos);
    }

    private int FindIndexOfLowerNull(int x)
    {
        int lowerNull = 99;
        for(int y = 7; y >= 0; y--)
        {
            if (elementBoard[x,y].element == null)
            {
                lowerNull = y;
            }
        }

        return lowerNull;
    }

    private MatchResult SuperMatch(MatchResult matchedElement)
    {
        // Match Result에서 Horizontal을 검사한다.
        if(matchedElement.GetDirection() == MatchDirection.Horizontal || matchedElement.GetDirection() == MatchDirection.LongHorizontal) {
            foreach(ElementData element in matchedElement.GetElementList()) {
                var extraConnectElement = new List<ElementData>();

                CheckDirection(element, new Vector2Int(0, 1), extraConnectElement);
                CheckDirection(element, new Vector2Int(0, -1), extraConnectElement);

                if(extraConnectElement.Count >=2) {
                    log.Info("Super Horizontal Match");
                    extraConnectElement.AddRange(matchedElement.GetElementList());

                    var superMatchResult = new MatchResult();
                    superMatchResult.SetDirection(MatchDirection.Super);
                    superMatchResult.SetGetElementList(extraConnectElement);
                    return superMatchResult;
                }
            }
            return matchedElement;
        }
        // Match Result에서 Vertical 검사한다.
        else if (matchedElement.GetDirection() == MatchDirection.Vertical || matchedElement.GetDirection() == MatchDirection.LongVertical) {
            foreach (ElementData element in matchedElement.GetElementList()) {
                var extraConnectElement = new List<ElementData>();

                CheckDirection(element, new Vector2Int(1, 0), extraConnectElement);
                CheckDirection(element, new Vector2Int(-1, 0), extraConnectElement);

                if (extraConnectElement.Count >=2) {
                    log.Info("Super Vertical Match");
                    extraConnectElement.AddRange(matchedElement.GetElementList());

                    var superMatchResult = new MatchResult();
                    superMatchResult.SetDirection(MatchDirection.Super);
                    superMatchResult.SetGetElementList(extraConnectElement);
                    return superMatchResult;
                }
            }
            return matchedElement;
        }
        return null;
    }

    private MatchResult IsConnected(ElementData element)
    {
        List<ElementData> connectedElement = new();
        ElementType elementType = element.elementType;

        connectedElement.Add(element);

        /// check element right, left
        CheckDirection(element, new Vector2Int(1, 0), connectedElement);
        CheckDirection(element, new Vector2Int(-1, 0), connectedElement);
        
        var matchResult = new MatchResult();

        // 3 match
        if (connectedElement.Count == 3) {
            log.Info("Horizontal Match" + connectedElement[0].elementType);

            matchResult.SetGetElementList(connectedElement);
            matchResult.SetDirection(MatchDirection.Horizontal);
            return matchResult;
        }
        else if (connectedElement.Count > 3) {
            log.Info("Long Horizontal Match" + connectedElement[0].elementType);

            matchResult.SetGetElementList(connectedElement);
            matchResult.SetDirection(MatchDirection.LongHorizontal);
            return matchResult;
        }

        // List clear 및 initalize
        connectedElement.Clear();
        connectedElement.Add(element);

        /// check element up, down
        CheckDirection(element, new Vector2Int(0, 1), connectedElement);
        CheckDirection(element, new Vector2Int(0, -1), connectedElement);

        // 3 match
        if (connectedElement.Count == 3) {
            log.Info("Vertical Match" + connectedElement[0].elementType);

            matchResult.SetGetElementList(connectedElement);
            matchResult.SetDirection(MatchDirection.Vertical);
            return matchResult;
        }
        else if (connectedElement.Count > 3) {
            log.Info("Long Vertical Match" + connectedElement[0].elementType);

            matchResult.SetGetElementList(connectedElement);
            matchResult.SetDirection(MatchDirection.LongVertical);
            return matchResult;
        }

        matchResult.SetGetElementList(connectedElement);
        matchResult.SetDirection(MatchDirection.None);
        return matchResult;
    }

    private void CheckDirection(ElementData element, Vector2Int direction, List<ElementData> connectedElement)
    {
        ElementType elementType = element.elementType;
        var elementVector = element.GetPosition();

        int x = elementVector.x + direction.x;
        int y = elementVector.y + direction.y;

        while(x >= 0 && y >= 0 && x < width && y < height) {
            if (elementBoard[x, y].isUsable) {
                var neighborElement = elementBoard[x, y].element.GetComponent<ElementData>();
                if (!neighborElement.isMatched && neighborElement.elementType == elementType) {
                    connectedElement.Add(neighborElement);
                    x += direction.x;
                    y += direction.y;
                }
                else {
                    break;
                }
            }
            else { 
                break; 
            }
        }
    }

    private void SelectElement(ElementData element)
    {
        // selectElement가 비어 있다면 선택한다.
        if (selectElement == null) {
            log.Info(element.name);
            selectElement = element;
        }
        // 같은 것을 선택했다면 다시 null로 만든다.
        else if (selectElement == element) {
            selectElement = null;
        }
        // 그것도 아니라면 바꾼다.
        else if(selectElement != element) {
            SwapElement(selectElement, element);
            selectElement = null;
        }
    }

    private void SwapElement(ElementData currentElement, ElementData targetElement)
    {
        // Adjacent
        if (!IsAdjacent(currentElement, targetElement)) {
            return;
        }

        // Swap
        DoSwap(currentElement, targetElement);

        isProcessing = true;

        // Process Match ... Coroutine으로 된 건 다 Job System으로 가능할까?
        StartCoroutine(ProcessMatches(currentElement, targetElement));
    }
    private bool IsAdjacent(ElementData currentElement, ElementData targetElement)
    {
        var currentVector = currentElement.GetPosition();
        var targetVector = targetElement.GetPosition();

        return (Mathf.Abs(currentVector.x - targetVector.x) + Mathf.Abs(currentVector.y - targetVector.y)) == 1;
    }

    private void DoSwap(ElementData currentElement, ElementData targetElement)
    {
        // ref Keyword 없어도 Class 자체가 ref type이라 keyword 없어도 괜찮다.
        GameObject temp = elementBoard[currentElement.GetPosition().x, currentElement.GetPosition().y].element;
        elementBoard[currentElement.GetPosition().x, currentElement.GetPosition().y].element = elementBoard[targetElement.GetPosition().x, targetElement.GetPosition().y].element;
        elementBoard[targetElement.GetPosition().x, targetElement.GetPosition().y].element = temp;
        
        // Index Swap
        Vector2Int tempVector = currentElement.GetPosition();
        currentElement.SetPosition(targetElement.GetPosition().x, targetElement.GetPosition().y);
        targetElement.SetPosition(tempVector.x, tempVector.y);

        currentElement.MoveToTarget(elementBoard[targetElement.GetPosition().x, targetElement.GetPosition().y].element.transform.position);
        targetElement.MoveToTarget(elementBoard[currentElement.GetPosition().x, currentElement.GetPosition().y].element.transform.position);
    }

    private IEnumerator ProcessMatches(ElementData currentElement, ElementData targetElement)
    {
        yield return new WaitForSeconds(0.2f);

        bool hasMatch = CheckBoard();

        if(hasMatch) {
            StartCoroutine(ProcessTurnOnMatchBoard(true));
        }
        else {
            DoSwap(currentElement, targetElement);
        }

        isProcessing = false;
    }
}
