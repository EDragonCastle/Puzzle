using System.Collections;
using System.Collections.Generic;
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


    private void Start()
    {
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
    }


    private void ClickObject(GameObject targetObject)
    {
        if (targetObject.GetComponent<ElementData>() != null)
        {
            var element = targetObject.GetComponent<ElementData>();
            SelectElement(element);
        }
    }

    private void DestoryElement()
    {
        if(elementToDestory != null) {
            var log = Locator.GetLogManager();
            log.Info("Destory Match");

            foreach(var element in elementToDestory) {
                Destroy(element);
            }
            elementToDestory.Clear();
        }
    }

    public bool CheckBoard()
    { 
        var log = Locator.GetLogManager();
        log.Info("Check Board");

        bool hasMatched = false;

        List<ElementData> removeData = new();

        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                if (elementBoard[x, y].isUsable) {
                    var element = elementBoard[x, y].element.GetComponent<ElementData>();

                    if(!element.isMatched) {
                        var matchedElement = IsConnected(element);

                        if(matchedElement.GetElementList().Count >= 3) {
                            removeData.AddRange(matchedElement.GetElementList());

                            foreach(var matchElement in matchedElement.GetElementList()) {
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

    MatchResult IsConnected(ElementData element)
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
            // 해당 내용을 함수로 뺄 수도 있으려나?
            var log = Locator.GetLogManager();
            log.Info("Horizontal Match" + connectedElement[0].elementType);

            matchResult.SetGetElementList(connectedElement);
            matchResult.SetDirection(MatchDirection.Horizontal);
            return matchResult;
        }
        else if (connectedElement.Count > 3) {
            // 해당 내용을 함수로 뺄 수도 있으려나?
            var log = Locator.GetLogManager();
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
            // 해당 내용을 함수로 뺄 수도 있으려나?
            var log = Locator.GetLogManager();
            log.Info("Vertical Match" + connectedElement[0].elementType);

            matchResult.SetGetElementList(connectedElement);
            matchResult.SetDirection(MatchDirection.Vertical);
            return matchResult;
        }
        else if (connectedElement.Count > 3) {
            // 해당 내용을 함수로 뺄 수도 있으려나?
            var log = Locator.GetLogManager();
            log.Info("Long Vertical Match" + connectedElement[0].elementType);

            matchResult.SetGetElementList(connectedElement);
            matchResult.SetDirection(MatchDirection.LongVertical);
            return matchResult;
        }

        matchResult.SetGetElementList(connectedElement);
        matchResult.SetDirection(MatchDirection.None);
        return matchResult;
    }

    void CheckDirection(ElementData element, Vector2Int direction, List<ElementData> connectedElement)
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

    public void SelectElement(ElementData element)
    {
        // selectElement가 비어 있다면 선택한다.
        if (selectElement == null) {
            var log = Locator.GetLogManager();
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
        if (IsAdjacent(currentElement, targetElement)) {
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
        var targetVector = currentElement.GetPosition();

        return Mathf.Abs(currentVector.x - targetVector.x) + Mathf.Abs(currentVector.y - targetVector.y) == 1;
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

        if(!hasMatch) {
            DoSwap(currentElement, targetElement);
        }

        isProcessing = false;
    }
}
