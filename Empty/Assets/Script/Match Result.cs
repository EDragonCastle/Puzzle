using System.Collections.Generic;

public class MatchResult 
{
    private List<ElementData> connectedElements;
    private MatchDirection direction;

    // Property {get; set;} ���� ���� �ʴ�? ���� ���
    public List<ElementData> GetElementList() => connectedElements;
    public void SetGetElementList(List<ElementData> _elementList) => connectedElements = _elementList;

    // Property
    public MatchDirection GetDirection() => direction;
    public void SetDirection(MatchDirection _direction) => direction = _direction;
}

public enum MatchDirection
{
    Vertical,
    Horizontal,
    LongVertical,
    LongHorizontal,
    Super,
    None,
}