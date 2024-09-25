using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OmokStoneEnum;

public class OmokStone : MonoBehaviour
{
    [SerializeField] StoneColor stoneColor;

    StoneData omokStoneData = null;
    public StoneData OmokStoneData { get { return omokStoneData; } }
    private void Awake()
    {
        CreateStoneData();
    }

    public void CreateStoneData()
    {
        Vector2Int _stoneCoordinate = new Vector2Int((int)this.transform.position.x, (int)this.transform.position.y);
        omokStoneData = new StoneData(_stoneCoordinate, stoneColor);
    }

    public void LinkStoneData()
    {
        // 매니저에 해당 스톤이 놓인 위치를 전달한다.
    }
}


public class StoneData
{
    public Vector2Int coordinate;
    public StoneColor color;

    public StoneData(Vector2Int coordinate, StoneColor color)
    {
        this.coordinate = coordinate;
        this.color = color;
    }
}